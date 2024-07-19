using Moq;
using Orleans.Timers;

namespace Orleans.TestKit.Timers;

/// <summary>
/// A test timer registry that contains state of operations performed on it
/// </summary>
public sealed class TestTimerRegistry : ITimerRegistry
{
    private readonly List<TestTimer> _timers = new();

    /// <summary>
    /// The underlying mock
    /// </summary>
    public Mock<ITimerRegistry> Mock { get; } = new();

    /// <summary>
    /// Count the timers that have not be disposed of yet.
    /// </summary>
    public int NumberOfActiveTimers => _timers.Count(x => !x.IsDisposed);

    /// <summary>
    /// Fire all timers
    /// </summary>
    /// <returns>Task</returns>
    public async Task FireAllAsync()
    {
        foreach (var testTimer in new List<TestTimer>(_timers.Where(x => !x.IsDisposed)))
        {
            await testTimer.FireAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Fire a timer manually by index
    /// </summary>
    /// <param name="index">The index of the timer to fire</param>
    /// <returns>Task</returns>
    public Task FireAsync(int index) => _timers[index].FireAsync();

    /// <inheritdoc/>
    [Obsolete]
    public IDisposable RegisterTimer(IGrainContext grainContext, Func<object?, Task> asyncCallback, object? state, TimeSpan dueTime, TimeSpan period)
    {
        if (grainContext == null)
        {
            throw new ArgumentNullException(nameof(grainContext));
        }

        Mock.Object.RegisterTimer(grainContext, asyncCallback, state, dueTime, period);

        var timer = new TestTimer(asyncCallback, state!);
        _timers.Add(timer);

        return timer;
    }

    public IGrainTimer RegisterGrainTimer<TState>(IGrainContext grainContext, Func<TState, CancellationToken, Task> asyncCallback, TState state,
        GrainTimerCreationOptions options)
    {
        if (grainContext == null)
        {
            throw new ArgumentNullException(nameof(grainContext));
        }

        var cb = new Func<object?, CancellationToken, Task>((s, ct) => asyncCallback(((TState?)s)!, ct));

        Mock.Object.RegisterGrainTimer(grainContext, asyncCallback, state, options);

        var grainTimer = new TestGrainTimer(cb, state!);
        _timers.Add(grainTimer);

        return grainTimer;
    }
}
