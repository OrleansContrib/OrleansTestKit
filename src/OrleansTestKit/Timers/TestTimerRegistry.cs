using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Orleans.Runtime;
using Orleans.Timers;

namespace Orleans.TestKit.Timers
{
    public sealed class TestTimerRegistry :
        ITimerRegistry
    {
        private readonly List<TestTimer> _timers = new();

        public Mock<ITimerRegistry> Mock { get; } = new();


        public IDisposable RegisterTimer(IGrainContext grainContext, Func<object, Task> asyncCallback, object state, TimeSpan dueTime, TimeSpan period)
        {
            if (grainContext == null)
            {
                throw new ArgumentNullException(nameof(grainContext));
            }

            Mock.Object.RegisterTimer(grainContext, asyncCallback, state, dueTime, period);
            var timer = new TestTimer(asyncCallback, state);
            _timers.Add(timer);
            return timer;
        }

        public Task FireAsync(int index) =>
            _timers[index].FireAsync();

        public async Task FireAllAsync()
        {
            foreach (var testTimer in new List<TestTimer>(_timers))
            {
                await testTimer.FireAsync().ConfigureAwait(false);
            }
        }

        public int NumberOfActiveTimers => _timers.Count(x => !x.IsDisposed);
    }
}
