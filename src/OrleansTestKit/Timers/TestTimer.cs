using System;
using System.Threading.Tasks;

namespace Orleans.TestKit.Timers
{
    public class TestTimer : IDisposable
    {
        private Func<object, Task> _asyncCallback;
        private object _state;

        public TestTimer(Func<object, Task> asyncCallback, object state)
        {
            _asyncCallback = asyncCallback;
            _state = state;
        }

        public void Fire()
        {
            _asyncCallback?.Invoke(_state);
        }

        public void Dispose()
        {
            _asyncCallback = null;
            _state = null;
        }
    }
}