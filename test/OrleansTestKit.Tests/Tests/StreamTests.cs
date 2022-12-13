using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Orleans.Streams;
using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests
{
    public class StreamTests : TestKitBase
    {
        [Fact]
        public async Task GrainSentMessages()
        {
            var chatty = await Silo.CreateGrainAsync<Chatty>(4);

            var stream = Silo.AddStreamProbe<ChatMessage>(Guid.Empty, null);

            const string msg = "Hello Chat";

            await chatty.SendChat(msg);

            stream.Sends.Should().Be(1);
            stream.VerifySend(m => m.Msg == msg);
        }

        [Fact]
        public async Task GrainSentBatchMessages()
        {
            var chatty = await Silo.CreateGrainAsync<Chatty>(4);

            var stream = Silo.AddStreamProbe<ChatMessage>(Guid.Empty, null);

            var msgs = new[] { "Hello Chat", "Goodbye Chat" };

            await chatty.SendChatBatch(msgs);

            stream.Sends.Should().Be((uint)msgs.Length);
            foreach (var msg in msgs)
            {
                stream.VerifySend(m => m.Msg == msg);
            }
        }

        [Fact]
        public async Task GrainSentBatchMessagesHandlesException()
        {
            var chatty = await Silo.CreateGrainAsync<Chatty>(4);

            var stream = Silo.AddStreamProbe<ChatMessage>(Guid.Empty, null);
            var mockObserver = new Mock<IAsyncObserver<ChatMessage>>();
            mockObserver.Setup(o => o.OnNextAsync(It.IsAny<ChatMessage>(), It.IsAny<StreamSequenceToken>()))
                .Throws<Exception>();

            await stream.SubscribeAsync(mockObserver.Object);

            var msgs = new[] { "Hello Chat", "Goodbye Chat" };

            await Assert.ThrowsAsync<AggregateException>(() => chatty.SendChatBatch(msgs));

            stream.Sends.Should().Be((uint)msgs.Length);
            foreach (var msg in msgs)
            {
                stream.VerifySend(m => m.Msg == msg);
            }
        }

        [Fact]
        public async Task AddNonReferenceTypeStreamProbe()
        {
            var stream = Silo.AddStreamProbe<(string Message, int Id)>(Guid.Empty, null);

            var chatty = await Silo.CreateGrainAsync<Chatty>(4);
            await chatty.Subscribe();

            const string msg = "Hello Chat";
            const int id = 2;
            await stream.OnNextAsync((msg, id));

            stream.Sends.Should().Be(1);
            stream.VerifySend(m => m.Message == msg);
            stream.VerifySend(m => m.Id == id);

            var message = await chatty.GetMessage();
            Assert.NotEqual(default((string Message, int Id)), message);
            Assert.Equal(msg, message.Message);
            Assert.Equal(id, message.Id);
        }

        [Fact]
        public async Task LazyStreamProvider()
        {
            var chatty = await Silo.CreateGrainAsync<Chatty>(4);

            const string msg = "Hello Chat";

            //Send a message without creating a stream probe
            chatty.Invoking(p => p.SendChat(msg).Wait()).Should().NotThrow();
        }

        [Fact]
        public async Task LazyStreamProviderStrict()
        {
            Silo.Options.StrictStreamProbes = true;

            var chatty = await Silo.CreateGrainAsync<Chatty>(4);

            const string msg = "Hello Chat";

            //This should throw an exception since the provider was not created
            chatty.Invoking(p => p.SendChat(msg).Wait()).Should().Throw<Exception>();
        }

        [Fact]
        public async Task IncorrectVerifyMessage()
        {
            var chatty = await Silo.CreateGrainAsync<Chatty>(4);

            var stream = Silo.AddStreamProbe<ChatMessage>(Guid.Empty, null);

            const string msg = "Hello Chat";

            await chatty.SendChat(msg);

            stream.Invoking(s => s.VerifySend(m => m.Msg == "This is not right")).Should().Throw<Exception>();
        }

        [Fact]
        public async Task IncorrectProbeId()
        {
            var chatty = await Silo.CreateGrainAsync<Chatty>(4);

            Silo.AddStreamProbe<ChatMessage>(Guid.NewGuid(), null);

            const string msg = "Hello Chat";

            chatty.Invoking(p => p.SendChat(msg).Wait()).Should().NotThrow();
        }

        [Fact]
        public async Task IncorrectProbeNamespace()
        {
            var chatty = await Silo.CreateGrainAsync<Chatty>(4);

            Silo.AddStreamProbe<ChatMessage>(Guid.Empty, "Wrong");

            const string msg = "Hello Chat";

            chatty.Invoking(p => p.SendChat(msg).Wait()).Should().NotThrow();
        }

        [Fact]
        public async Task GrainIsSubscribed()
        {
            var stream = Silo.AddStreamProbe<ChatMessage>(Guid.Empty, null);

            await Silo.CreateGrainAsync<Listener>(1);

            stream.Subscribed.Should().Be(1);
        }

        [Fact]
        public async Task GrainIsUnsubscribed()
        {
            var stream = Silo.AddStreamProbe<(string Message, int Id)>(Guid.Empty, null);

            var chatty = await Silo.CreateGrainAsync<Chatty>(4);
            await chatty.Subscribe();

            stream.Subscribed.Should().Be(1);

            const string msg = "Goodbye";
            const int id = 3;
            await stream.OnNextAsync((msg, id));

            stream.Sends.Should().Be(1);
            stream.VerifySend(m => m.Message == msg);
            stream.VerifySend(m => m.Id == id);

            stream.Subscribed.Should().Be(0);
        }

        [Fact]
        public async Task GrainGetAllSubscriptionHandles()
        {
            var stream = Silo.AddStreamProbe<ChatMessage>(Guid.Empty, null);

            await Silo.CreateGrainAsync<Listener>(1);
            var handlers = await stream.GetAllSubscriptionHandles();

            handlers.Count.Should().Be(1);
            stream.Subscribed.Should().Be(1);

            await handlers[0].UnsubscribeAsync();

            handlers.Count.Should().Be(1);
            stream.Subscribed.Should().Be(0);
        }

        [Fact]
        public async Task GrainReceives()
        {
            var stream = Silo.AddStreamProbe<ChatMessage>(Guid.Empty, null);

            var grain = await Silo.CreateGrainAsync<Listener>(1);

            await stream.OnNextAsync(new ChatMessage("Ding"));

            (await grain.ReceivedCount()).Should().Be(1);
        }


        [Fact]
        public async Task SubscriptionHandlesShouldHaveIdentity()
        {
            var stream = Silo.AddStreamProbe<ChatMessage>(Guid.Empty, null);

            await Silo.CreateGrainAsync<Listener>(1);
            var handlers = await stream.GetAllSubscriptionHandles();

            handlers.Count.Should().Be(1);
            stream.Subscribed.Should().Be(1);


            foreach (var handle in handlers)
            {
                handle.ProviderName.Should().Be("Default");
                handle.HandleId.Should().NotBeEmpty();
                handle.StreamId.Should().NotBeNull();
                handle.StreamId.Namespace.Should().NotBeNull();
            }

            await handlers[0].UnsubscribeAsync();

            handlers.Count.Should().Be(1);
            stream.Subscribed.Should().Be(0);
        }

    }
}
