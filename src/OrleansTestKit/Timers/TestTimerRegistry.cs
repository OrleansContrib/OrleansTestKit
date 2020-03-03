using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Orleans.Timers;

namespace Orleans.TestKit.Timers
{
    public sealed class TestTimerRegistry :
        ITimerRegistry
    {
        private readonly List<TestTimer> _timers = new List<TestTimer>();

        public Mock<ITimerRegistry> Mock { get; } = new Mock<ITimerRegistry>();

        public IDisposable RegisterTimer(Grain grain, Func<object, Task> asyncCallback, object state, TimeSpan dueTime, TimeSpan period)
        {
            if (grain == null)
            {
                throw new ArgumentNullException(nameof(grain));
            }

            Mock.Object.RegisterTimer(grain, asyncCallback, state, dueTime, period);
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
    }
}
