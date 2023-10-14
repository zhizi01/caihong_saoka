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
        private SemaphoreSlim pauseSemaphore = new SemaphoreSlim(1, 1);  // ��ʼ״̬Ϊ����ͣ
        private const int THREAD_COUNT = 10;
        private ConcurrentQueue<string> qqQueue;
        private static string gl_atime = "";
        private static string gl_btime = "";
        private static string[] proxyIPList;
        private static int nowIPList = 0;
        private static int MAX_RETRY_COUNT = 3; // ������Դ�������ֹ����ѭ��
        public saoka()
        {
            InitializeComponent();
        }

        private void saoka_Load(object sender, EventArgs e)
        {
            addLog("ɨ���ű� �����汾: 23a02w(R)");
            addLog("���ȵ���������ϼ����������У�");
        }

        private void addLog(string text)
        {
            string time = DateTime.Now.ToString();
            logTextBox.AppendText($"[{time}] {text}\r\n");
            logTextBox.ScrollToCaret();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // ����һ�� OpenFileDialog �Ի��������û�ѡ���ļ�
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"; // ��ɸѡ�ı��ļ�
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); // Ĭ��λ������Ϊ����

            // ��ʾ�Ի��򲢻�ȡ���
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // ��ȡ�ļ�·��
                string selectedFilePath = openFileDialog.FileName;
                if (string.IsNullOrEmpty(selectedFilePath))
                {
                    return; // �����ַΪ�ջ��û�����ѡ��ֱ�ӷ���
                }
                else
                {
                    keyPath = selectedFilePath; // ���� keyPath ����
                    button1.Enabled = false;
                    button2.Enabled = true;
                    label1.Text = "�ֵ�λ��:" + keyPath;
                    addLog($"������{keyPath}�Ѽ�¼λ��,�����ƶ���ɾ��!");
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "��ʼ")
            {
                if (string.IsNullOrEmpty(keyPath))
                {
                    MessageBox.Show("�ȵ���������ϼ���������!");
                    return;
                }

                gl_atime = textBox3.Text;
                gl_btime = textBox4.Text;

                // ��ȡ����IP�б�
                proxyIPList = await GetProxyIPList();

                if (proxyIPList.Length < 1)
                {
                    MessageBox.Show("����IP��ȡʧ�ܣ������޷�����!");
                    return;
                }
                else {
                    addLog($"�Ѷ�ȡ��{proxyIPList.Length}��IP����IP.");
                }

                button2.Text = "����������...";



                ToggleUIControls(false);

                cancellationTokenSource = new CancellationTokenSource();
                var lines = File.ReadAllLines(keyPath);
                qqQueue = new ConcurrentQueue<string>(lines);

                var tasks = Enumerable.Range(0, THREAD_COUNT)
                    .Select(_ => ProcessQQs()).ToList();

                await Task.WhenAll(tasks);

                FinishTask();
            }
            else if (button2.Text == "ֹͣ")
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
            addLog($"����IP��ַ�Ѹ���Ϊ:{newip}");
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
                        lock (filePath)  // ��д���ļ�ʱȷ���̰߳�ȫ
                        {
                            File.AppendAllText(filePath, result);
                        }
                        addLog($"QQ{qq}��ȡ������,�Ѵ��档");
                    }
                }
            }
        }


        private void ToggleUIControls(bool enable)
        {
            // ���������״̬���û����UI�ؼ�
            textBox1.Enabled = enable;
            textBox2.Enabled = enable;
            textBox3.Enabled = enable;
            textBox4.Enabled = enable;
            button1.Enabled = enable;

            if (enable)
            {
                button2.Text = "��ʼ";
            }
            else
            {
                button2.Text = "ֹͣ";
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
            addLog($"�߳��ѽ���,�����߳���:{THREAD_COUNT}");

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
                                    output.AppendLine();  // ����
                                }
                            }

                            return output.ToString();
                        }
                    }
                    else
                    {
                        addLog($"[HTTPERROR] -> ���ʴ���,���Ե� {retryCount + 1} �����Բ���������...");
                        // ��������
                        var newProxyIP = await GetNextProxyIP();
                        client.DefaultRequestHeaders.ConnectionClose = true; // Close current connection before switching proxy
                        var handler = CreateHttpClientHandler(newProxyIP);
                        client = new HttpClient(handler); // ���´���client��Ӧ���µĴ���
                        retryCount++;
                    }
                }
                catch (Exception ex)
                {
                    addLog($"��������: {ex.Message}, ���Ե� {retryCount + 1} �����Բ���������...");
                    // ��������
                    var newProxyIP = await GetNextProxyIP();
                    client.DefaultRequestHeaders.ConnectionClose = true; // Close current connection before switching proxy
                    var handler = CreateHttpClientHandler(newProxyIP);
                    client = new HttpClient(handler); // ���´���client��Ӧ���µĴ���
                    retryCount++;
                }
            }

            return null;
        }


        private void FinishTask()
        {
            // ����������һЩ������
            button2.Text = "��ʼ";
            ToggleUIControls(true);
            addLog("������ִ����ϣ�");
            MessageBox.Show("������ִ����ϣ�");
        }

    }
}