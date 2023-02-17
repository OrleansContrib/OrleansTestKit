#if NSUBSTITUTE

using NSubstitute;

#else
using Moq;
#endif

using Orleans.Runtime;
using Orleans.Timers;

namespace Orleans.TestKit.Timers;

public sealed class TestTimerRegistry : ITimerRegistry
{
    private readonly List<TestTimer> _timers = new();

#if NSUBSTITUTE

    public ITimerRegistry Mock { get; } = Substitute.For<ITimerRegistry>();

#else
    public Mock<ITimerRegistry> Mock { get; } = new();
#endif

    public int NumberOfActiveTimers => _timers.Count(x => !x.IsDisposed);

    public async Task FireAllAsync()
    {
        foreach (var testTimer in new List<TestTimer>(_timers))
        {
            await testTimer.FireAsync().ConfigureAwait(false);
        }
    }

    public Task FireAsync(int index) =>
        _timers[index].FireAsync();

    public IDisposable RegisterTimer(IGrainContext grainContext, Func<object, Task> asyncCallback, object state, TimeSpan dueTime, TimeSpan period)
    {
        if (grainContext == null)
        {
            throw new ArgumentNullException(nameof(grainContext));
        }
#if NSUBSTITUTE
        Mock.RegisterTimer(grainContext, asyncCallback, state, dueTime, period);
#else
        Mock.Object.RegisterTimer(grainContext, asyncCallback, state, dueTime, period);
#endif
        var timer = new TestTimer(asyncCallback, state);
        _timers.Add(timer);
        return timer;
    }
}
