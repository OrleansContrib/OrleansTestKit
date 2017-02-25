using System;
using System.Collections.Concurrent;
using Orleans.Runtime;

namespace Orleans.TestKit.Loggers
{
    internal sealed class TestLogManager
    {
        private readonly ConcurrentDictionary<string, TestLogger> _loggers =
            new ConcurrentDictionary<string, TestLogger>();

        public Logger GetLogger(string loggerName) => _loggers.GetOrAdd(loggerName, new TestLogger(this));

        public void WriteLog(string log)
        {
            Console.WriteLine(log);
        }
    }
}