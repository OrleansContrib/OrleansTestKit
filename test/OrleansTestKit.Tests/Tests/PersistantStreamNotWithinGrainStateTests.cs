using System.Reflection;
using Moq;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans.TestKit.Streams;
using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests;

public class PersistantStreamNotWithinGrainStateTests : TestKitBase
{
    private readonly PersistentListenerStateWithoutHandle _stateWithoutHandle;

    private readonly TestStream<ChatMessage> _stream;

    public PersistantStreamNotWithinGrainStateTests()
    {
        _stateWithoutHandle = new PersistentListenerStateWithoutHandle();

        Silo.AddPersistentState(
            "listenerStateWithoutHandler", state: _stateWithoutHandle);

        _stream = Silo.AddStreamProbe<ChatMessage>(Guid.Empty, null, "Default");
    }

    [Fact]
    public async Task GivenHandler_WhenGrainActivates_ThenResumeHandler()
    {
        //Arrange
        var onResumeCalled = false;
        var onAttachingObserver = new Action<IAsyncObserver<ChatMessage>>(obs => onResumeCalled = true);

        //Adds a handler, but doesn't store to state
        await _stream.AddEmptyStreamHandler(onAttachingObserver);

        //Check to see there is a handler registered
        var handles = await _stream.GetAllSubscriptionHandles();
        Assert.Equal(1, handles.Count);
        Assert.False(onResumeCalled);

        //Act
        var grain = await Silo.CreateGrainAsync<PersistentListenerWithoutHandleInState>(1);

        //Assert
        Assert.True(onResumeCalled);

        //Act
        await _stream.OnNextAsync(new ChatMessage("here's a message!"));

        //Assert
        var receivedMessages = await grain.ReceivedCount();
        Assert.Equal(1, receivedMessages);
    }

    [Fact]
    public async Task GivenNoHandler_WhenActivated_ThenSubscribeToStream()
    {
        //Arrange + Act
        await Silo.CreateGrainAsync<PersistentListenerWithoutHandleInState>(1);

        //Assert
        Assert.Equal(1, _stream.Subscribed);

        var streamHandles = await _stream.GetAllSubscriptionHandles();
        Assert.Equal(1, streamHandles.Count);
    }
}
