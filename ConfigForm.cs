using System;
using System.Drawing;
using System.Windows.Forms;

namespace MESFileProcessor
{
    public partial class ConfigForm : Form
    {
        private TextBox txtMesUrl;
        private TextBox txtStationName;
        private TextBox txtLineName;
        private Button btnSave;
        private Button btnCancel;
        private Label lblMesUrl;
        private Label lblStationName;
        private Label lblLineName;
        
        public Config CurrentConfig { get; private set; }
        
        public ConfigForm(Config config)
        {
            CurrentConfig = new Config
            {
                MesUrl = config.MesUrl,
                StationName = config.StationName,
                LineName = config.LineName
            };
            
            InitializeComponent();
            LoadConfig();
        }
        
        private void InitializeComponent()
        {
            this.txtMesUrl = new TextBox();
            this.txtStationName = new TextBox();
            this.txtLineName = new TextBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.lblMesUrl = new Label();
            this.lblStationName = new Label();
            this.lblLineName = new Label();
            
            this.SuspendLayout();
            
            // Form - 美化
            this.Text = "程序配置";
            this.Size = new Size(450, 280);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.Font = new Font("微软雅黑", 9F, FontStyle.Regular);
            
            // lblMesUrl
            this.lblMesUrl.Text = "MES网址:";
            this.lblMesUrl.Location = new Point(30, 30);
            this.lblMesUrl.Size = new Size(90, 25);
            this.lblMesUrl.TextAlign = ContentAlignment.MiddleLeft;
            this.lblMesUrl.Font = new Font("微软雅黑", 10F, FontStyle.Regular);
            
            // txtMesUrl
            this.txtMesUrl.Location = new Point(130, 30);
            this.txtMesUrl.Size = new Size(280, 25);
            this.txtMesUrl.Font = new Font("微软雅黑", 9.5F);
            
            // lblStationName
            this.lblStationName.Text = "工站名称:";
            this.lblStationName.Location = new Point(30, 80);
            this.lblStationName.Size = new Size(90, 25);
            this.lblStationName.TextAlign = ContentAlignment.MiddleLeft;
            this.lblStationName.Font = new Font("微软雅黑", 10F, FontStyle.Regular);
            
            // txtStationName
            this.txtStationName.Location = new Point(130, 80);
            this.txtStationName.Size = new Size(280, 25);
            this.txtStationName.Font = new Font("微软雅黑", 9.5F);
            
            // lblLineName
            this.lblLineName.Text = "线体名称:";
            this.lblLineName.Location = new Point(30, 130);
            this.lblLineName.Size = new Size(90, 25);
            this.lblLineName.TextAlign = ContentAlignment.MiddleLeft;
            this.lblLineName.Font = new Font("微软雅黑", 10F, FontStyle.Regular);
            
            // txtLineName
            this.txtLineName.Location = new Point(130, 130);
            this.txtLineName.Size = new Size(280, 25);
            this.txtLineName.Font = new Font("微软雅黑", 9.5F);
            
            // btnSave - 美化
            this.btnSave.Text = "保存";
            this.btnSave.Location = new Point(230, 190);
            this.btnSave.Size = new Size(85, 35);
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.BackColor = Color.FromArgb(34, 139, 34);
            this.btnSave.ForeColor = Color.White;
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.Font = new Font("微软雅黑", 10F, FontStyle.Bold);
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            
            // btnCancel - 美化
            this.btnCancel.Text = "取消";
            this.btnCancel.Location = new Point(325, 190);
            this.btnCancel.Size = new Size(85, 35);
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.BackColor = Color.FromArgb(220, 20, 60);
            this.btnCancel.ForeColor = Color.White;
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.Font = new Font("微软雅黑", 10F, FontStyle.Bold);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            
            // 添加控件到窗体
            this.Controls.Add(this.lblMesUrl);
            this.Controls.Add(this.txtMesUrl);
            this.Controls.Add(this.lblStationName);
            this.Controls.Add(this.txtStationName);
            this.Controls.Add(this.lblLineName);
            this.Controls.Add(this.txtLineName);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            
            this.ResumeLayout(false);
        }
        
        private void LoadConfig()
        {
            txtMesUrl.Text = CurrentConfig.MesUrl;
            txtStationName.Text = CurrentConfig.StationName;
            txtLineName.Text = CurrentConfig.LineName;
        }
        
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // 验证输入
                if (string.IsNullOrWhiteSpace(txtMesUrl.Text))
                {
                    MessageBox.Show("请输入MES网址", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMesUrl.Focus();
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(txtStationName.Text))
                {
                    MessageBox.Show("请输入工站名称", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtStationName.Focus();
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(txtLineName.Text))
                {
                    MessageBox.Show("请输入线体名称", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLineName.Focus();
                    return;
                }
                
                // 保存配置
                CurrentConfig.MesUrl = txtMesUrl.Text.Trim();
                CurrentConfig.StationName = txtStationName.Text.Trim();
                CurrentConfig.LineName = txtLineName.Text.Trim();
                
                ConfigManager.SaveConfig(CurrentConfig);
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("保存配置失败: {0}", ex.Message), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
