using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Orleans.Streams;

namespace Orleans.TestKit.Streams
{
    public sealed class TestStream<T> : IAsyncStream<T>
    {
        private readonly List<IAsyncObserver<T>> _observers = new List<IAsyncObserver<T>>();

        private readonly Mock<IAsyncStream<T>> _mockStream = new Mock<IAsyncStream<T>>();

        private readonly List<StreamSubscriptionHandle<T>> _handlers = new List<StreamSubscriptionHandle<T>>();

        public Guid Guid { get; }

        public string Namespace { get; }

        public bool IsRewindable { get; } = false;

        public string ProviderName { get; }

        /// <summary>
        /// Number of times OneNextAsync was called
        /// </summary>
        public uint Sends { get; private set; }

        public int Subscribed => _observers.Count;

        public TestStream(Guid streamId, string streamNamespace, string providerName)
        {
            Guid = streamId;
            Namespace = streamNamespace;
            ProviderName = providerName;
        }

        public bool Equals(IAsyncStream<T> other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IAsyncStream<T> other)
        {
            throw new NotImplementedException();
        }

        public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncObserver<T> observer)
        {
            _observers.Add(observer);

            TestStreamSubscriptionHandle<T> handle = null;
            handle = new TestStreamSubscriptionHandle<T>(() =>
            {
                _observers.Remove(observer);
                _handlers.Remove(handle);
            });

            _handlers.Add(handle);
            return Task.FromResult<StreamSubscriptionHandle<T>>(handle);
        }

        public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncObserver<T> observer, StreamSequenceToken token,
            StreamFilterPredicate filterFunc = null,
            object filterData = null)
        {
            throw new NotImplementedException();
        }

        public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncBatchObserver<T> observer)
        {
            throw new NotImplementedException();
        }

        public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncBatchObserver<T> observer, StreamSequenceToken token)
        {
            throw new NotImplementedException();
        }

        public Task OnNextAsync(T item, StreamSequenceToken token = null)
        {
            Sends++;

            _mockStream.Object.OnNextAsync(item, token);

            return Task.WhenAll(_observers.Select(o => o.OnNextAsync(item, token)));
        }

        public Task OnCompletedAsync()
        {
            return Task.WhenAll(_observers.Select(o => o.OnCompletedAsync()));
        }

        public Task OnErrorAsync(Exception ex)
        {
            return Task.WhenAll(_observers.Select(o => o.OnErrorAsync(ex)));
        }

        public Task OnNextBatchAsync(IEnumerable<T> batch, StreamSequenceToken token = null)
        {
            throw new NotImplementedException();
        }

        public Task<IList<StreamSubscriptionHandle<T>>> GetAllSubscriptionHandles()
        {
            return Task.FromResult<IList<StreamSubscriptionHandle<T>>>(new List<StreamSubscriptionHandle<T>>(_handlers));
        }

        public void VerifySend(Func<T, bool> check) => VerifySend(check, Times.Once());

        public void VerifySend(Func<T, bool> check, Times times)
        {
            _mockStream.Verify(s => s.OnNextAsync(It.Is<T>(a => check(a)), It.IsAny<StreamSequenceToken>()), times);
        }
    }
}
