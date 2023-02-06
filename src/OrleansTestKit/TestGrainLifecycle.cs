using System.Collections.ObjectModel;
using Orleans.Runtime;
using Orleans.TestKit.Utilities;

namespace Orleans.TestKit;

internal sealed class TestGrainLifecycle : IGrainLifecycle
{
    private readonly Collection<(int Stage, ILifecycleObserver Observer)> _observers = new();

    public IDisposable Subscribe(string observerName, int stage, ILifecycleObserver observer)
    {
        if (observer == null)
        {
            throw new ArgumentNullException(nameof(observer));
        }

        var item = (Stage: stage, Observer: observer);
        _observers.Add(item);
        return new LambdaDisposable(() =>
        {
            _observers.Remove(item);
        });
    }

    public Task TriggerStartAsync()
    {
        var tasks = _observers.OrderBy(x => x.Stage).Select(x => x.Observer.OnStart(CancellationToken.None));
        return Task.WhenAll(tasks.ToArray());
    }

    public Task TriggerStopAsync()
    {
        var tasks = _observers.Select(x => x.Observer.OnStop(CancellationToken.None));
        return Task.WhenAll(tasks.ToArray());
    }
}
