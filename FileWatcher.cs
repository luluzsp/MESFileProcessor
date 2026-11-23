using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace MESFileProcessor
{
    public class FileWatcher
    {
        private FileSystemWatcher watcher;
        private readonly string watchPath;
        private readonly string outputPath;
        private readonly Config config;
        private readonly Action<string> logCallback;
        private readonly ConcurrentDictionary<string, byte> processingFiles = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);

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

            EnsureDirectories();
            ProcessExistingFiles();

            watcher = new FileSystemWatcher(watchPath);
            watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName;
            watcher.Created += OnFileCreated;
            watcher.Changed += OnFileChanged;
            watcher.EnableRaisingEvents = true;

            logCallback(string.Format("开始监控XML文件夹: {0}", watchPath));
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
                LogManager.WriteLog("文件监控停止", string.Empty);
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
            await ProcessFile(e.FullPath);
        }

        private async Task ProcessFile(string filePath)
        {
            if (!processingFiles.TryAdd(filePath, 0))
            {
                return;
            }

            try
            {
                // 等待写入方释放文件，避免读取到尚未写完的XML。
                await Task.Delay(1000);

                if (!File.Exists(filePath))
                {
                    return;
                }

                string fileName = Path.GetFileName(filePath);
                string extension = Path.GetExtension(filePath);

                logCallback(string.Format("检测到文件: {0}", fileName));
                LogManager.WriteLog("检测到文件", filePath);

                if (!string.Equals(extension, ".xml", StringComparison.OrdinalIgnoreCase))
                {
                    MoveToErrorFolder(filePath, "仅支持XML格式文件");
                    return;
                }

                await ProcessXmlFile(filePath);
            }
            catch (Exception ex)
            {
                string message = string.Format("处理文件时出错: 文件={0}, 异常={1}", filePath, ex.Message);
                logCallback(message);
                LogManager.WriteErrorLog("处理文件异常", ex);
                MoveToFailFolder(filePath, message);
            }
            finally
            {
                byte ignored;
                processingFiles.TryRemove(filePath, out ignored);
            }
        }

        private async Task ProcessXmlFile(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string sn;
            string panelResult;
            Dictionary<string, string> components;
            string ncContext;

            try
            {
                ReadXmlFields(filePath, out sn, out panelResult, out components, out ncContext);
            }
            catch (Exception ex)
            {
                FailXml(filePath, string.Format("XML解析失败: {0}", ex.Message), ex);
                return;
            }

            if (string.IsNullOrWhiteSpace(sn))
            {
                FailXml(filePath, "XML缺少产品SN: 未找到Lotid节点或节点内容为空");
                return;
            }

            if (string.IsNullOrWhiteSpace(panelResult))
            {
                FailXml(filePath, "XML缺少测试结果: 未找到PanleResult节点或节点内容为空");
                return;
            }

            sn = sn.Trim();
            panelResult = panelResult.Trim().ToUpperInvariant();

            bool isPassResult = panelResult == "GOOD" || panelResult == "PASS";
            if (!isPassResult && panelResult != "NG")
            {
                FailXml(filePath, string.Format("XML测试结果不支持: PanleResult={0}，仅支持GOOD、PASS或NG", panelResult));
                return;
            }

            if (!isPassResult && string.IsNullOrWhiteSpace(ncContext))
            {
                FailXml(filePath, "NG文件缺少NC/ComponentPos，无法调用NcComplete");
                return;
            }

            foreach (string requiredName in (config.ComponentNames ?? string.Empty)
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(name => name.Trim()).Where(name => name.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                string componentValue;
                if (!requiredName.EndsWith("_BC", StringComparison.OrdinalIgnoreCase) ||
                    !components.TryGetValue(requiredName, out componentValue) ||
                    string.IsNullOrWhiteSpace(componentValue))
                {
                    string reason = string.Format("{0}文件中没有{1}组件信息", fileName, requiredName);
                    logCallback(reason);
                    LogManager.WriteLog("组件信息缺少", reason);
                    MoveToErrorFolder(filePath, reason);
                    return;
                }
            }

            string parsedMessage = string.Format("解析XML成功: 文件={0}, SN={1}, PanleResult={2}", fileName, sn, panelResult);
            logCallback(parsedMessage);
            LogManager.WriteLog("XML解析成功", parsedMessage);

            var sfcInfo = await MESService.GetSfcInfo(sn, logCallback);
            if (sfcInfo == null || sfcInfo.SFC == null ||
                string.IsNullOrWhiteSpace(sfcInfo.SFC.SCHEDULING_ID) ||
                string.IsNullOrWhiteSpace(sfcInfo.SFC.SHOPORDER) ||
                string.IsNullOrWhiteSpace(sfcInfo.SFC.LINE))
            {
                string reason = "GetSfcInfo失败或缺少SCHEDULING_ID/SHOPORDER/LINE";
                LogApiFailure(fileName, sn, reason);
                MoveToFailFolder(filePath, reason);
                return;
            }

            logCallback(string.Format("【API调用】准备调用START接口，SN={0}", sn));
            MESService.MesApiResponse startResponse = await MESService.StartProduct(
                sn, sfcInfo.SFC.SCHEDULING_ID, config.StationName,
                sfcInfo.SFC.LINE, config.WorkStation, config.OperatorId, logCallback);

            if (startResponse == null || !startResponse.Success)
            {
                string reason = BuildApiFailureReason("START", startResponse);
                LogApiFailure(fileName, sn, reason);
                MoveToFailFolder(filePath, reason);
                return;
            }

            string finalApiName = isPassResult ? "Complete" : "NcComplete";
            MESService.MesApiResponse stationResponse = await MESService.Complete(
                sn, sfcInfo.SFC.SCHEDULING_ID, config.StationName,
                config.WorkStation, config.OperatorId, isPassResult ? null : ncContext, logCallback);

            if (stationResponse == null || !stationResponse.Success)
            {
                string reason = BuildApiFailureReason(finalApiName, stationResponse);
                LogApiFailure(fileName, sn, reason);
                MoveToFailFolder(filePath, reason);
                return;
            }

            string successReason = string.Format("START和{0}接口调用成功，SN={1}", finalApiName, sn);
            logCallback(successReason);
            LogManager.WriteLog("MES过站成功", successReason);

            if (components.Count > 0)
            {
                // 工单查询和组件上传属于过站后的附加步骤，不改变过站结果。
                string shoporder = sfcInfo.SFC.SHOPORDER;
                if (string.IsNullOrWhiteSpace(shoporder))
                {
                    string message = string.Format("GetSfcInfo失败或缺少SHOPORDER: 文件={0}, SN={1}，跳过组件上传", fileName, sn);
                    logCallback(message);
                    LogManager.WriteLog("组件上传失败", message);
                }
                else
                {
                    foreach (var component in components.Where(item => !string.IsNullOrWhiteSpace(item.Value)))
                    {
                        bool uploaded = await MESService.AddSfcKey(sn, config.StationName, shoporder,
                            component.Key, component.Value, logCallback);
                        if (!uploaded)
                        {
                            string message = string.Format("AddSfcKey失败: 文件={0}, SN={1}, DATA_NAME={2}", fileName, sn, component.Key);
                            logCallback(message);
                            LogManager.WriteLog("组件上传失败", message);
                        }
                    }
                }
            }
            MoveToPassFolder(filePath, successReason);
        }

        private static void ReadXmlFields(string filePath, out string sn, out string panelResult,
            out Dictionary<string, string> components, out string ncContext)
        {
            sn = null;
            panelResult = null;
            components = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            ncContext = null;

            XmlReaderSettings settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                IgnoreComments = true,
                IgnoreWhitespace = true
            };

            using (XmlReader reader = XmlReader.Create(filePath, settings))
            {
                reader.Read();
                bool inNc = false;
                while (!reader.EOF)
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "NC") inNc = true;
                    if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "NC") inNc = false;
                    if (inNc && ncContext == null && reader.NodeType == XmlNodeType.Element &&
                        reader.LocalName == "ComponentPos")
                    {
                        string position = reader.ReadElementContentAsString().Trim();
                        if (!string.IsNullOrWhiteSpace(position))
                        {
                            ncContext = position;
                        }
                        continue;
                    }
                    if (reader.NodeType == XmlNodeType.Element &&
                        string.Equals(reader.LocalName, "Lotid", StringComparison.OrdinalIgnoreCase))
                    {
                        sn = reader.ReadElementContentAsString();
                        continue;
                    }

                    if (reader.NodeType == XmlNodeType.Element &&
                        string.Equals(reader.LocalName, "PanleResult", StringComparison.OrdinalIgnoreCase))
                    {
                        panelResult = reader.ReadElementContentAsString();
                        continue;
                    }

                    if (reader.NodeType == XmlNodeType.Element &&
                        reader.LocalName.EndsWith("_BC", StringComparison.OrdinalIgnoreCase))
                    {
                        string name = reader.LocalName;
                        string value = reader.ReadElementContentAsString().Trim();
                        if (!components.ContainsKey(name) || string.IsNullOrWhiteSpace(components[name]))
                        {
                            components[name] = value;
                        }
                        continue;
                    }

                    reader.Read();
                }
            }
        }

        private void FailXml(string filePath, string reason, Exception exception = null)
        {
            string fileName = Path.GetFileName(filePath);
            string message = string.Format("XML文件处理失败: 文件={0}, 原因={1}", fileName, reason);
            logCallback(message);

            if (exception == null)
            {
                LogManager.WriteLog("XML文件处理失败", message);
            }
            else
            {
                LogManager.WriteErrorLog(message, exception);
            }

            MoveToFailFolder(filePath, reason);
        }

        private void LogApiFailure(string fileName, string sn, string reason)
        {
            string message = string.Format("MES接口调用失败: 文件={0}, SN={1}, 原因={2}", fileName, sn, reason);
            logCallback(message);
            LogManager.WriteLog("MES接口调用失败", message);
        }

        private static string BuildApiFailureReason(string apiName, MESService.MesApiResponse response)
        {
            if (response == null)
            {
                return string.Format("{0}接口未返回结果", apiName);
            }

            return string.Format("{0}接口失败: {1}", apiName, response.ErrorMessage);
        }

        private void MoveToErrorFolder(string filePath, string reason)
        {
            MoveFile(filePath, "error", "文件移动到error", reason);
        }

        private void MoveToFailFolder(string filePath, string reason)
        {
            MoveFile(filePath, "fail", "文件移动到fail", reason);
        }

        private void MoveToPassFolder(string filePath, string reason)
        {
            MoveFile(filePath, "pass", "文件移动到pass", reason);
        }

        private void MoveFile(string filePath, string subFolder, string action, string reason)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    logCallback(string.Format("文件已被处理或不存在，跳过移动: {0}", filePath));
                    return;
                }

                string fileName = Path.GetFileName(filePath);
                string targetPath = GetUniqueFilePath(outputPath, subFolder, fileName);
                File.Move(filePath, targetPath);

                string message = string.Format("文件 {0} 移动到{1}文件夹: {2}", fileName, subFolder, reason);
                logCallback(message);
                LogManager.WriteLog(action, message);
            }
            catch (Exception ex)
            {
                logCallback(string.Format("移动文件到{0}文件夹时出错: {1}", subFolder, ex.Message));
                LogManager.WriteErrorLog(string.Format("移动文件到{0}文件夹失败", subFolder), ex);
            }
        }

        private static string GetUniqueFilePath(string basePath, string subFolder, string fileName)
        {
            string targetPath = Path.Combine(basePath, subFolder, fileName);
            if (!File.Exists(targetPath))
            {
                return targetPath;
            }

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            return Path.Combine(basePath, subFolder, string.Format("{0}_{1}{2}", fileNameWithoutExtension, timestamp, extension));
        }
    }
}
