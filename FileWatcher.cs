using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MESFileProcessor
{
    public class FileWatcher
    {
        private FileSystemWatcher watcher;
        private readonly string watchPath;
        private readonly string outputPath;
        private readonly Config config;
        private readonly Action<string> logCallback;
        
        public FileWatcher(string watchPath, string outputPath, Config config, Action<string> logCallback)
        {
            this.watchPath = watchPath;
            this.outputPath = outputPath;
            this.config = config;
            this.logCallback = logCallback;
        }
        
        public void Start()
        {
            if (string.IsNullOrEmpty(watchPath) || !Directory.Exists(watchPath))
            {
                logCallback("监控路径无效或不存在");
                return;
            }
            
            if (string.IsNullOrEmpty(outputPath) || !Directory.Exists(outputPath))
            {
                logCallback("输出路径无效或不存在");
                return;
            }
            
            // 确保输出目录下有必要的子文件夹
            EnsureDirectories();
            
            // 处理已存在的文件
            ProcessExistingFiles();
            
            watcher = new FileSystemWatcher(watchPath);
            watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName;
            watcher.EnableRaisingEvents = true;
            
            watcher.Created += OnFileCreated;
            watcher.Changed += OnFileChanged;
            
            logCallback(string.Format("开始监控文件夹: {0}", watchPath));
            LogManager.WriteLog("文件监控启动", watchPath);
        }
        
        private void ProcessExistingFiles()
        {
            try
            {
                string[] files = Directory.GetFiles(watchPath);
                if (files.Length > 0)
                {
                    logCallback(string.Format("发现 {0} 个已存在的文件，开始处理...", files.Length));
                    foreach (string file in files)
                    {
                        // 异步处理每个文件
                        Task.Run(async () => await ProcessFile(file));
                    }
                }
            }
            catch (Exception ex)
            {
                logCallback(string.Format("扫描已存在文件失败: {0}", ex.Message));
                LogManager.WriteErrorLog("扫描已存在文件失败", ex);
            }
        }
        
        public void Stop()
        {
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
                watcher = null;
                
                logCallback("文件监控已停止");
                LogManager.WriteLog("文件监控停止", "");
            }
        }
        
        private void EnsureDirectories()
        {
            string[] subDirs = { "fail", "pass", "error" };
            foreach (string subDir in subDirs)
            {
                string path = Path.Combine(outputPath, subDir);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    LogManager.WriteLog("创建目录", path);
                }
            }
        }
        
        private async void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            await ProcessFile(e.FullPath);
        }
        
        private async void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            // 延迟处理，避免文件正在写入时处理
            await Task.Delay(1000);
            await ProcessFile(e.FullPath);
        }
        
        private async Task ProcessFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return;
                
                string fileName = Path.GetFileName(filePath);
                string extension = Path.GetExtension(filePath).ToLower();
                
                logCallback(string.Format("检测到文件: {0}", fileName));
                LogManager.WriteLog("检测到文件", filePath);
                
                // 检查文件扩展名
                if (extension != ".tar")
                {
                    // 检查源文件是否存在
                    if (!File.Exists(filePath))
                    {
                        logCallback(string.Format("文件已被处理或不存在，跳过移动: {0}", fileName));
                        return;
                    }
                    
                    string errorPath = GetUniqueFilePath(outputPath, "error", fileName);
                    File.Move(filePath, errorPath);
                    
                    string message = string.Format("文件 {0} 移动到 {1}", fileName, errorPath);
                    logCallback(message);
                    LogManager.WriteLog("文件移动到error", message);
                    return;
                }
                
                // 处理.tar文件
                await ProcessTarFile(filePath);
            }
            catch (Exception ex)
            {
                logCallback(string.Format("处理文件时出错: {0}", ex.Message));
                LogManager.WriteErrorLog("处理文件异常", ex);
            }
        }
        
        private async Task ProcessTarFile(string filePath)
        {
            try
            {
                string fileName = Path.GetFileName(filePath);
                
                // 解析tar文件内容
                string sfc = null;
                string result = null;
                
                // 简单的文本文件读取方式（假设tar文件实际上是文本文件或可以直接读取）
                try
                {
                    string[] lines = ReadFileWithEncoding(filePath);
                    
                    // 获取第一行内容作为SFC（从第二位开始）
                    if (lines.Length > 0)
                    {
                        string firstLine = lines[0].Trim();
                        if (firstLine.Length > 1)
                        {
                            sfc = firstLine.Substring(1).Trim();
                        }
                    }
                    
                    // 获取第10行内容作为result
                    if (lines.Length >= 10)
                    {
                        result = lines[9].Trim();
                    }
                }
                catch
                {
                    // 如果直接读取失败，尝试其他方法或记录错误
                    MoveToFailFolder(filePath, "无法解析tar文件内容");
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(sfc))
                {
                    MoveToFailFolder(filePath, "无法获取有效的SFC");
                    return;
                }
                
                logCallback(string.Format("解析文件 {0}: SFC={1}, Result={2}", fileName, sfc, result));
                LogManager.WriteLog("文件解析", string.Format("文件={0}, SFC={1}, Result={2}", fileName, sfc, result));
                
                // 调用MES接口获取序列化数据
                logCallback(string.Format("【API调用】准备调用GetSerializeData接口，SFC={0}", sfc));
                var serializeResponse = await MESService.GetSerializeData(config.MesUrl, sfc, logCallback);
                
                if (serializeResponse == null || serializeResponse.LOAD_ID == null)
                {
                    MoveToFailFolder(filePath, string.Format("SFC {0} 在MES系统中不存在", sfc));
                    return;
                }
                
                // 处理返回的数据
                if (serializeResponse.OLDSFC_DATA != null && serializeResponse.OLDSFC_DATA.Count > 0)
                {
                    bool allSuccess = true;
                    
                    foreach (var oldSfcData in serializeResponse.OLDSFC_DATA)
                    {
                        string dataValue = result == "TP" ? "PASS" : result == "TF" ? "FAIL" : "UNKNOWN";
                        long unixTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                        string dataName = string.Format("{0}{1}", config.StationName, unixTime);
                        
                        logCallback(string.Format("【API调用】准备调用addSfcKey接口，SFC={0}, DataValue={1}", oldSfcData.newSfc, dataValue));
                        
                        var addSfcResponse = await MESService.AddSfcKey(
                            config.MesUrl,
                            oldSfcData.newSfc,
                            config.StationName,
                            oldSfcData.shoporder,
                            config.LineName,
                            dataName,
                            dataValue,
                            logCallback
                        );
                        
                        if (addSfcResponse == null || addSfcResponse.RESULT != "PASS")
                        {
                            allSuccess = false;
                            break;
                        }
                    }
                    
                    if (allSuccess)
                    {
                        MoveToPassFolder(filePath, "所有接口调用成功");
                    }
                    else
                    {
                        MoveToFailFolder(filePath, "部分接口调用失败");
                    }
                }
                else
                {
                    MoveToFailFolder(filePath, "未获取到有效的OLDSFC_DATA");
                }
            }
            catch (Exception ex)
            {
                MoveToFailFolder(filePath, string.Format("处理异常: {0}", ex.Message));
                LogManager.WriteErrorLog("处理tar文件异常", ex);
            }
        }
        
        private void MoveToFailFolder(string filePath, string reason)
        {
            try
            {
                // 检查源文件是否存在
                if (!File.Exists(filePath))
                {
                    logCallback(string.Format("文件已被处理或不存在，跳过移动: {0}", filePath));
                    return;
                }
                
                string fileName = Path.GetFileName(filePath);
                string failPath = GetUniqueFilePath(outputPath, "fail", fileName);
                
                File.Move(filePath, failPath);
                
                string message = string.Format("文件 {0} 移动到fail文件夹: {1}", fileName, reason);
                logCallback(message);
                LogManager.WriteLog("文件移动到fail", message);
            }
            catch (Exception ex)
            {
                // 只在非"文件未找到"错误时记录
                if (!(ex is System.IO.FileNotFoundException))
                {
                    logCallback(string.Format("移动文件到fail文件夹时出错: {0}", ex.Message));
                    LogManager.WriteErrorLog("移动文件到fail文件夹失败", ex);
                }
            }
        }
        
        private void MoveToPassFolder(string filePath, string reason)
        {
            try
            {
                // 检查源文件是否存在
                if (!File.Exists(filePath))
                {
                    logCallback(string.Format("文件已被处理或不存在，跳过移动: {0}", filePath));
                    return;
                }
                
                string fileName = Path.GetFileName(filePath);
                string passPath = GetUniqueFilePath(outputPath, "pass", fileName);
                
                File.Move(filePath, passPath);
                
                string message = string.Format("文件 {0} 移动到pass文件夹: {1}", fileName, reason);
                logCallback(message);
                LogManager.WriteLog("文件移动到pass", message);
            }
            catch (Exception ex)
            {
                // 只在非"文件未找到"错误时记录
                if (!(ex is System.IO.FileNotFoundException))
                {
                    logCallback(string.Format("移动文件到pass文件夹时出错: {0}", ex.Message));
                    LogManager.WriteErrorLog("移动文件到pass文件夹失败", ex);
                }
            }
        }
        
        /// <summary>
        /// 获取唯一的文件路径，如果文件已存在则添加时间戳
        /// </summary>
        private string GetUniqueFilePath(string basePath, string subFolder, string fileName)
        {
            string targetPath = Path.Combine(basePath, subFolder, fileName);
            
            // 如果文件不存在，直接返回
            if (!File.Exists(targetPath))
            {
                return targetPath;
            }
            
            // 文件已存在，添加时间戳
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            string newFileName = string.Format("{0}_{1}{2}", fileNameWithoutExt, timestamp, extension);
            
            return Path.Combine(basePath, subFolder, newFileName);
        }
        
        /// <summary>
        /// 使用多种编码尝试读取文件，支持ANSI和UTF-8
        /// </summary>
        private string[] ReadFileWithEncoding(string filePath)
        {
            // 尝试的编码顺序：UTF-8, GB2312(ANSI), Default
            System.Text.Encoding[] encodings = new System.Text.Encoding[]
            {
                System.Text.Encoding.UTF8,
                System.Text.Encoding.GetEncoding("GB2312"),
                System.Text.Encoding.Default
            };
            
            foreach (var encoding in encodings)
            {
                try
                {
                    string content = File.ReadAllText(filePath, encoding);
                    
                    // 检查是否有乱码（简单检测：检查是否有大量的?或特殊字符）
                    int questionMarkCount = content.Count(c => c == '?');
                    if (questionMarkCount < content.Length * 0.1) // 如果?少于10%，认为是正确的编码
                    {
                        return content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    }
                }
                catch
                {
                    continue;
                }
            }
            
            // 如果所有编码都失败，使用默认编码
            return File.ReadAllLines(filePath);
        }
    }
}
