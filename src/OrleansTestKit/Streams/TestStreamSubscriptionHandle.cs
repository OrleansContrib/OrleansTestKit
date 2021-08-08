using System;
using System.IO;
using System.Threading.Tasks;
using Orleans.Streams;

namespace Orleans.TestKit.Streams
{
    internal sealed class TestStreamSubscriptionHandle<T> :
        StreamSubscriptionHandle<T>
    {
        private readonly Action _unsubscribe;

        public TestStreamSubscriptionHandle(Action unsubscribe) =>
            _unsubscribe = unsubscribe ?? throw new ArgumentNullException(nameof(unsubscribe));

        public override Guid HandleId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string ProviderName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override IStreamIdentity StreamIdentity
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Task<StreamSubscriptionHandle<T>> ResumeAsync(IAsyncObserver<T> observer,
            StreamSequenceToken token = null) => ResumeAsyncImpl();

        public override Task<StreamSubscriptionHandle<T>> ResumeAsync(IAsyncBatchObserver<T> observer,
            StreamSequenceToken token = null) => ResumeAsyncImpl();

        private Task<StreamSubscriptionHandle<T>> ResumeAsyncImpl() => Task.FromResult<StreamSubscriptionHandle<T>>(this);

        public override bool Equals(StreamSubscriptionHandle<T> other) =>
            ReferenceEquals(this, other);

        public override Task UnsubscribeAsync()
        {
            _unsubscribe();
            return Task.CompletedTask;
        }
    }
}
