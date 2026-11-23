using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MESFileProcessor
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // 设置全局异常处理
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            try
            {
                string operatorId = PromptOperatorId();
                if (operatorId == null) return;
                Config config = ConfigManager.LoadConfig();
                config.OperatorId = operatorId;
                ConfigManager.SaveConfig(config);
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                LogManager.WriteErrorLog("程序启动异常", ex);
                MessageBox.Show(string.Format("程序启动失败: {0}", ex.Message), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string PromptOperatorId()
        {
            using (var dialog = new Form())
            using (var label = new Label())
            using (var input = new TextBox())
            using (var confirm = new Button())
            {
                dialog.Text = "成都汽车电子AOI上传软件V1.0 - 工号登录";
                dialog.ClientSize = new Size(360, 145);
                dialog.StartPosition = FormStartPosition.CenterScreen;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                label.Text = "请输入6至8位数字工号：";
                label.SetBounds(20, 20, 310, 25);
                input.SetBounds(20, 52, 310, 26);
                input.MaxLength = 8;
                confirm.Text = "登录";
                confirm.SetBounds(245, 95, 85, 30);
                confirm.Click += (sender, args) =>
                {
                    if (!Regex.IsMatch(input.Text.Trim(), @"^[0-9]{6,8}$"))
                    {
                        MessageBox.Show("工号必须是6至8位数字", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        input.Focus();
                        return;
                    }
                    dialog.DialogResult = DialogResult.OK;
                };
                dialog.Controls.AddRange(new Control[] { label, input, confirm });
                dialog.AcceptButton = confirm;
                return dialog.ShowDialog() == DialogResult.OK ? input.Text.Trim() : null;
            }
        }
        
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            LogManager.WriteErrorLog("未处理的线程异常", e.Exception);
            MessageBox.Show(string.Format("程序发生异常: {0}", e.Exception.Message), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogManager.WriteErrorLog("未处理的应用程序域异常", ex);
            }
            else
            {
                LogManager.WriteLog("未处理的应用程序域异常", e.ExceptionObject != null ? e.ExceptionObject.ToString() : "Unknown exception");
            }
        }
    }
}
