using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace MESFileProcessor
{
    public class ConfigManager
    {
        private static readonly string ConfigPath = Path.Combine(Application.StartupPath, "config.json");
        
        public static Config LoadConfig()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    Config config = JsonConvert.DeserializeObject<Config>(json);
                    return config ?? new Config();
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(string.Format("加载配置文件失败: {0}", ex.Message));
            }
            
            return new Config();
        }
        
        public static void SaveConfig(Config config)
        {
            try
            {
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(ConfigPath, json);
                LogManager.WriteLog("配置文件保存成功");
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(string.Format("保存配置文件失败: {0}", ex.Message));
                throw;
            }
        }
        
        public static string GetConfigDisplayString(Config config)
        {
            return string.Format("工站名称: {0} | 线体名称: {1}", 
                config.StationName, config.LineName);
        }
    }
}
