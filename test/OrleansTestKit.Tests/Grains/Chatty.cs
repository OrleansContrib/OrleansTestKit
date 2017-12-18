using System;
using System.Threading.Tasks;
using Orleans;
using TestInterfaces;

namespace TestGrains
{
    public class Chatty : Grain, IChatty
    {
        public async Task SendChat(string msg)
        {
            var provider = GetStreamProvider("Default");

            var stream = provider.GetStream<ChatMessage>(Guid.Empty, null);

            await stream.OnNextAsync(new ChatMessage(msg));
        }
    }
}
