using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MESFileProcessor
{
    public partial class ConfigForm : Form
    {
        private TextBox txtStationName;
        private TextBox txtWorkStation;
        private TextBox txtOperatorId;
        private TextBox txtComponentNames;
        private Button btnSave;
        private Button btnCancel;
        private Label lblStationName;
        private Label lblWorkStation;
        private Label lblOperatorId;
        private Label lblComponentNames;
        private Label lblComponentExample;

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
                NcCode = config.NcCode,
                ComponentNames = config.ComponentNames,
                WorkStation = config.WorkStation,
                OperatorId = config.OperatorId
            };

            InitializeComponent();
            LoadConfig();
        }

        private void InitializeComponent()
        {
            txtStationName = new TextBox();
            txtWorkStation = new TextBox();
            txtOperatorId = new TextBox();
            txtComponentNames = new TextBox();
            btnSave = new Button();
            btnCancel = new Button();
            lblStationName = new Label();
            lblWorkStation = new Label();
            lblOperatorId = new Label();
            lblComponentNames = new Label();
            lblComponentExample = new Label();

            SuspendLayout();

            Text = "程序配置";
            Size = new Size(480, 380);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(240, 240, 240);
            Font = new Font("微软雅黑", 9F, FontStyle.Regular);

            ConfigureLabel(lblStationName, "工站名称:", 30);
            ConfigureTextBox(txtStationName, 30);
            ConfigureLabel(lblWorkStation, "工位:", 85);
            ConfigureTextBox(txtWorkStation, 85);
            ConfigureLabel(lblOperatorId, "工号:", 140);
            ConfigureTextBox(txtOperatorId, 140);
            txtOperatorId.ReadOnly = true;
            ConfigureLabel(lblComponentNames, "组件信息:", 195);
            ConfigureTextBox(txtComponentNames, 195);
            lblComponentExample.Text = "示例：PCB1_BC;U56_BC（按XML标签原名填写）";
            lblComponentExample.Location = new Point(140, 225);
            lblComponentExample.Size = new Size(300, 28);
            lblComponentExample.ForeColor = Color.DimGray;

            btnSave.Text = "保存";
            btnSave.Location = new Point(250, 270);
            btnSave.Size = new Size(85, 35);
            btnSave.UseVisualStyleBackColor = true;
            btnSave.BackColor = Color.FromArgb(34, 139, 34);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("微软雅黑", 10F, FontStyle.Bold);
            btnSave.Click += new EventHandler(btnSave_Click);

            btnCancel.Text = "取消";
            btnCancel.Location = new Point(345, 270);
            btnCancel.Size = new Size(85, 35);
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.BackColor = Color.FromArgb(220, 20, 60);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("微软雅黑", 10F, FontStyle.Bold);
            btnCancel.Click += new EventHandler(btnCancel_Click);

            Controls.Add(lblStationName);
            Controls.Add(txtStationName);
            Controls.Add(lblWorkStation);
            Controls.Add(txtWorkStation);
            Controls.Add(lblOperatorId);
            Controls.Add(txtOperatorId);
            Controls.Add(lblComponentNames);
            Controls.Add(txtComponentNames);
            Controls.Add(lblComponentExample);
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
            txtWorkStation.Text = CurrentConfig.WorkStation;
            txtOperatorId.Text = CurrentConfig.OperatorId;
            txtComponentNames.Text = CurrentConfig.ComponentNames;
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

                if (string.IsNullOrWhiteSpace(txtWorkStation.Text))
                {
                    MessageBox.Show("请输入工位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtWorkStation.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtComponentNames.Text) ||
                    txtComponentNames.Text.Split(';').Any(name => string.IsNullOrWhiteSpace(name) ||
                        !name.Trim().EndsWith("_BC", StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("请输入以_BC结尾的组件信息名称，多个名称用英文分号分隔", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtComponentNames.Focus();
                    return;
                }

                CurrentConfig.StationName = txtStationName.Text.Trim();
                CurrentConfig.WorkStation = txtWorkStation.Text.Trim();
                CurrentConfig.ComponentNames = txtComponentNames.Text.Trim();
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
