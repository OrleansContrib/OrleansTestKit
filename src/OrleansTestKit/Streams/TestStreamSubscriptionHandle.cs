using System;
using System.Threading.Tasks;
using Orleans.Streams;

namespace Orleans.TestKit.Streams
{
    internal sealed class TestStreamSubscriptionHandle<T> :
        StreamSubscriptionHandle<T>
    {
        private readonly Action _unsubscribe;
        private readonly Guid _handleId = Guid.NewGuid();
        private readonly string _namespace = "SuperDuperCrazyTown";

        public TestStreamSubscriptionHandle(Action unsubscribe) =>
            _unsubscribe = unsubscribe ?? throw new ArgumentNullException(nameof(unsubscribe));

        public override Guid HandleId
        {
            get
            {
                return _handleId;
            }
        }

        public override string ProviderName
        {
            get
            {
                return _namespace;
            }
        }

        public override IStreamIdentity StreamIdentity
        {
            get
            {
                return new TestStreamId(_handleId, _namespace);
            }
        }

        public override Task<StreamSubscriptionHandle<T>> ResumeAsync(IAsyncObserver<T> observer, StreamSequenceToken token = null) =>
            throw new NotImplementedException();

        public override Task<StreamSubscriptionHandle<T>> ResumeAsync(IAsyncBatchObserver<T> observer, StreamSequenceToken token = null) =>
            throw new NotImplementedException();

        public override bool Equals(StreamSubscriptionHandle<T> other) =>
            ReferenceEquals(this, other);

        public override Task UnsubscribeAsync()
        {
            _unsubscribe();
            return Task.CompletedTask;
        }
    }
}
