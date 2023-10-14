namespace saoka_tools
{
    partial class saoka
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox2 = new GroupBox();
            textBox3 = new TextBox();
            label4 = new Label();
            textBox2 = new TextBox();
            label3 = new Label();
            textBox1 = new TextBox();
            label2 = new Label();
            label1 = new Label();
            button1 = new Button();
            button2 = new Button();
            groupBox3 = new GroupBox();
            logTextBox = new RichTextBox();
            progressBar1 = new ProgressBar();
            label5 = new Label();
            textBox4 = new TextBox();
            label6 = new Label();
            label7 = new Label();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(textBox4);
            groupBox2.Controls.Add(textBox3);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(textBox2);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(textBox1);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(button1);
            groupBox2.Controls.Add(button2);
            groupBox2.Location = new Point(12, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(1110, 129);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "扫卡设置";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(102, 60);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(280, 23);
            textBox3.TabIndex = 13;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(17, 63);
            label4.Name = "label4";
            label4.Size = new Size(59, 17);
            label4.TabIndex = 12;
            label4.Text = "过滤时间:";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(493, 25);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(440, 23);
            textBox2.TabIndex = 11;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(410, 28);
            label3.Name = "label3";
            label3.Size = new Size(83, 17);
            label3.TabIndex = 10;
            label3.Text = "代理读取地址:";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(102, 25);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(280, 23);
            textBox1.TabIndex = 9;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(17, 25);
            label2.Name = "label2";
            label2.Size = new Size(59, 17);
            label2.TabIndex = 8;
            label2.Text = "扫描网站:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 99);
            label1.Name = "label1";
            label1.Size = new Size(99, 17);
            label1.TabIndex = 7;
            label1.Text = "字典位置: 未设置";
            // 
            // button1
            // 
            button1.Location = new Point(974, 31);
            button1.Name = "button1";
            button1.Size = new Size(119, 29);
            button1.TabIndex = 6;
            button1.Text = "导入字典";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Enabled = false;
            button2.Location = new Point(974, 66);
            button2.Name = "button2";
            button2.Size = new Size(119, 29);
            button2.TabIndex = 3;
            button2.Text = "开始";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(logTextBox);
            groupBox3.Controls.Add(progressBar1);
            groupBox3.Controls.Add(label5);
            groupBox3.Location = new Point(12, 147);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(1110, 438);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "执行日志";
            // 
            // logTextBox
            // 
            logTextBox.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point);
            logTextBox.Location = new Point(9, 20);
            logTextBox.Name = "logTextBox";
            logTextBox.Size = new Size(1089, 375);
            logTextBox.TabIndex = 11;
            logTextBox.Text = "";
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(6, 424);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(1098, 8);
            progressBar1.TabIndex = 10;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(6, 402);
            label5.Name = "label5";
            label5.Size = new Size(82, 17);
            label5.TabIndex = 9;
            label5.Text = "扫描进度: 0/0";
            // 
            // textBox4
            // 
            textBox4.Location = new Point(410, 61);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(280, 23);
            textBox4.TabIndex = 14;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(389, 63);
            label6.Name = "label6";
            label6.Size = new Size(13, 17);
            label6.TabIndex = 15;
            label6.Text = "-";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(696, 63);
            label7.Name = "label7";
            label7.Size = new Size(165, 17);
            label7.TabIndex = 16;
            label7.Text = "格式: YYYY-MM-DD HH:ii:ss";
            // 
            // saoka
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1134, 597);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Name = "saoka";
            Text = "扫卡工具";
            Load += saoka_Load;
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private Button button2;
        private Button button1;
        private Label label1;
        private TextBox textBox1;
        private Label label2;
        private TextBox textBox2;
        private Label label3;
        private TextBox textBox3;
        private Label label4;
        private Label label5;
        private ProgressBar progressBar1;
        private RichTextBox logTextBox;
        private Label label7;
        private Label label6;
        private TextBox textBox4;
    }
}