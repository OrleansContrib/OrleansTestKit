using System;
using System.Collections.Generic;
using Orleans.Runtime;

namespace Orleans.TestKit.Loggers
{
    internal sealed class TestLogger : Logger
    {
        private readonly TestLogManager _logManager;

        public TestLogger(TestLogManager logManager)
        {
            _logManager = logManager;
        }

        public override Logger GetLogger(string loggerName)
        {
            throw new NotImplementedException();
        }

        public override void Log(int errorCode, Severity sev, string format, object[] args, Exception exception)
        {
            //Create the formatted message
            var message = args == null || args.Length == 0 ? format : string.Format(format, args);

            var time = DateTime.UtcNow;

            _logManager.WriteLog($"{time}  {errorCode}  {sev}  {Name}  {message}");

            //Write any exceptions
            if (exception != null)
                _logManager.WriteLog($"{time}  {errorCode}  {sev}  {Name}  {exception}");
        }

        public override Severity SeverityLevel { get; }
        public override string Name { get; }
    }
}