using System.Reflection;
using Moq;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans.TestKit.Streams;
using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests;

public class PersistentStreamWithinGrainStateTests : TestKitBase
{
    private readonly PersistentListenerStateWithHandle _stateWithHandle;

    private readonly TestStream<ChatMessage> _stream;

    public PersistentStreamWithinGrainStateTests()
    {
        _stateWithHandle = new PersistentListenerStateWithHandle();

        Silo.AddPersistentState(
            "listenerStateWithHandler", state: _stateWithHandle);

        _stream = Silo.AddStreamProbe<ChatMessage>(Guid.Empty, null, "Default");
    }

    [Fact]
    public async Task GivenEmptyHandlerInState_WhenGrainActivates_ThenResumeHandler()
    {
        //Arrange
        var onResumeCalled = false;
        var onAttachingObserver = new Action<IAsyncObserver<ChatMessage>>(obs => onResumeCalled = true);

        _stateWithHandle.ChatMessageStreamSubscriptionHandle =
            await _stream.AddEmptyStreamHandler(onAttachingObserver);

        //Check to see there is a handler registered
        var handles = await _stream.GetAllSubscriptionHandles();
        Assert.Equal(1, handles.Count);
        Assert.False(onResumeCalled);

        //Act
        var grain = await Silo.CreateGrainAsync<PersistentListenerWithHandleInState>(1);

        //Assert
        Assert.True(onResumeCalled);

        //Act
        await _stream.OnNextAsync(new ChatMessage("here's a message"));

        //Assert
        var receivedMessages = await grain.ReceivedCount();
        Assert.Equal(1, receivedMessages);
    }

    [Fact]
    public async Task WhenActivated_StoreStreamHandlerInState()
    {
        //Arrange + Act
        await Silo.CreateGrainAsync<PersistentListenerWithHandleInState>(1);

        //Assert
        Assert.Equal(1, _stream.Subscribed);
        Assert.NotNull(_stateWithHandle.ChatMessageStreamSubscriptionHandle);
        
        var stats = Silo.StorageManager.GetStorageStats("listenerStateWithHandler");
        Assert.Equal(1, stats.Writes);
    }
}
