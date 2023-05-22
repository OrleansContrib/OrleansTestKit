using System.Diagnostics.CodeAnalysis;
using Moq;
using Orleans.Providers.Streams.Common;
using Orleans.Runtime;
using Orleans.Streams;

namespace Orleans.TestKit.Streams;

/// <summary>
/// A test stream that implements IAsyncStream.
/// </summary>
/// <typeparam name="T">The stream event type.</typeparam>
[SuppressMessage("Microsoft.Design", "CA1036:OverrideMethodsOnComparableTypes", Justification = "not needed")]
public sealed class TestStream<T> : IAsyncStream<T>, IStreamIdentity
{
    private readonly List<StreamSubscriptionHandle<T>> _handlers = new();

    private readonly Mock<IAsyncStream<T>> _mockStream = new();

    private readonly List<IAsyncObserver<T>> _observers = new();
    private readonly List<IAsyncBatchObserver<T>> _batchObservers = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TestStream{T}"/> class.
    /// </summary>
    /// <param name="streamId">The stream id.</param>
    /// <param name="providerName">The provider name to use.</param>
    public TestStream(StreamId streamId, string providerName)
    {
        StreamId = streamId;
        ProviderName = providerName ?? throw new ArgumentNullException(nameof(providerName));
    }

    /// <inheritdoc/>
    public Guid Guid { get; }

    /// <inheritdoc/>
    public bool IsRewindable => false;

    /// <inheritdoc/>
    public string Namespace { get; } = string.Empty;

    /// <inheritdoc/>
    public string ProviderName { get; }

    /// <summary>Gets the number of times OneNextAsync was called.</summary>
    public uint Sends { get; private set; }

    /// <inheritdoc/>
    public StreamId StreamId { get; }

    /// <summary>
    /// Gets the count of subscribers.
    /// </summary>
    public int Subscribed => _observers.Count + _batchObservers.Count;

    /// <summary>
    /// Create an empty handler that can then be used to test resuming streams.
    /// </summary>
    /// <param name="onAttachingObserver">action to fire.</param>
    /// <returns>stream handle.</returns>
    public Task<StreamSubscriptionHandle<T>> AddEmptyStreamHandler(Action<IAsyncObserver<T>>? onAttachingObserver = null)
    {
        var handle = CreateEmptyStreamHandlerImpl(_observers, onAttachingObserver);

        return Task.FromResult<StreamSubscriptionHandle<T>>(handle);
    }

    /// <inheritdoc/>
    public int CompareTo(IAsyncStream<T>? other) => 0;

    /// <inheritdoc/>
    public bool Equals(IAsyncStream<T>? other) => ReferenceEquals(this, other);

    /// <inheritdoc/>
    public Task<IList<StreamSubscriptionHandle<T>>> GetAllSubscriptionHandles() =>
        Task.FromResult<IList<StreamSubscriptionHandle<T>>>(new List<StreamSubscriptionHandle<T>>(_handlers));

    /// <inheritdoc/>
    public async Task OnCompletedAsync()
    {
        await Task.WhenAll(_observers.ToList().Select(o => o.OnCompletedAsync()));
        await Task.WhenAll(_batchObservers.ToList().Select(o => o.OnCompletedAsync()));
    }

    /// <inheritdoc/>
    public async Task OnErrorAsync(Exception ex)
    {
        await Task.WhenAll(_observers.ToList().Select(o => o.OnErrorAsync(ex)));
        await Task.WhenAll(_batchObservers.ToList().Select(o => o.OnErrorAsync(ex)));
    }

    /// <inheritdoc/>
    public Task OnNextAsync(T item, StreamSequenceToken? token = null)
    {
        Sends++;
        _mockStream.Object.OnNextAsync(item, token);
        return Task.WhenAll(_observers.ToList().Select(o => o.OnNextAsync(item, token)));
    }

