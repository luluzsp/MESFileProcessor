using System;
using System.Drawing;
using System.Windows.Forms;

namespace MESFileProcessor
{
    public partial class ConfigForm : Form
    {
        private TextBox txtStationName;
        private TextBox txtStationId;
        private TextBox txtNcCode;
        private Button btnSave;
        private Button btnCancel;
        private Label lblStationName;
        private Label lblStationId;
        private Label lblNcCode;

        public Config CurrentConfig { get; private set; }

        public ConfigForm(Config config)
        {
            CurrentConfig = new Config
            {
                // 保留旧字段，保存新配置时不丢失已有数据。
                MesUrl = config.MesUrl,
                LineName = config.LineName,
                StationName = config.StationName,
                StationId = config.StationId,
                NcCode = config.NcCode
            };

            InitializeComponent();
            LoadConfig();
        }

        private void InitializeComponent()
        {
            txtStationName = new TextBox();
            txtStationId = new TextBox();
            txtNcCode = new TextBox();
            btnSave = new Button();
            btnCancel = new Button();
            lblStationName = new Label();
            lblStationId = new Label();
            lblNcCode = new Label();

            SuspendLayout();

            Text = "程序配置";
            Size = new Size(480, 330);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(240, 240, 240);
            Font = new Font("微软雅黑", 9F, FontStyle.Regular);

            ConfigureLabel(lblStationName, "工站名称:", 30);
            ConfigureTextBox(txtStationName, 30);
            ConfigureLabel(lblStationId, "工站ID:", 85);
            ConfigureTextBox(txtStationId, 85);
            ConfigureLabel(lblNcCode, "不良代码:", 140);
            ConfigureTextBox(txtNcCode, 140);

            btnSave.Text = "保存";
            btnSave.Location = new Point(250, 215);
            btnSave.Size = new Size(85, 35);
            btnSave.UseVisualStyleBackColor = true;
            btnSave.BackColor = Color.FromArgb(34, 139, 34);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("微软雅黑", 10F, FontStyle.Bold);
            btnSave.Click += new EventHandler(btnSave_Click);

            btnCancel.Text = "取消";
            btnCancel.Location = new Point(345, 215);
            btnCancel.Size = new Size(85, 35);
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.BackColor = Color.FromArgb(220, 20, 60);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("微软雅黑", 10F, FontStyle.Bold);
            btnCancel.Click += new EventHandler(btnCancel_Click);

            Controls.Add(lblStationName);
            Controls.Add(txtStationName);
            Controls.Add(lblStationId);
            Controls.Add(txtStationId);
            Controls.Add(lblNcCode);
            Controls.Add(txtNcCode);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);

            ResumeLayout(false);
            PerformLayout();
        }

        private static void ConfigureLabel(Label label, string text, int top)
        {
            label.Text = text;
            label.Location = new Point(30, top);
            label.Size = new Size(100, 25);
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Font = new Font("微软雅黑", 10F, FontStyle.Regular);
        }

        private static void ConfigureTextBox(TextBox textBox, int top)
        {
            textBox.Location = new Point(140, top);
            textBox.Size = new Size(290, 25);
            textBox.Font = new Font("微软雅黑", 9.5F);
        }

        private void LoadConfig()
        {
            txtStationName.Text = CurrentConfig.StationName;
            txtStationId.Text = CurrentConfig.StationId;
            txtNcCode.Text = CurrentConfig.NcCode;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtStationName.Text))
                {
                    MessageBox.Show("请输入工站名称", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtStationName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtStationId.Text))
                {
                    MessageBox.Show("请输入工站ID", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtStationId.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNcCode.Text))
                {
                    MessageBox.Show("请输入不良代码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNcCode.Focus();
                    return;
                }

                CurrentConfig.StationName = txtStationName.Text.Trim();
                CurrentConfig.StationId = txtStationId.Text.Trim();
                CurrentConfig.NcCode = txtNcCode.Text.Trim();
                ConfigManager.SaveConfig(CurrentConfig);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("保存配置失败: {0}", ex.Message), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
