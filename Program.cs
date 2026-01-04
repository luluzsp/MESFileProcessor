using System;
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
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                LogManager.WriteErrorLog("程序启动异常", ex);
                MessageBox.Show(string.Format("程序启动失败: {0}", ex.Message), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
