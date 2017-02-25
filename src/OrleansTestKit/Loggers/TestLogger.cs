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

        public override void TrackDependency(string name, string commandName, DateTimeOffset startTime, TimeSpan duration, bool success)
        {
            throw new NotImplementedException();
        }

        public override void TrackEvent(string name, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            throw new NotImplementedException();
        }

        public override void TrackMetric(string name, double value, IDictionary<string, string> properties = null)
        {
            throw new NotImplementedException();
        }

        public override void TrackMetric(string name, TimeSpan value, IDictionary<string, string> properties = null)
        {
            throw new NotImplementedException();
        }

        public override void IncrementMetric(string name)
        {
            throw new NotImplementedException();
        }

        public override void IncrementMetric(string name, double value)
        {
            throw new NotImplementedException();
        }

        public override void DecrementMetric(string name)
        {
            throw new NotImplementedException();
        }

        public override void DecrementMetric(string name, double value)
        {
            throw new NotImplementedException();
        }

        public override void TrackRequest(string name, DateTimeOffset startTime, TimeSpan duration, string responseCode, bool success)
        {
            throw new NotImplementedException();
        }

        public override void TrackException(Exception exception, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            throw new NotImplementedException();
        }

        public override void TrackTrace(string message)
        {
            throw new NotImplementedException();
        }

        public override void TrackTrace(string message, Severity severityLevel)
        {
            throw new NotImplementedException();
        }

        public override void TrackTrace(string message, Severity severityLevel, IDictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }

        public override void TrackTrace(string message, IDictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }

        public override Severity SeverityLevel { get; }
        public override string Name { get; }
    }

//    public class TestLogger :Logger
//    {
//        private readonly LoggerType _loggerType;
//        private readonly ITestOutputHelper _output;
//        private readonly string _loggerName;
//
//        public static Severity GlobalSeverity { get; set; } = Severity.Info;
//
//        private static readonly ConcurrentDictionary<string, TestLogger> LoggerCache = new ConcurrentDictionary<string, TestLogger>();
//
//        public static TestLogger GetLogger(ITestOutputHelper output, string loggerName, LoggerType logType)
//        {
//            return LoggerCache.GetOrAdd(loggerName, _ => new TestLogger(output, loggerName, logType));
//        }
//
//        private TestLogger(ITestOutputHelper output,string loggerName,LoggerType logType)
//        {
//            _output = output;
//            _loggerName = loggerName;
//            _loggerType = logType;
//        }
//
//        private void Log(int errorCode, Severity sev, string format, object[] args, Exception exception)
//        {
//            if (sev > SeverityLevel)
//            {
//                return;
//            }
//
//            //Create the formatted message
//            var message = args == null || args.Length == 0 ? format : string.Format(format, args);
//
//            var logEntry = $"{DateTime.UtcNow}     {errorCode}     {sev}     {_loggerName}     {message}";
//
//            _output.WriteLine(logEntry);
//
//            //Write any exceptions
//            if(exception != null)
//                _output.WriteLine(exception.ToString());
//        }
//
//        public override void Verbose(string format, params object[] args)
//        {
//            Log(0, Severity.Verbose, format, args, null);
//        }
//
//        public override void Verbose2(string format, params object[] args)
//        {
//            Log(0, Severity.Verbose2, format, args, null);
//        }
//
//        public override void Verbose3(string format, params object[] args)
//        {
//            Log(0, Severity.Verbose3, format, args, null);
//        }
//
//        public override void Info(string format, params object[] args)
//        {
//            Log(0, Severity.Info, format, args, null);
//        }
//
//        public override void Error(int logCode, string message, Exception exception = null)
//        {
//            Log(logCode, Severity.Error, message, new object[] { }, exception);
//        }
//
//        public override void Warn(int logCode, string format, params object[] args)
//        {
//            Log(logCode, Severity.Warning, format, args, null);
//        }
//
//        public override void Warn(int logCode, string message, Exception exception = null)
//        {
//            Log(logCode, Severity.Warning, message, new object[] { }, exception);
//        }
//
//        public override void Info(int logCode, string format, params object[] args)
//        {
//            Log(logCode, Severity.Info, format, args, null);
//        }
//
//        public override void Verbose(int logCode, string format, params object[] args)
//        {
//            Log(logCode, Severity.Verbose, format, args, null);
//        }
//
//        public override void Verbose2(int logCode, string format, params object[] args)
//        {
//            Log(logCode, Severity.Verbose2, format, args, null);
//        }
//
//        public override void Verbose3(int logCode, string format, params object[] args)
//        {
//            Log(logCode, Severity.Verbose3, format, args, null);
//        }
//
//        public override void TrackDependency(string name, string commandName, DateTimeOffset startTime, TimeSpan duration, bool success)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override void TrackEvent(string name, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override void TrackMetric(string name, double value, IDictionary<string, string> properties = null)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override void TrackMetric(string name, TimeSpan value, IDictionary<string, string> properties = null)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override void IncrementMetric(string name)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override void IncrementMetric(string name, double value)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override void DecrementMetric(string name)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override void DecrementMetric(string name, double value)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override void TrackRequest(string name, DateTimeOffset startTime, TimeSpan duration, string responseCode, bool success)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override void TrackException(Exception exception, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override void TrackTrace(string message)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override void TrackTrace(string message, Severity severityLevel)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override void TrackTrace(string message, Severity severityLevel, IDictionary<string, string> properties)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override void TrackTrace(string message, IDictionary<string, string> properties)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override Severity SeverityLevel => GlobalSeverity;
//    }
}