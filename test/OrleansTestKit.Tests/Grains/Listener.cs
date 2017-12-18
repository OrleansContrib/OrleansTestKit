using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Streams;
using TestInterfaces;

namespace TestGrains
{
    public class Listener : Grain, IListener
    {
        private int _receivedCount;

        public override Task OnActivateAsync()
        {
            var stream = GetStreamProvider("Default").GetStream<ChatMessage>(Guid.Empty, null);

            stream.SubscribeAsync((data, token) =>
            {
                _receivedCount++;

                return Task.CompletedTask;
            });

            return base.OnActivateAsync();
        }

        public Task<int> ReceivedCount() => Task.FromResult(_receivedCount);
    }
}