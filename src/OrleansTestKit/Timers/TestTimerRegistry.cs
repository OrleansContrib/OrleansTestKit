using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Orleans.Timers;

namespace Orleans.TestKit.Timers
{
    public class TestTimerRegistry : ITimerRegistry
    {
        private readonly List<TestTimer> _timers = new List<TestTimer>();

        public readonly Mock<ITimerRegistry> Mock = new Mock<ITimerRegistry>();

        public IDisposable RegisterTimer(Grain grain, Func<object, Task> asyncCallback, object state, TimeSpan dueTime, TimeSpan period)
        {
            // Trigger the internal mock so it can be verified
            Mock.Object.RegisterTimer(grain, asyncCallback, state, dueTime, period);

            var timer = new TestTimer(asyncCallback, state);
            _timers.Add(timer);

            return timer;
        }

        [Obsolete("Use FireAsync instead.")]
        public void Fire(int index) => _timers[index].Fire();

        [Obsolete("Use FireAllAsync instead.")]
        public void FireAll()
        {
            foreach (var testTimer in _timers.ToArray())
                testTimer.Fire();
        }

        public Task FireAsync(int index) => _timers[index].FireAsync();

        public async Task FireAllAsync()
        {
            foreach (var testTimer in _timers.ToArray())
            {
                await testTimer.FireAsync();
            }
        }
    }
}
