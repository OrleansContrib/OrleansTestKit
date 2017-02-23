using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Streams;

namespace OrleansNonSiloTesting.Streams
{
    internal sealed class TestStreamSubscriptionHandle<T> : StreamSubscriptionHandle<T>
    {
        private readonly Action _unsubscribe;

        public override IStreamIdentity StreamIdentity { get { throw new NotImplementedException(); } }

        public override Guid HandleId { get {throw new NotImplementedException();} }

        public TestStreamSubscriptionHandle(Action unsubscribe)
        {
            _unsubscribe = unsubscribe;
        }

        public override Task UnsubscribeAsync()
        {
            _unsubscribe();
            return TaskDone.Done;
        }

        public override Task<StreamSubscriptionHandle<T>> ResumeAsync(IAsyncObserver<T> observer,
            StreamSequenceToken token = null)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(StreamSubscriptionHandle<T> other)
        {
            throw new NotImplementedException();
        }
    }
}