using FluentAssertions;
using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests;

public class StrictStreamTests : TestKitBase
{
    public StrictStreamTests()
    {
        Silo.Options.StrictStreamProbes = true;
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
    public async Task IncorrectProbeId()
    {
        var chatty = await Silo.CreateGrainAsync<Chatty>(4);

        Silo.AddStreamProbe<ChatMessage>(Guid.NewGuid(), null);

        const string msg = "Hello Chat";

        chatty.Invoking(p => p.SendChat(msg).Wait()).Should().Throw<Exception>();
    }

    [Fact]
    public async Task IncorrectProbeNamespace()
    {
        var chatty = await Silo.CreateGrainAsync<Chatty>(4);

        Silo.AddStreamProbe<ChatMessage>(Guid.Empty, "Wrong");

        const string msg = "Hello Chat";

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
}
