# Log

The `Log` class creates meaningful messages from any object, using current request data, and sends them to your `LogWriter`.  
You implement `LogWriter : ILogWriter` to control where the message is stored. 
- Register it as a service in your app, which is done automatically if you use AddFrameworkServices<T>()

#### Remarks
`Log.Error()` generates a log message and calls your `LogWriter` to store it.  

If `ILogWriter` is not registered, `Log.Dump` is used (not recommended for production due to performance and disk usage).
