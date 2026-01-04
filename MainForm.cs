using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MESFileProcessor
{
    public partial class MainForm : Form
    {
        private Label lblConfigInfo;
        private TextBox txtDataPath;
        private Button btnSelectDataPath;
        private TextBox txtOutputPath;
        private Button btnSelectOutputPath;
        private Button btnConfig;
        private Button btnStart;
        private RichTextBox rtbLog;
        private Label lblProgramInfo;
        private FileWatcher fileWatcher;
        private Config currentConfig;
        private Label lblDataPath;
        private Label lblOutputPath;
        private Label lblLogTitle;
        private bool isMonitoring = false;
        
        public MainForm()
        {
            currentConfig = ConfigManager.LoadConfig();
            InitializeComponent();
            UpdateConfigDisplay();
            
            LogManager.WriteLog("程序启动", "MES文件处理程序启动");
        }
        
        private void InitializeComponent()
        {
            this.lblConfigInfo = new System.Windows.Forms.Label();
            this.txtDataPath = new System.Windows.Forms.TextBox();
            this.btnSelectDataPath = new System.Windows.Forms.Button();
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.btnSelectOutputPath = new System.Windows.Forms.Button();
            this.btnConfig = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.lblProgramInfo = new System.Windows.Forms.Label();
            this.lblDataPath = new System.Windows.Forms.Label();
            this.lblOutputPath = new System.Windows.Forms.Label();
            this.lblLogTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblConfigInfo
            // 
            this.lblConfigInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
            this.lblConfigInfo.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.lblConfigInfo.ForeColor = System.Drawing.Color.White;
            this.lblConfigInfo.Location = new System.Drawing.Point(15, 15);
            this.lblConfigInfo.Name = "lblConfigInfo";
            this.lblConfigInfo.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lblConfigInfo.Size = new System.Drawing.Size(850, 35);
            this.lblConfigInfo.TabIndex = 0;
            this.lblConfigInfo.Text = "配置信息";
            this.lblConfigInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtDataPath
            // 
            this.txtDataPath.BackColor = System.Drawing.Color.White;
            this.txtDataPath.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.txtDataPath.Location = new System.Drawing.Point(130, 70);
            this.txtDataPath.Name = "txtDataPath";
            this.txtDataPath.ReadOnly = true;
            this.txtDataPath.Size = new System.Drawing.Size(600, 31);
            this.txtDataPath.TabIndex = 2;
            // 
            // btnSelectDataPath
            // 
            this.btnSelectDataPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(149)))), ((int)(((byte)(237)))));
            this.btnSelectDataPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectDataPath.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnSelectDataPath.ForeColor = System.Drawing.Color.White;
            this.btnSelectDataPath.Location = new System.Drawing.Point(740, 68);
            this.btnSelectDataPath.Name = "btnSelectDataPath";
            this.btnSelectDataPath.Size = new System.Drawing.Size(120, 30);
            this.btnSelectDataPath.TabIndex = 3;
            this.btnSelectDataPath.Text = "浏览...";
            this.btnSelectDataPath.UseVisualStyleBackColor = false;
            this.btnSelectDataPath.Click += new System.EventHandler(this.btnSelectDataPath_Click);
            // 
            // txtOutputPath
            // 
            this.txtOutputPath.BackColor = System.Drawing.Color.White;
            this.txtOutputPath.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.txtOutputPath.Location = new System.Drawing.Point(130, 115);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.ReadOnly = true;
            this.txtOutputPath.Size = new System.Drawing.Size(600, 31);
            this.txtOutputPath.TabIndex = 5;
            // 
            // btnSelectOutputPath
            // 
            this.btnSelectOutputPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(149)))), ((int)(((byte)(237)))));
            this.btnSelectOutputPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectOutputPath.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnSelectOutputPath.ForeColor = System.Drawing.Color.White;
            this.btnSelectOutputPath.Location = new System.Drawing.Point(740, 113);
            this.btnSelectOutputPath.Name = "btnSelectOutputPath";
            this.btnSelectOutputPath.Size = new System.Drawing.Size(120, 30);
            this.btnSelectOutputPath.TabIndex = 6;
            this.btnSelectOutputPath.Text = "浏览...";
            this.btnSelectOutputPath.UseVisualStyleBackColor = false;
            this.btnSelectOutputPath.Click += new System.EventHandler(this.btnSelectOutputPath_Click);
            // 
            // btnConfig
            // 
            this.btnConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(165)))), ((int)(((byte)(0)))));
            this.btnConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfig.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.btnConfig.ForeColor = System.Drawing.Color.White;
            this.btnConfig.Location = new System.Drawing.Point(620, 160);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(110, 35);
            this.btnConfig.TabIndex = 8;
            this.btnConfig.Text = "程序配置";
            this.btnConfig.UseVisualStyleBackColor = false;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(139)))), ((int)(((byte)(34)))));
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Location = new System.Drawing.Point(750, 160);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(110, 35);
            this.btnStart.TabIndex = 9;
            this.btnStart.Text = "启动监控";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.rtbLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbLog.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.rtbLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(127)))));
            this.rtbLog.Location = new System.Drawing.Point(15, 240);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbLog.Size = new System.Drawing.Size(850, 330);
            this.rtbLog.TabIndex = 10;
            this.rtbLog.Text = "";
            // 
            // lblProgramInfo
            // 
            this.lblProgramInfo.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblProgramInfo.ForeColor = System.Drawing.Color.Black;
            this.lblProgramInfo.Location = new System.Drawing.Point(15, 580);
            this.lblProgramInfo.Name = "lblProgramInfo";
            this.lblProgramInfo.Size = new System.Drawing.Size(850, 42);
            this.lblProgramInfo.TabIndex = 11;
            this.lblProgramInfo.Text = "UF-AOI数据上传程序 V1.0";
            this.lblProgramInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDataPath
            // 
            this.lblDataPath.Font = new System.Drawing.Font("微软雅黑", 9.5F);
            this.lblDataPath.Location = new System.Drawing.Point(15, 70);
            this.lblDataPath.Name = "lblDataPath";
            this.lblDataPath.Size = new System.Drawing.Size(110, 25);
            this.lblDataPath.TabIndex = 1;
            this.lblDataPath.Text = "数据文件路径:";
            this.lblDataPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblOutputPath
            // 
            this.lblOutputPath.Font = new System.Drawing.Font("微软雅黑", 9.5F);
            this.lblOutputPath.Location = new System.Drawing.Point(15, 115);
            this.lblOutputPath.Name = "lblOutputPath";
            this.lblOutputPath.Size = new System.Drawing.Size(110, 25);
            this.lblOutputPath.TabIndex = 4;
            this.lblOutputPath.Text = "解析后文件路径:";
            this.lblOutputPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLogTitle
            // 
            this.lblLogTitle.Font = new System.Drawing.Font("微软雅黑", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblLogTitle.Location = new System.Drawing.Point(15, 210);
            this.lblLogTitle.Name = "lblLogTitle";
            this.lblLogTitle.Size = new System.Drawing.Size(100, 25);
            this.lblLogTitle.TabIndex = 7;
            this.lblLogTitle.Text = "运行日志:";
            // 
            // MainForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(891, 647);
            this.Controls.Add(this.lblConfigInfo);
            this.Controls.Add(this.lblDataPath);
            this.Controls.Add(this.txtDataPath);
            this.Controls.Add(this.btnSelectDataPath);
            this.Controls.Add(this.lblOutputPath);
            this.Controls.Add(this.txtOutputPath);
            this.Controls.Add(this.btnSelectOutputPath);
            this.Controls.Add(this.lblLogTitle);
            this.Controls.Add(this.btnConfig);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.rtbLog);
            this.Controls.Add(this.lblProgramInfo);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.MinimumSize = new System.Drawing.Size(900, 650);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UF-AOI数据上传程序";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        
        private void UpdateConfigDisplay()
        {
            lblConfigInfo.Text = ConfigManager.GetConfigDisplayString(currentConfig);
        }
        
        private void AddLog(string message)
        {
            if (rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(new Action<string>(AddLog), message);
                return;
            }
            
            string logEntry = string.Format("[{0:HH:mm:ss}] {1}", DateTime.Now, message);
            rtbLog.AppendText(logEntry + Environment.NewLine);
            rtbLog.ScrollToCaret();
            
            // 限制日志行数为500行，防止数据量过大导致程序崩溃
            if (rtbLog.Lines.Length > 500)
            {
                var lines = rtbLog.Lines.Skip(100).ToArray(); // 删除前100行，保留最新400行
                rtbLog.Lines = lines;
            }
        }
        
        private void btnSelectDataPath_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "选择数据文件路径";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtDataPath.Text = dialog.SelectedPath;
                    AddLog(string.Format("选择数据文件路径: {0}", dialog.SelectedPath));
                    LogManager.WriteLog("选择数据文件路径", dialog.SelectedPath);
                }
            }
        }
        
        private void btnSelectOutputPath_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "选择解析后文件存放路径";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtOutputPath.Text = dialog.SelectedPath;
                    AddLog(string.Format("选择输出路径: {0}", dialog.SelectedPath));
                    LogManager.WriteLog("选择输出路径", dialog.SelectedPath);
                    
                    // 确保目标文件夹下有必要的子文件夹
                    EnsureOutputDirectories(dialog.SelectedPath);
                }
            }
        }
        
        private void EnsureOutputDirectories(string outputPath)
        {
            try
            {
                string[] subDirs = { "fail", "pass", "error" };
                foreach (string subDir in subDirs)
                {
                    string path = System.IO.Path.Combine(outputPath, subDir);
                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                        AddLog(string.Format("创建文件夹: {0}", path));
                        LogManager.WriteLog("创建文件夹", path);
                    }
                }
            }
            catch (Exception ex)
            {
                AddLog(string.Format("创建子文件夹失败: {0}", ex.Message));
                LogManager.WriteErrorLog("创建子文件夹失败", ex);
            }
        }
        
        private void btnConfig_Click(object sender, EventArgs e)
        {
            using (var configForm = new ConfigForm(currentConfig))
            {
                if (configForm.ShowDialog() == DialogResult.OK)
                {
                    currentConfig = configForm.CurrentConfig;
                    UpdateConfigDisplay();
                    AddLog("配置已更新");
                }
            }
        }
        
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!isMonitoring)
            {
                StartMonitoring();
            }
            else
            {
                StopMonitoring();
            }
        }
        
        private void StartMonitoring()
        {
            try
            {
                // 验证必要的配置
                if (string.IsNullOrWhiteSpace(txtDataPath.Text))
                {
                    MessageBox.Show("请先选择数据文件路径", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(txtOutputPath.Text))
                {
                    MessageBox.Show("请先选择解析后文件存放路径", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(currentConfig.MesUrl))
                {
                    MessageBox.Show("请先配置MES网址", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(currentConfig.StationName))
                {
                    MessageBox.Show("请先配置工站名称", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(currentConfig.LineName))
                {
                    MessageBox.Show("请先配置线体名称", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // 启动文件监控
                fileWatcher = new FileWatcher(txtDataPath.Text, txtOutputPath.Text, currentConfig, AddLog);
                fileWatcher.Start();
                
                isMonitoring = true;
                btnStart.Text = "停止监控";
                btnStart.BackColor = Color.FromArgb(220, 20, 60); // 深红色
                
                AddLog("文件监控已启动");
            }
            catch (Exception ex)
            {
                AddLog(string.Format("启动监控失败: {0}", ex.Message));
                LogManager.WriteErrorLog("启动监控失败", ex);
                MessageBox.Show(string.Format("启动监控失败: {0}", ex.Message), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void StopMonitoring()
        {
            try
            {
                if (fileWatcher != null)
                {
                    fileWatcher.Stop();
                    fileWatcher = null;
                }
                
                isMonitoring = false;
                btnStart.Text = "启动监控";
                btnStart.BackColor = Color.FromArgb(34, 139, 34); // 绿色
                
                AddLog("文件监控已停止");
            }
            catch (Exception ex)
            {
                AddLog(string.Format("停止监控失败: {0}", ex.Message));
                LogManager.WriteErrorLog("停止监控失败", ex);
            }
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 验证关闭口令
            if (!ValidateClosePassword())
            {
                e.Cancel = true; // 取消关闭
                return;
            }
            
            if (isMonitoring)
            {
                StopMonitoring();
            }
            
            LogManager.WriteLog("程序关闭", "MES文件处理程序关闭");
            base.OnFormClosing(e);
        }
        
        private bool ValidateClosePassword()
        {
            const string correctPassword = "12345678";
            
            // 创建密码输入对话框
            Form passwordForm = new Form();
            passwordForm.Text = "关闭程序验证";
            passwordForm.Size = new Size(350, 150);
            passwordForm.StartPosition = FormStartPosition.CenterParent;
            passwordForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            passwordForm.MaximizeBox = false;
            passwordForm.MinimizeBox = false;
            passwordForm.Font = new Font("微软雅黑", 9F);
            
            Label lblPrompt = new Label();
            lblPrompt.Text = "请输入关闭口令:";
            lblPrompt.Location = new Point(20, 20);
            lblPrompt.Size = new Size(300, 25);
            lblPrompt.Font = new Font("微软雅黑", 10F);
            
            TextBox txtPassword = new TextBox();
            txtPassword.Location = new Point(20, 50);
            txtPassword.Size = new Size(290, 25);
            txtPassword.PasswordChar = '●';
            txtPassword.Font = new Font("微软雅黑", 10F);
            
            Button btnOk = new Button();
            btnOk.Text = "确定";
            btnOk.Location = new Point(150, 85);
            btnOk.Size = new Size(75, 30);
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Font = new Font("微软雅黑", 9F);
            
            Button btnCancel = new Button();
            btnCancel.Text = "取消";
            btnCancel.Location = new Point(235, 85);
            btnCancel.Size = new Size(75, 30);
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Font = new Font("微软雅黑", 9F);
            
            passwordForm.Controls.Add(lblPrompt);
            passwordForm.Controls.Add(txtPassword);
            passwordForm.Controls.Add(btnOk);
            passwordForm.Controls.Add(btnCancel);
            
            passwordForm.AcceptButton = btnOk;
            passwordForm.CancelButton = btnCancel;
            
            // 显示对话框
            DialogResult result = passwordForm.ShowDialog(this);
            
            if (result == DialogResult.OK)
            {
                if (txtPassword.Text == correctPassword)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("口令错误，无法关闭程序！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            else
            {
                return false; // 用户点击取消
            }
        }
    }
}
