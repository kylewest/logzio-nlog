using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NLog.LogzIO
{
    internal class LogzIOLogEventInfo
    {
        public LogzIOLogEventInfo(string pToken, string pMessage, DateTime pLogTimestamp, string pHost, LogLevel pLevel, Exception pException)
        {
            Token = pToken;
            Message = pMessage;
            LogTimestamp = pLogTimestamp;
            Host = pHost;
            Level = pLevel.ToString();
            Exception = pException;
        }

        public string Token { get; set; }
        public string Message { get; set; }
        public DateTime LogTimestamp { get; set; }
        public string Type { get; set; }
        public string Host { get; set; }
        public string Level { get; set; }
        public Exception Exception { get; set; }
        [JsonExtensionData]
        public IDictionary<object, object> Details { get; set; }
    }
}
