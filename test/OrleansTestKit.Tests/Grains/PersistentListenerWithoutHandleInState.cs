﻿using Orleans.Runtime;
using Orleans.Streams;
using TestInterfaces;

namespace TestGrains;

public class PersistentListenerStateWithoutHandle
{
    public int ReceivedCount { get; set; }
}

public class PersistentListenerWithoutHandleInState : Grain, IListener
{
    private readonly IPersistentState<PersistentListenerStateWithoutHandle> _persistentState;

    public PersistentListenerWithoutHandleInState([PersistentState("listenerStateWithoutHandler")] IPersistentState<PersistentListenerStateWithoutHandle> persistentState)
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
        var stream = this.GetStreamProvider("Default").GetStream<ChatMessage>(Guid.Empty);
        var handlers = await stream.GetAllSubscriptionHandles();

        var chatMessageHandle = handlers.FirstOrDefault();

        if (chatMessageHandle != null)
        {
            await chatMessageHandle.ResumeAsync(ChatMessageHandler);
        }
        else
        {
            await stream.SubscribeAsync(ChatMessageHandler);
        }

        await base.OnActivateAsync(cancellationToken);
    }

    public Task<int> ReceivedCount() => Task.FromResult(_persistentState.State.ReceivedCount);
}
