namespace Orleans.TestKit.Timers;

/// <summary>
/// Represents a specialization of TestTimer that is used for grain timers.
/// </summary>
public sealed class TestGrainTimer(Func<object?, CancellationToken, Task> asyncCallback, object state)
    : TestTimer(asyncCallback, state), IGrainTimer
{
    public void Change(TimeSpan dueTime, TimeSpan period) => throw new NotSupportedException();
}

/// <summary>
/// A test timer
/// </summary>
public class TestTimer : IDisposable
{
    private Func<Task>? _asyncCallback;
    public readonly CancellationTokenSource Cts = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TestTimer"/> class.
    /// </summary>
    /// <param name="asyncCallback">A callback function to invoke when the timer is fired</param>
    /// <param name="state">The timer's state</param>
    public TestTimer(Func<object?, Task> asyncCallback, object state) =>
        _asyncCallback = () => asyncCallback(state);

    /// <summary>
    /// Initializes a new instance of the <see cref="TestTimer"/> class.
    /// </summary>
    /// <param name="asyncCallback">A callback function to invoke when the timer is fired</param>
    /// <param name="state">The timer's state</param>
    public TestTimer(Func<object?, CancellationToken, Task> asyncCallback, object state) =>
        _asyncCallback = () => asyncCallback(state, Cts.Token);

    /// <summary>
    /// Gets a value indicating whether the timer has been disposed of
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <inheritdoc/>
    public void Dispose()
    {
        _asyncCallback = null;
        IsDisposed = true;

        // used by TestGrainTimer callback only
        Cts.Cancel();
        Cts.Dispose();
    }

    /// <summary>
    /// Fire the timer
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public Task FireAsync() =>
        IsDisposed || _asyncCallback == null
            ? throw new ObjectDisposedException(GetType().FullName)
            : _asyncCallback();
}

