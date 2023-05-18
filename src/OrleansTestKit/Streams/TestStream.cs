using System.Diagnostics.CodeAnalysis;
using Moq;
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
    public int Subscribed => _observers.Count;

    /// <summary>
    /// Create an empty handler that can then be used to test resuming streams.
    /// </summary>
    /// <param name="onAttachingObserver">action to fire.</param>
    /// <returns>stream handle.</returns>
    public Task<StreamSubscriptionHandle<T>> AddEmptyStreamHandler(Action<IAsyncObserver<T>>? onAttachingObserver = null)
    {
        var handle = CreateEmptyStreamHandlerImpl(onAttachingObserver);
        _handlers.Add(handle);

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
    public Task OnCompletedAsync() =>
        Task.WhenAll(_observers.ToList().Select(o => o.OnCompletedAsync()));

    /// <inheritdoc/>
    public Task OnErrorAsync(Exception ex) =>
        Task.WhenAll(_observers.ToList().Select(o => o.OnErrorAsync(ex)));

    /// <inheritdoc/>
    public Task OnNextAsync(T item, StreamSequenceToken? token = null)
    {
        Sends++;
        _mockStream.Object.OnNextAsync(item, token);
        return Task.WhenAll(_observers.ToList().Select(o => o.OnNextAsync(item, token)));
    }

    /// <inheritdoc/>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "intentional")]
    public async Task OnNextBatchAsync(IEnumerable<T>? batch, StreamSequenceToken? token = null)
    {
        if (batch == null)
        {
            throw new ArgumentNullException(nameof(batch));
        }

        var innerExceptions = new List<Exception>();
        foreach (var item in batch)
        {
            try
            {
                await OnNextAsync(item, token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                innerExceptions.Add(ex);
            }
        }

        if (innerExceptions.Count > 0)
        {
            throw new AggregateException(innerExceptions);
        }
    }

    /// <inheritdoc/>
    public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncObserver<T> observer)
    {
        if (observer == null)
        {
            throw new ArgumentNullException(nameof(observer));
        }

        var handle = CreateEmptyStreamHandlerImpl();
        handle.AttachObserver(observer);
        _handlers.Add(handle);

        return Task.FromResult<StreamSubscriptionHandle<T>>(handle);
    }

    /// <inheritdoc/>
    public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncObserver<T> observer, StreamSequenceToken? token, string? filterData = null) =>
        throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncBatchObserver<T>? observer) =>
        throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncBatchObserver<T>? observer, StreamSequenceToken? token) =>
        throw new NotImplementedException();

    /// <summary>
    /// Verify that the stream was sent.
    /// </summary>
    /// <param name="check">item to check.</param>
    public void VerifySend(Func<T, bool> check) =>
        VerifySend(check, Times.Once());

    /// <summary>
    /// Verify that the stream was sent the specified item N times.
    /// </summary>
    /// <param name="check">item to check.</param>
    /// <param name="times">number of times.</param>
    public void VerifySend(Func<T, bool> check, Times times) =>
        _mockStream.Verify(s => s.OnNextAsync(It.Is<T>(a => check(a)), It.IsAny<StreamSequenceToken>()), times);

    private TestStreamSubscriptionHandle<T> CreateEmptyStreamHandlerImpl(
        Action<IAsyncObserver<T>>? onAttachingObserver = null)
    {
        TestStreamSubscriptionHandle<T>? handle = null;

        handle = new TestStreamSubscriptionHandle<T>(
            StreamId,
            ProviderName,
            observer =>
            {
                _handlers.Remove(handle!);
                if (observer != null)
                {
                    _observers.Remove(observer);
                }
            },
            observer =>
            {
                onAttachingObserver?.Invoke(observer);
                _observers.Add(observer);
            },
            observer =>
            {
                _observers.Remove(observer);
            });

        return handle;
    }
}
