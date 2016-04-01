# logzio-nlog
NLog target for Logz.io

Community Plugin -  Send logs from NLog

There are 2 Nlog’s config file:

1. Using NLog’s AsyncWrapper – NLog has a generic async wrapper that writes all the messages to a queue and every 50ms it reads 100 items from it and sends It to the target, the limit of the queue is 10000 but and it discards messages if the queue is empty. All this parameters are configurable, you can read more about it in https://github.com/nlog/NLog/wiki/AsyncWrapper-target.

2. Sync target - Calls the target on every message

The performance impact is significant (sending 10,000 messages using async wrapper takes about 250 ms and using the sync target takes about 14800 ms)
 
We recommend using the Async wrapper with the default parameters

About the implementation:

The parameter named “typeField “  should include the name of the property that will be used as the type of the message,
For ex : in case you set that parameter to be “RoleName” and then sends NLog message using the following code:

```
Logger mLogger = LogManager.GetLogger(name);
mLogger.Log(LogLevel.Debug)
       .Message(“Some Message”)
       .Properties(new Dictionary<string, object>(){{"RoleName", "LogType-RoleName"}, {"OtherProperty", "TestProp"}})
       .Write();
```
The target will put “LogType-RoleName” as the type of the message 
In case you don’t add the property “RoleName” it will put “json” as the default type.
