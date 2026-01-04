using System;
using System.IO;
using System.Windows.Forms;

namespace MESFileProcessor
{
    public class LogManager
    {
        private static readonly string LogDirectory = Path.Combine(Application.StartupPath, "Logs");
        
        static LogManager()
        {
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }
        }
        
        public static void WriteLog(string message)
        {
            try
            {
                string logFileName = string.Format("{0:yyyy-MM-dd}.log", DateTime.Now);
                string logPath = Path.Combine(LogDirectory, logFileName);
                
                string logEntry = string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}", DateTime.Now, message);
                
                File.AppendAllText(logPath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // 避免日志写入错误导致程序崩溃
                Console.WriteLine(string.Format("写入日志失败: {0}", ex.Message));
            }
        }
        
        public static void WriteLog(string action, string details)
        {
            WriteLog(string.Format("{0}: {1}", action, details));
        }
        
        public static void WriteErrorLog(string action, Exception ex)
        {
            WriteLog(string.Format("错误 - {0}: {1}", action, ex.Message));
        }
        
        public static void WriteApiLog(string url, string parameters, string response)
        {
            WriteLog(string.Format("API调用 - URL: {0}, 参数: {1}, 响应: {2}", url, parameters, response));
        }
    }
}
