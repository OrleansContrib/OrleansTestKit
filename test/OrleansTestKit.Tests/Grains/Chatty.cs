using System;
using System.Linq;
using System.Threading.Tasks;
using Orleans;
using Orleans.Streams;
using TestInterfaces;

namespace TestGrains
{
    public class Chatty : Grain, IChatty
    {
        private (string Message, int Id) _recievedMessage;
        private StreamSubscriptionHandle<(string Message, int Id)> _subscription;

        public Task<(string Message, int Id)> GetMessage()
        {
            return Task.FromResult(_recievedMessage);
        }

        public async Task SendChat(string msg)
        {

            var provider = this.GetStreamProvider("Default");

            var stream = provider.GetStream<ChatMessage>(Guid.Empty);

            await stream.OnNextAsync(new ChatMessage(msg));
        }

        public async Task SendChatBatch(params string[] chats)
        {
            var provider = this.GetStreamProvider("Default");

            var stream = provider.GetStream<ChatMessage>(Guid.Empty);

            await stream.OnNextBatchAsync(chats.Select(chat => new ChatMessage(chat)));
        }

        public async Task Subscribe()
        {
            var provider = this.GetStreamProvider("Default");
            var stream = provider.GetStream<(string Message, int Id)>(Guid.Empty);
            _subscription = await stream.SubscribeAsync(async (item, _) =>
            {
                _recievedMessage = item;
                if ("goodbye".Equals(_recievedMessage.Message, StringComparison.OrdinalIgnoreCase))
                {
                    if (_subscription != null)
                    {
                        await _subscription.UnsubscribeAsync();
                        _subscription = null;
                    }
                }
            });
        }
    }
}
