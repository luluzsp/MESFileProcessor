using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MESFileProcessor
{
    public class MESService
    {
        public const string MesApiUrl = "http://10.131.30.150:20009/zjyt/exi/me/bobcat";
        public const string ComponentApiUrl = "http://10.131.30.150:20010/Service.action";

        private static readonly HttpClient httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        public class MesApiResponse
        {
            public bool Success { get; set; }
            public string ResponseContent { get; set; }
            public string ErrorMessage { get; set; }
        }

        public class SerializeDataResponse
        {
            public List<OldSfcData> OLDSFC_DATA { get; set; }
            public string RESULT { get; set; }
        }

        public class OldSfcData
        {
            public string shoporder { get; set; }
        }

        public static async Task<SerializeDataResponse> GetSerializeData(string sn, Action<string> logCallback)
        {
            string parameters = JsonConvert.SerializeObject(new { LOGIN_ID = "-1", CLIENT_ID = "1", SFC = sn });
            string response = await ExecuteComponentPost("GetSerializeData", parameters, logCallback);
            if (response == null) return null;
            try { return JsonConvert.DeserializeObject<SerializeDataResponse>(response); }
            catch (Exception ex)
            {
                WriteUiLog(logCallback, "GetSerializeData响应解析失败: " + ex.Message);
                LogManager.WriteErrorLog("GetSerializeData响应解析失败", ex);
                return null;
            }
        }

        public static async Task<bool> AddSfcKey(string sn, string stationName, string shoporder,
            string dataName, string dataValue, Action<string> logCallback)
        {
            string parameters = JsonConvert.SerializeObject(new
            {
                LOGIN_ID = "-1", CLIENT_ID = "1", SFC = sn, STATION_NAME = stationName,
                SHOPORDER = shoporder, DATA_NAME = dataName, DATA_VALUE = dataValue,
                IS_FINISH = "1", QTY = "0", IS_UPDATE = true
            });
            string response = await ExecuteComponentPost("AddSfcKey", parameters, logCallback);
            if (response == null) return false;
            try
            {
                var result = JsonConvert.DeserializeObject<AddSfcKeyResponse>(response);
                return result != null && string.Equals(result.RESULT, "PASS", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                WriteUiLog(logCallback, "AddSfcKey响应解析失败: " + ex.Message);
                LogManager.WriteErrorLog("AddSfcKey响应解析失败", ex);
                return false;
            }
        }

        private class AddSfcKeyResponse { public string RESULT { get; set; } }

        private static async Task<string> ExecuteComponentPost(string method, string parameters, Action<string> logCallback)
        {
            string url = ComponentApiUrl + "?method=" + method + "&param=" + Uri.EscapeDataString(parameters);
            try
            {
                WriteUiLog(logCallback, string.Format("【{0}请求URL】{1}", method, url));
                using (var body = new StringContent("", Encoding.UTF8, "application/json"))
                using (var response = await httpClient.PostAsync(url, body))
                {
                    string content = await response.Content.ReadAsStringAsync();
                    LogManager.WriteApiLog(url, parameters, content);
                    WriteUiLog(logCallback, string.Format("【{0}响应】HTTP {1}: {2}", method, (int)response.StatusCode, LimitLogText(content)));
                    if (response.IsSuccessStatusCode) return content;
                    WriteUiLog(logCallback, string.Format("{0}接口失败: HTTP {1}", method, (int)response.StatusCode));
                    return null;
                }
            }
            catch (Exception ex)
            {
                WriteUiLog(logCallback, string.Format("{0}接口异常: {1}", method, ex.Message));
                LogManager.WriteErrorLog(method + "接口异常", ex);
                return null;
            }
        }

        public static Task<MesApiResponse> StartProduct(
            string stationName,
            string stationId,
            string sn,
            Action<string> logCallback = null)
        {
            return ExecuteGetAsync(
                "START",
                new Dictionary<string, string>
                {
                    { "station_name", stationName },
                    { "c", "start" },
                    { "sn", sn },
                    { "station_id", stationId }
                },
                logCallback);
        }

        public static Task<MesApiResponse> AddRecord(
            string stationName,
            string stationId,
            string sn,
            DateTime startTime,
            DateTime stopTime,
            Action<string> logCallback = null)
        {
            return ExecuteGetAsync(
                "ADD_RECORD",
                new Dictionary<string, string>
                {
                    { "result", "PASS" },
                    { "start_time", FormatMesTime(startTime) },
                    { "c", "ADD_RECORD" },
                    { "stop_time", FormatMesTime(stopTime) },
                    { "station_id", stationId },
                    { "test_station_name", stationName },
                    { "sn", sn }
                },
                logCallback);
        }

        public static Task<MesApiResponse> LogNc(
            string ncCode,
            string stationName,
            string stationId,
            string sn,
            Action<string> logCallback = null)
        {
            return ExecuteGetAsync(
                "LOGNC",
                new Dictionary<string, string>
                {
                    { "nc_code", ncCode },
                    { "c", "LOGNC" },
                    { "user_id", "bobcat" },
                    { "station_id", stationId },
                    { "test_station_name", stationName },
                    { "sn", sn }
                },
                logCallback);
        }

        private static async Task<MesApiResponse> ExecuteGetAsync(
            string apiName,
            IDictionary<string, string> parameters,
            Action<string> logCallback)
        {
            string url = BuildUrl(parameters);

            try
            {
                WriteUiLog(logCallback, string.Format("【{0}请求URL】{1}", apiName, url));

                using (HttpResponseMessage response = await httpClient.GetAsync(url))
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    string trimmedContent = (responseContent ?? string.Empty).Trim();
                    bool businessSuccess = trimmedContent.StartsWith("0 SFC_OK", StringComparison.OrdinalIgnoreCase);
                    bool success = response.IsSuccessStatusCode && businessSuccess;

                    WriteUiLog(logCallback, string.Format(
                        "【{0}响应状态】{1} ({2})",
                        apiName,
                        response.StatusCode,
                        (int)response.StatusCode));
                    WriteUiLog(logCallback, string.Format(
                        "【{0}响应内容】{1}",
                        apiName,
                        LimitLogText(responseContent)));
                    LogManager.WriteApiLog(url, BuildParameterLog(parameters), responseContent);

                    if (success)
                    {
                        return new MesApiResponse
                        {
                            Success = true,
                            ResponseContent = responseContent,
                            ErrorMessage = string.Empty
                        };
                    }

                    string errorMessage;
                    if (!response.IsSuccessStatusCode)
                    {
                        errorMessage = string.Format("HTTP状态码异常: {0} ({1})", response.StatusCode, (int)response.StatusCode);
                    }
                    else if (string.IsNullOrWhiteSpace(trimmedContent))
                    {
                        errorMessage = "MES响应为空";
                    }
                    else
                    {
                        errorMessage = string.Format("MES业务响应失败: {0}", LimitLogText(trimmedContent));
                    }

                    WriteUiLog(logCallback, string.Format("【{0}失败】{1}", apiName, errorMessage));
                    LogManager.WriteLog(string.Format("{0}接口调用失败", apiName), errorMessage);

                    return new MesApiResponse
                    {
                        Success = false,
                        ResponseContent = responseContent,
                        ErrorMessage = errorMessage
                    };
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("{0}接口调用异常: {1}", apiName, ex.Message);
                WriteUiLog(logCallback, string.Format("【API异常】{0}", errorMessage));
                LogManager.WriteErrorLog(errorMessage, ex);

                return new MesApiResponse
                {
                    Success = false,
                    ResponseContent = string.Empty,
                    ErrorMessage = errorMessage
                };
            }
        }

        private static string BuildUrl(IDictionary<string, string> parameters)
        {
            StringBuilder builder = new StringBuilder(MesApiUrl);
            builder.Append('?');
            bool first = true;

            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                if (!first)
                {
                    builder.Append('&');
                }

                builder.Append(Uri.EscapeDataString(parameter.Key));
                builder.Append('=');
                builder.Append(Uri.EscapeDataString(parameter.Value ?? string.Empty));
                first = false;
            }

            return builder.ToString();
        }

        private static string BuildParameterLog(IDictionary<string, string> parameters)
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(parameter.Key);
                builder.Append('=');
                builder.Append(parameter.Value);
            }

            return builder.ToString();
        }

        private static string FormatMesTime(DateTime value)
        {
            return value.ToString("yyyy/M/d H:mm:ss", CultureInfo.InvariantCulture);
        }

        private static string LimitLogText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "<空>";
            }

            return text.Length > 1000 ? text.Substring(0, 1000) + "..." : text;
        }

        private static void WriteUiLog(Action<string> logCallback, string message)
        {
            if (logCallback != null)
            {
                logCallback(message);
            }
        }
    }
}
