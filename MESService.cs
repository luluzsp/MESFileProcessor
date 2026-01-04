using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MESFileProcessor
{
    public class MESService
    {
        private static readonly HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        
        public class SerializeDataResponse
        {
            public int SERVICE_RUNTIME { get; set; }
            public List<OldSfcData> OLDSFC_DATA { get; set; }
            public int? LOAD_ID { get; set; }
            public object NEWSFC_DATA { get; set; }
            public string RESULT { get; set; }
        }
        
        public class OldSfcData
        {
            public string createName { get; set; }
            public string createTime { get; set; }
            public int id { get; set; }
            public string module { get; set; }
            public string newSfc { get; set; }
            public string oldSfc { get; set; }
            public string shoporder { get; set; }
            public string splitNum { get; set; }
            public string station { get; set; }
        }
        
        public class AddSfcKeyResponse
        {
            public string RESULT { get; set; }
        }
        
        public static async Task<SerializeDataResponse> GetSerializeData(string mesUrl, string sfc, Action<string> logCallback = null)
        {
            try
            {
                var paramObj = new
                {
                    LOGIN_ID = "-1",
                    CLIENT_ID = "1",
                    SFC = sfc
                };
                
                string parameters = JsonConvert.SerializeObject(paramObj);
                string url = string.Format("{0}?method=GetSerializeData&param={1}", mesUrl, parameters);
                
                // 在界面显示完整请求URL
                if (logCallback != null)
                {
                    logCallback(string.Format("【请求URL】{0}", url));
                }
                
                var content = new StringContent("", Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(url, content);
                
                string responseContent = await response.Content.ReadAsStringAsync();
                
                // 在界面显示完整响应信息
                if (logCallback != null)
                {
                    logCallback(string.Format("【响应状态】{0} ({1})", response.StatusCode, (int)response.StatusCode));
                    logCallback(string.Format("【响应内容】{0}", responseContent.Length > 500 ? responseContent.Substring(0, 500) + "..." : responseContent));
                }
                
                LogManager.WriteApiLog(url, parameters, responseContent);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<SerializeDataResponse>(responseContent);
                    return result;
                }
                else
                {
                    LogManager.WriteLog("API调用失败", string.Format("状态码: {0}, 响应: {1}", response.StatusCode, responseContent));
                    return null;
                }
            }
            catch (Exception ex)
            {
                if (logCallback != null)
                {
                    logCallback(string.Format("【API异常】GetSerializeData调用失败: {0}", ex.Message));
                }
                LogManager.WriteErrorLog("GetSerializeData API调用异常", ex);
                return null;
            }
        }
        
        public static async Task<AddSfcKeyResponse> AddSfcKey(string mesUrl, string sfc, string stationName, 
            string shoporder, string lineName, string dataName, string dataValue, Action<string> logCallback = null)
        {
            try
            {
                var paramObj = new
                {
                    LOGIN_ID = "-1",
                    CLIENT_ID = "1",
                    SFC = sfc,
                    STATION_NAME = stationName,
                    SHOPORDER = shoporder,
                    LINE = lineName,
                    DATA_NAME = dataName,
                    DATA_VALUE = dataValue
                };
                
                string parameters = JsonConvert.SerializeObject(paramObj);
                string url = string.Format("{0}?method=AddSfcKey&param={1}", mesUrl, parameters);
                
                // 在界面显示完整请求URL
                if (logCallback != null)
                {
                    logCallback(string.Format("【请求URL】{0}", url));
                }
                
                var content = new StringContent("", Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(url, content);
                
                string responseContent = await response.Content.ReadAsStringAsync();
                
                // 在界面显示完整响应信息
                if (logCallback != null)
                {
                    logCallback(string.Format("【响应状态】{0} ({1})", response.StatusCode, (int)response.StatusCode));
                    logCallback(string.Format("【响应内容】{0}", responseContent.Length > 500 ? responseContent.Substring(0, 500) + "..." : responseContent));
                }
                
                LogManager.WriteApiLog(url, parameters, responseContent);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<AddSfcKeyResponse>(responseContent);
                    return result;
                }
                else
                {
                    LogManager.WriteLog("addSfcKey API调用失败", string.Format("状态码: {0}, 响应: {1}", response.StatusCode, responseContent));
                    return null;
                }
            }
            catch (Exception ex)
            {
                if (logCallback != null)
                {
                    logCallback(string.Format("【API异常】addSfcKey调用失败: {0}", ex.Message));
                }
                LogManager.WriteErrorLog("addSfcKey API调用异常", ex);
                return null;
            }
        }
    }
}
