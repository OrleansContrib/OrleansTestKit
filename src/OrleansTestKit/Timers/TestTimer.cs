using System;
using System.Threading.Tasks;

namespace Orleans.TestKit.Timers
{
    public sealed class TestTimer :
        IDisposable
    {
        private Func<object, Task> _asyncCallback;

        private object _state;

        public TestTimer(Func<object, Task> asyncCallback, object state)
        {
            _asyncCallback = asyncCallback;
            _state = state;
        }

        public Task FireAsync()
        {
            if (_asyncCallback == null)
            {
                return Task.CompletedTask;
            }

            return _asyncCallback(_state);
        }

        public void Dispose()
        {
            _asyncCallback = null;
            _state = null;
        }
    }
}
