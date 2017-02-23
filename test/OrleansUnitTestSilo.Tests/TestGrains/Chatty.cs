using System;
using System.Threading.Tasks;
using Orleans;
using OrleansUnitTestSilo.Tests.TestGrainInterfaces;

namespace OrleansUnitTestSilo.Tests.TestGrains
{
    public class Chatty : Grain, IChatty
    {
        public Task SendChat(string msg)
        {
            return
                GetStreamProvider("Default")
                    .GetStream<ChatMessage>(Guid.Empty, null)
                    .OnNextAsync(new ChatMessage(msg));
        }
    }
}