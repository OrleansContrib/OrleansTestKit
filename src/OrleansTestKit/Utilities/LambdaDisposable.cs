using System;

namespace Orleans.TestKit.Utilities
{
    internal sealed class LambdaDisposable : IDisposable
    {
        private Action _action;

        public LambdaDisposable(Action action)
        {
            this._action = action;
        }
        public void Dispose()
        {
            this._action();
        }
    }
}
