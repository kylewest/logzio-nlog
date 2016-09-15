using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using NLog.Common;
using NLog.Config;
using NLog.Targets;

namespace NLog.LogzIO
{
    [Target("LogzIO")]
    public class LogzIOTarget : TargetWithLayout
    {
        [RequiredParameter]
        public string Host { get; set; }

        [RequiredParameter]
        public int Port { get; set; }

        public string TypeField { get; set; }

        [RequiredParameter]
        public string LogzAccountToken { get; set; }

        private Socket mClient;
        private static string mLocalHostName;
        private static readonly object mLockObj = new object();
        private static JsonSerializerSettings mJsonSerializerSettings;

        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            mJsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new LowercaseContractResolver(),
                DateFormatString = "dd/MM/yyyy hh:mm:ss.ff"
            };

            mClient = new Socket(SocketType.Stream, ProtocolType.Tcp);
            mClient.Connect(new DnsEndPoint(Host, Port));
        }

        protected override void CloseTarget()
        {
            if (mClient != null)
            {
                mClient.Dispose();
            }

            base.CloseTarget();
        }

        public static string HostName
        {
            get
            {
                if (mLocalHostName == null)
                {
                    lock (mLockObj)
                    {
                        if (mLocalHostName != null) return mLocalHostName;
                        try
                        {
                            var host = Dns.GetHostEntry(Dns.GetHostName());

                            var localIp = host.AddressList.Where(ip => ip.AddressFamily.ToString() == "InterNetwork")
                                .Select(ip => ip.ToString())
                                .FirstOrDefault();

                            mLocalHostName = localIp == default(string) ? "?" : localIp;
                        }
                        catch
                        {
                            mLocalHostName = "1.1.1.1";
                        }
                    }
                }

                return mLocalHostName;
            }
        }

        protected override void Write(LogEventInfo logEvent)
        {
            try
            {
                var logEventProperties = logEvent.Properties;

                var fieldNameToUseAsDataType = GetFieldNameToUseAsDateType(logEventProperties);

                // In case we have NewLine LogzIO may translate it as 2 different messages
                var message = GetFilteredMessage(logEvent).Replace(Environment.NewLine, " ");

                var messageToSend = new LogzIOLogEventInfo(LogzAccountToken, message, DateTime.UtcNow, HostName, logEvent.Level, logEvent.Exception)
                {
                    Type = fieldNameToUseAsDataType ?? "json",
                    Details = logEventProperties
                };

                var buffer = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(messageToSend, Formatting.None, mJsonSerializerSettings) + "\n");
                if (mClient.Send(buffer, buffer.Length, SocketFlags.None) == 0)
                {
                    InternalLogger.Error("Connection Error - 0 bytes transfered to LogzIo");
                }
            }
            catch (Exception ex)
            {
                InternalLogger.Error("Error while writing to LogzIO Target {0}", ex);
            }
        }

        private static string GetFilteredMessage(LogEventInfo logEvent)
        {
            return new string(logEvent.Message.Where(c => (int) c >= 32 && (int) c <= 126).ToArray());
        }

        private string GetFieldNameToUseAsDateType(IDictionary<object, object> logEventProperties)
        {
            return logEventProperties.Where(prop => String.Equals(prop.Key.ToString(), TypeField, StringComparison.CurrentCultureIgnoreCase))
                    .Select(prop => prop.Value.ToString())
                    .FirstOrDefault();
        }
    }
}
