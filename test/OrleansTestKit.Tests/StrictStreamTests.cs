using System;
using System.Threading.Tasks;
using FluentAssertions;
using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests
{
    public class StrictStreamTests : TestKitBase
    {
        public StrictStreamTests()
        {
            Silo.Options.StrictStreamProbes = true;
        }

        [Fact]
        public async Task GrainSentMessages()
        {
            var chatty = Silo.CreateGrain<Chatty>(4);

            var stream = Silo.AddStreamProbe<ChatMessage>(Guid.Empty, null);

            const string msg = "Hello Chat";

            await chatty.SendChat(msg);

            stream.Sends.Should().Be(1);
            stream.VerifySend(m => m.Msg == msg);
        }

        [Fact]
        public async Task IncorrectVerifyMessage()
        {
            var chatty = Silo.CreateGrain<Chatty>(4);

            var stream = Silo.AddStreamProbe<ChatMessage>(Guid.Empty, null);

            const string msg = "Hello Chat";

            await chatty.SendChat(msg);

            stream.Invoking(s => s.VerifySend(m => m.Msg == "This is not right")).ShouldThrow<Exception>();
        }

        [Fact]
        public void IncorrectProbeId()
        {
            var chatty = Silo.CreateGrain<Chatty>(4);

            Silo.AddStreamProbe<ChatMessage>(Guid.NewGuid(), null);

            const string msg = "Hello Chat";

            chatty.Invoking(p => p.SendChat(msg)).ShouldThrowExactly<Exception>();
        }

        [Fact]
        public void IncorrectProbeNamespace()
        {
            var chatty = Silo.CreateGrain<Chatty>(4);

            Silo.AddStreamProbe<ChatMessage>(Guid.Empty, "Wrong");

            const string msg = "Hello Chat";

            chatty.Invoking(p => p.SendChat(msg)).ShouldThrowExactly<Exception>();
        }

        [Fact]
        public void GrainIsSubscribed()
        {
            var stream = Silo.AddStreamProbe<ChatMessage>(Guid.Empty, null);

            Silo.CreateGrain<Listener>(1);

            stream.Subscribed.Should().Be(1);
        }

        [Fact]
        public async Task GrainReceives()
        {
            var stream = Silo.AddStreamProbe<ChatMessage>(Guid.Empty, null);

            var grain = Silo.CreateGrain<Listener>(1);

            await stream.OnNextAsync(new ChatMessage("Ding"));

            (await grain.ReceivedCount()).Should().Be(1);
        }
    }
}