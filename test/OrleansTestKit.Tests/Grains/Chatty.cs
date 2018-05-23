using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Streams;
using TestInterfaces;

namespace TestGrains
{
    public class Chatty : Grain, IChatty
    {
        private (string Message, int Id) _recievedMessage;

        public Task<(string Message, int Id)> GetMessage()
        {
            return Task.FromResult(_recievedMessage);
        }

        public async Task SendChat(string msg)
        {
            var provider = GetStreamProvider("Default");

            var stream = provider.GetStream<ChatMessage>(Guid.Empty, null);

            await stream.OnNextAsync(new ChatMessage(msg));
        }

        public async Task Subscribe()
        {
            var provider = GetStreamProvider("Default");

            var stream = provider.GetStream<(string Message, int Id)>(Guid.Empty, null);

            await stream.SubscribeAsync((item, _) =>
            {
                _recievedMessage = item;
                return Task.CompletedTask;
            });
        }
    }
}
