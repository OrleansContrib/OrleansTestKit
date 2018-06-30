﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using Orleans.TestKit.Streams;
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
        public async Task LazyStreamProvider()
        {
            var chatty = await Silo.CreateGrainAsync<Chatty>(4);

            const string msg = "Hello Chat";

            //Send a message without creating a stream probe
            chatty.Invoking(p => p.SendChat(msg).Wait()).ShouldNotThrow();
        }

        [Fact]
        public async Task LazyStreamProviderStrict()
        {
            Silo.Options.StrictStreamProbes = true;

            var chatty = await Silo.CreateGrainAsync<Chatty>(4);

            const string msg = "Hello Chat";

            //This should throw an exception since the provider was not created
            chatty.Invoking(p => p.SendChat(msg).Wait()).ShouldThrow<Exception>();
        }

        [Fact]
        public async Task IncorrectVerifyMessage()
        {
            var chatty = await Silo.CreateGrainAsync<Chatty>(4);

            var stream = Silo.AddStreamProbe<ChatMessage>(Guid.Empty, null);

            const string msg = "Hello Chat";

            await chatty.SendChat(msg);

            stream.Invoking(s => s.VerifySend(m => m.Msg == "This is not right")).ShouldThrow<Exception>();
        }

        [Fact]
        public async Task IncorrectProbeId()
        {
            var chatty = await Silo.CreateGrainAsync<Chatty>(4);

            Silo.AddStreamProbe<ChatMessage>(Guid.NewGuid(), null);

            const string msg = "Hello Chat";

            chatty.Invoking(p => p.SendChat(msg).Wait()).ShouldNotThrow();
        }

        [Fact]
        public async Task IncorrectProbeNamespace()
        {
            var chatty = await Silo.CreateGrainAsync<Chatty>(4);

            Silo.AddStreamProbe<ChatMessage>(Guid.Empty, "Wrong");

            const string msg = "Hello Chat";

            chatty.Invoking(p => p.SendChat(msg).Wait()).ShouldNotThrow();
        }

        [Fact]
        public async Task GrainIsSubscribed()
        {
            var stream = Silo.AddStreamProbe<ChatMessage>(Guid.Empty, null);

            await Silo.CreateGrainAsync<Listener>(1);

            stream.Subscribed.Should().Be(1);
        }

        [Fact]
        public async Task GrainReceives()
        {
            var stream = Silo.AddStreamProbe<ChatMessage>(Guid.Empty, null);

            var grain = await Silo.CreateGrainAsync<Listener>(1);

            await stream.OnNextAsync(new ChatMessage("Ding"));

            (await grain.ReceivedCount()).Should().Be(1);
        }
    }
}