    /// <inheritdoc/>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "intentional")]
    public async Task OnNextBatchAsync(IEnumerable<T> batch, StreamSequenceToken? token = null)
    {
        if (batch is null)
            return;

        token ??= new EventSequenceTokenV2(0);

        await _mockStream.Object.OnNextBatchAsync(batch, token);

        var sequentialItems = batch.Select((x, i) => new SequentialItem<T>(x, new EventSequenceTokenV2(token.SequenceNumber, i))).ToArray();

        var innerExceptions = new List<Exception>();

        foreach (var item in sequentialItems)
        {
            try
            {
                await OnNextAsync(item.Item, item.Token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                innerExceptions.Add(ex);
            }
        }

        foreach (var batchObserver in _batchObservers)
        {
            try
            {
                await batchObserver.OnNextAsync(sequentialItems);
            }
            catch (Exception ex)
            {
                innerExceptions.Add(ex);
            }
        }

        if (innerExceptions.Any())
        {
            throw new AggregateException(innerExceptions);
        }
    }

    /// <inheritdoc/>
    public async Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncObserver<T> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        var handle = CreateEmptyStreamHandlerImpl(_observers);

        return await handle.ResumeAsync(observer);
    }

    /// <summary>
    /// This method does not support subscribing to a stream with a filter or from a token -- it merely calls into the default method
    /// </summary>
    /// <param name="observer">The observer.</param>
    /// <param name="token">The token (not used)</param>
    /// <param name="filterData">The filterData (not used)</param>
    /// <returns>The stream handle</returns>
    public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncObserver<T> observer, StreamSequenceToken? token, string? filterData = null) =>
        SubscribeAsync(observer);

    /// <inheritdoc/>
    public async Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncBatchObserver<T>? observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        var handle = CreateEmptyStreamHandlerImpl(_batchObservers);

        return await handle.ResumeAsync(observer);
    }

    /// <summary>
    /// This method does not support subscribing from a token -- it merely calls into the default method
    /// </summary>
    /// <param name="observer">The observer.</param>
    /// <param name="token">The token (not used)</param>
    /// <returns>The stream handle</returns>
    public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncBatchObserver<T>? observer, StreamSequenceToken? token) => SubscribeAsync(observer);

    /// <summary>
    /// Verify that the stream was sent.
    /// </summary>
    /// <param name="check">item to check.</param>
    public void VerifySend(Func<T, bool> check) => VerifySend(check, Times.Once());

    /// <summary>
    /// Verify that the stream was sent the specified item N times.
    /// </summary>
    /// <param name="check">item to check.</param>
    /// <param name="times">number of times.</param>
    public void VerifySend(Func<T, bool> check, Times times) =>
        _mockStream.Verify(s => s.OnNextAsync(It.Is<T>(a => check(a)), It.IsAny<StreamSequenceToken>()), times);

    /// <summary>
    /// Verify that the stream was sent.
    /// </summary>
    public void VerifySendBatch() => VerifySendBatch(Times.Once());

    /// <summary>
    /// Verify that the stream was sent the specified item N times.
    /// </summary>
    /// <param name="times">number of times.</param>
    public void VerifySendBatch(Times times) =>
        _mockStream.Verify(s => s.OnNextBatchAsync(It.IsAny<IEnumerable<T>>(), It.IsAny<StreamSequenceToken>()), times);

    private TestStreamSubscriptionHandle<TObserver, T> CreateEmptyStreamHandlerImpl<TObserver>(List<TObserver> observers, Action<TObserver>? onAttachingObserver = null)
        where TObserver : class
    {
        TestStreamSubscriptionHandle<TObserver, T> handle = null!;

        handle = new TestStreamSubscriptionHandle<TObserver, T>(StreamId, ProviderName,
            unsubscribe: observer =>
            {
                _handlers.Remove(handle!);
                if (observer != null)
                {
                    observers.Remove(observer);
                }
            },
            onAttaching: observer =>
            {
                onAttachingObserver?.Invoke(observer);
                observers.Add(observer);
            },
            onDetaching: observer =>
            {
                observers.Remove(observer);
            }
        );

        _handlers.Add(handle);

        return handle;
    }
}
