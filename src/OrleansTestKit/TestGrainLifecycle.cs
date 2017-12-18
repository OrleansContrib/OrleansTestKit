using System;
using System.Collections.Generic;
using Orleans.Runtime;
using Orleans.TestKit.Utilities;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace Orleans.TestKit
{
    internal sealed class TestGrainLifecycle : IGrainLifecycle
    {
        private readonly List<ILifecycleObserver> observers = new List<ILifecycleObserver>();

        public IDisposable Subscribe(int stage, ILifecycleObserver observer)
        {
            observers.Add(observer);

            return new LambdaDisposable(() =>
            {
                observers.Remove(observer);
            });
        }

        public void TriggerStart()
        {
            var tasks = observers.Select(x => x.OnStart(CancellationToken.None));

            Task.WaitAll(tasks.ToArray(), 1000);
        }

        public void TriggerStop()
        {
            var tasks = observers.Select(x => x.OnStop(CancellationToken.None));

            Task.WaitAll(tasks.ToArray(), 1000);
        }
    }
}
