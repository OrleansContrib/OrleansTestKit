using System;
using System.IO;
using System.Threading.Tasks;
using Orleans.Runtime;
using Orleans.Streams;

namespace Orleans.TestKit.Streams
{
    internal sealed class TestStreamSubscriptionHandle<T> :
        StreamSubscriptionHandle<T>
    {
        private readonly Action<IAsyncObserver<T>> _unsubscribe;
        private readonly Action<IAsyncObserver<T>> _onAttachingObserver;
        private readonly Action<IAsyncObserver<T>> _onDetachingObserver;
        private readonly Guid _handleId;
        private readonly StreamId _streamIdentity;
        private readonly string _providerName;
        private IAsyncObserver<T> _observer;

        public TestStreamSubscriptionHandle(
            StreamId streamId,
            string providerName,
            Action<IAsyncObserver<T>> unsubscribe,
            Action<IAsyncObserver<T>> onAttachingObserver = null,
            Action<IAsyncObserver<T>> onDetachingObserver = null)
        {
            _unsubscribe = unsubscribe ?? throw new ArgumentNullException(nameof(unsubscribe));
            _onAttachingObserver = onAttachingObserver;
            _onDetachingObserver = onDetachingObserver;

            _handleId = Guid.NewGuid();
            _streamIdentity = streamId;
            _providerName = providerName;
        }


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
                return _providerName;
            }
        }

        public override StreamId StreamId
        {
            get
            {
                return _streamIdentity;
            }
        }

        public override Task<StreamSubscriptionHandle<T>> ResumeAsync(IAsyncObserver<T> observer,
            StreamSequenceToken token = null)
        {
            DetachCurrentObserver();
            AttachObserver(observer);
            return Task.FromResult<StreamSubscriptionHandle<T>>(this);
        }

        public override Task<StreamSubscriptionHandle<T>> ResumeAsync(IAsyncBatchObserver<T> observer,
            StreamSequenceToken token = null) => throw new NotImplementedException();



        public void AttachObserver(IAsyncObserver<T> observer)
        {
            if (_observer != null)
            {
                throw new Exception("You can only have one observer per handler");
            }
            _observer = observer;
            _onAttachingObserver?.Invoke(observer);
        }

        public void DetachCurrentObserver()
        {
            if (_observer == null)
            {
                return;
            }
            _onDetachingObserver?.Invoke(_observer);
            _observer = null;
        }

        public override bool Equals(StreamSubscriptionHandle<T> other) =>
            ReferenceEquals(this, other);

        public override Task UnsubscribeAsync()
        {
            _unsubscribe?.Invoke(_observer);
            return Task.CompletedTask;
        }
    }
}
