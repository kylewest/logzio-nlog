using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NLog.LogzIO
{
    internal class LogzIOLogEventInfo
    {
        public LogzIOLogEventInfo(string token, string message, DateTime timestamp, string host, string level)
        {
            Token = token;
            Message = message;
            Timestamp = timestamp;
            Host = host;
            Level = level;
        }

        public string Application { get; set; }

        public string Token { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string Type { get; set; }
        public string LoggerName { get; set; }
        public string Host { get; set; }
        public string Level { get; set; }
        public Exception Exception { get; set; }

        [JsonExtensionData]
        public IDictionary<object, object> Details { get; set; }
    }
}
