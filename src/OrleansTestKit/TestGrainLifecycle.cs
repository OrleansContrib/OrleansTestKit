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
        private readonly List<(int, ILifecycleObserver)> observers = new List<(int, ILifecycleObserver)>();

        public IDisposable Subscribe(int stage, ILifecycleObserver observer)
        {
            var item = (stage, observer);

            observers.Add(item);

            return new LambdaDisposable(() =>
            {
                observers.Remove(item);
            });
        }

        public void TriggerStart()
        {
            var tasks = observers.OrderBy(x => x.Item1).Select(x => x.Item2.OnStart(CancellationToken.None));

            Task.WaitAll(tasks.ToArray(), 1000);
        }

        public void TriggerStop()
        {
            var tasks = observers.Select(x => x.Item2.OnStop(CancellationToken.None));

            Task.WaitAll(tasks.ToArray(), 1000);
        }
    }
}
