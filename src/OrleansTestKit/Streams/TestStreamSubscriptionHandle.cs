using Orleans.Runtime;
using Orleans.Streams;

namespace Orleans.TestKit.Streams;

internal sealed class TestStreamSubscriptionHandle<TObserver, T> : StreamSubscriptionHandle<T>
    where TObserver : class
{
    private readonly Guid _handleId = Guid.NewGuid();

    private readonly StreamId _streamIdentity;
    private readonly string _providerName;
    private readonly Action<TObserver> _onAttaching;
    private readonly Action<TObserver> _onDetaching;
    private readonly Action<TObserver> _unsubscribe;

    private TObserver? _observer;

    public TestStreamSubscriptionHandle(StreamId streamId, string providerName, Action<TObserver> unsubscribe, Action<TObserver> onAttaching, Action<TObserver> onDetaching)
    {
        _streamIdentity = streamId;
        _providerName = providerName;
        _unsubscribe = unsubscribe;
        _onAttaching = onAttaching;
        _onDetaching = onDetaching;
    }

    public override Guid HandleId => _handleId;

    public override string ProviderName => _providerName;

    public override StreamId StreamId => _streamIdentity;

    public override bool Equals(StreamSubscriptionHandle<T> other) => ReferenceEquals(this, other);

    public override Task<StreamSubscriptionHandle<T>> ResumeAsync(IAsyncObserver<T> observer, StreamSequenceToken? token = null) => ResumeGenericAsync(observer);

    public override Task<StreamSubscriptionHandle<T>> ResumeAsync(IAsyncBatchObserver<T> observer, StreamSequenceToken? token = null) => ResumeGenericAsync(observer);

    private Task<StreamSubscriptionHandle<T>> ResumeGenericAsync(object observer)
    {
        if (observer is not TObserver typedObserver)
            throw new InvalidOperationException($"Subscription resumed with {observer.GetType().Name} but stream probe is of type {typeof(TObserver).Name}");

        DetachCurrentObserver();
        _observer = typedObserver;
        _onAttaching(_observer);
        return Task.FromResult<StreamSubscriptionHandle<T>>(this);
    }

    public override Task UnsubscribeAsync()
    {
        if (_observer is not null)
        {
            _unsubscribe(_observer);
        }

        return Task.CompletedTask;
    }

    private void DetachCurrentObserver()
    {
        if (_observer is not null)
        {
            _onDetaching(_observer);
            _observer = null;
        }
    }
}
