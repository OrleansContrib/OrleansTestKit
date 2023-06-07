using Orleans.Runtime;
using Orleans.Streams;
using TestInterfaces;

namespace TestGrains;

public class PersistentListenerStateWithHandle
{
    public StreamSubscriptionHandle<ChatMessage>? ChatMessageStreamSubscriptionHandle { get; set; }

    public int ReceivedCount { get; set; }
}

public class PersistentListenerWithHandleInState : Grain, IListener
{
    private readonly IPersistentState<PersistentListenerStateWithHandle> _persistentState;

    public PersistentListenerWithHandleInState([PersistentState("listenerStateWithHandler")] IPersistentState<PersistentListenerStateWithHandle> persistentState)
    {
        _persistentState = persistentState;
    }

    private Func<ChatMessage, StreamSequenceToken, Task> ChatMessageHandler =>
        async (message, token) =>
        {
            _persistentState.State.ReceivedCount++;
            await _persistentState.WriteStateAsync();
        };

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (_persistentState.State.ChatMessageStreamSubscriptionHandle != null)
        {
            await _persistentState.State.ChatMessageStreamSubscriptionHandle.ResumeAsync(ChatMessageHandler);
        }
        else
        {
            var stream = this.GetStreamProvider("Default").GetStream<ChatMessage>(Guid.Empty);
            _persistentState.State.ChatMessageStreamSubscriptionHandle = await stream.SubscribeAsync(ChatMessageHandler);
            await _persistentState.WriteStateAsync();
        }

        await base.OnActivateAsync(cancellationToken);
    }

    public Task<int> ReceivedCount() => Task.FromResult(_persistentState.State.ReceivedCount);
}
