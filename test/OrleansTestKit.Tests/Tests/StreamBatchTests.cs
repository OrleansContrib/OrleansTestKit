using FluentAssertions;
using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests;

public class StreamBatchTests : TestKitBase
{
    [Fact]
    public async Task AddNonReferenceTypeStreamProbe()
    {
        var stream = Silo.AddStreamProbe<ChattyMessage>(Guid.Empty, null);

        var chatty = await Silo.CreateGrainAsync<Chatty>(4);
        await chatty.SubscribeBatch();

        const string msg = "Hello Chat";
        const int id = 2;
        await stream.OnNextBatchAsync(new ChattyMessage[] { new(msg, id) });

        stream.Sends.Should().Be(1);
        stream.VerifySendBatch();

        var message = await chatty.GetMessage();
        Assert.NotEqual(default, message);
        Assert.Equal(msg, message.Message);
        Assert.Equal(id, message.Id);
    }

    [Fact]
    public async Task GrainGetAllSubscriptionHandles()
    {
        var stream = Silo.AddStreamProbe<ChattyMessage>(Guid.Empty, null);

        var chatty = await Silo.CreateGrainAsync<Chatty>(4);
        await chatty.SubscribeBatch();
        var handlers = await stream.GetAllSubscriptionHandles();

        handlers.Count.Should().Be(1);
        stream.Subscribed.Should().Be(1);

        await handlers[0].UnsubscribeAsync();

        handlers.Count.Should().Be(1);
        stream.Subscribed.Should().Be(0);
    }
}
