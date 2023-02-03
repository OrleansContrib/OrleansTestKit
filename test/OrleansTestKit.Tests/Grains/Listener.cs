using System;
using System.Threading;
using System.Threading.Tasks;
using Orleans;
using Orleans.Streams;
using TestInterfaces;

namespace TestGrains
{
    public class Listener : Grain, IListener
    {
        private int _receivedCount;

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var stream = this.GetStreamProvider("Default").GetStream<ChatMessage>(Guid.Empty);

            stream.SubscribeAsync((data, token) =>
            {
                _receivedCount++;

                return Task.CompletedTask;
            });

            return base.OnActivateAsync(cancellationToken);
        }

        public Task<int> ReceivedCount() => Task.FromResult(_receivedCount);
    }
}
