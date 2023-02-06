namespace Orleans.TestKit.Timers;

public sealed class TestTimer : IDisposable
{
    private Func<Task>? _asyncCallback;

    public TestTimer(Func<object, Task> asyncCallback, object state) =>
        _asyncCallback = () => asyncCallback(state);

    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
        _asyncCallback = null;
        IsDisposed = true;
    }

    public Task FireAsync() =>
        IsDisposed || _asyncCallback == null
            ? throw new ObjectDisposedException(GetType().FullName)
            : _asyncCallback();
}
