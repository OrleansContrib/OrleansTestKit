namespace Orleans.TestKit.Utilities;

internal sealed class LambdaDisposable : IDisposable
{
    private Action _action;

    public LambdaDisposable(Action action)
    {
        _action = action;
    }

    public void Dispose()
    {
        _action?.Invoke();
    }
}
