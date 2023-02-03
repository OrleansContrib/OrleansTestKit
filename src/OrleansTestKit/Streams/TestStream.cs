using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Orleans.Runtime;
using Orleans.Streams;

namespace Orleans.TestKit.Streams
{
    [SuppressMessage("Microsoft.Design", "CA1036:OverrideMethodsOnComparableTypes")]
    [SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix")]
    public sealed class TestStream<T> : IAsyncStream<T>, IStreamIdentity
    {
        private readonly List<IAsyncObserver<T>> _observers = new List<IAsyncObserver<T>>();

        private readonly Mock<IAsyncStream<T>> _mockStream = new Mock<IAsyncStream<T>>();

        private readonly List<StreamSubscriptionHandle<T>> _handlers = new List<StreamSubscriptionHandle<T>>();

        public bool IsRewindable => false;
        public string ProviderName { get; }
        public StreamId StreamId { get; }

        public Guid Guid { get; }

        public string Namespace { get; }

        /// <summary>
        /// Number of times OneNextAsync was called
        /// </summary>
        public uint Sends { get; private set; }

        public int Subscribed => _observers.Count;

        public TestStream(StreamId streamId, string providerName)
        {
            StreamId = streamId;
            ProviderName = providerName ?? throw new ArgumentNullException(nameof(providerName));
        }

        public int CompareTo(IAsyncStream<T> other) =>
            throw new NotImplementedException();

        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public bool Equals(IAsyncStream<T> other) =>
            throw new NotImplementedException();

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


        /// <summary>
        /// Create an empty handler that can then be used to test resuming streams
        /// </summary>
        public Task<StreamSubscriptionHandle<T>> AddEmptyStreamHandler(Action<IAsyncObserver<T>> onAttachingObserver = null)
        {
            var handle = CreateEmptyStreamHandlerImpl(onAttachingObserver);
            _handlers.Add(handle);

            return Task.FromResult<StreamSubscriptionHandle<T>>(handle);
        }

        private TestStreamSubscriptionHandle<T> CreateEmptyStreamHandlerImpl(
            Action<IAsyncObserver<T>> onAttachingObserver = null)
        {
            TestStreamSubscriptionHandle<T> handle = null;

            handle = new TestStreamSubscriptionHandle<T>(
                StreamId,
                ProviderName,
                observer =>
                {
                    _handlers.Remove(handle);
                    if (observer != null)
                    {
                        _observers.Remove(observer);
                    }
                }, observer =>
                {
                    onAttachingObserver?.Invoke(observer);
                    _observers.Add(observer);
                }, observer =>
                {
                    _observers.Remove(observer);
                });

            return handle;
        }

        public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncObserver<T> observer, StreamSequenceToken token, string filterData = null) => throw new NotImplementedException();

        public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncBatchObserver<T> observer) =>
            throw new NotImplementedException();

        public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncBatchObserver<T> observer, StreamSequenceToken token) =>
            throw new NotImplementedException();

        public Task OnNextAsync(T item, StreamSequenceToken token = null)
        {
            Sends++;
            _mockStream.Object.OnNextAsync(item, token);
            return Task.WhenAll(_observers.ToList().Select(o => o.OnNextAsync(item, token)));
        }

        public Task OnCompletedAsync() =>
            Task.WhenAll(_observers.ToList().Select(o => o.OnCompletedAsync()));

        public Task OnErrorAsync(Exception ex) =>
            Task.WhenAll(_observers.ToList().Select(o => o.OnErrorAsync(ex)));

        [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
        public async Task OnNextBatchAsync(IEnumerable<T> batch, StreamSequenceToken token = null)
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

        public Task<IList<StreamSubscriptionHandle<T>>> GetAllSubscriptionHandles() =>
            Task.FromResult<IList<StreamSubscriptionHandle<T>>>(new List<StreamSubscriptionHandle<T>>(_handlers));

        public void VerifySend(Func<T, bool> check) =>
            VerifySend(check, Times.Once());

        public void VerifySend(Func<T, bool> check, Times times) =>
            _mockStream.Verify(s => s.OnNextAsync(It.Is<T>(a => check(a)), It.IsAny<StreamSequenceToken>()), times);
    }
}
