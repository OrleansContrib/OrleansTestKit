using System;
using System.Collections;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using TestInterfaces;

namespace TestGrains
{
    public class PersistentListenerStateWithHandle
    {
        public StreamSubscriptionHandle<ChatMessage> ChatMessageStreamSubscriptionHandle = null;
        public int ReceivedCount = 0;
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

        public Task<int> ReceivedCount() => Task.FromResult(_persistentState.State.ReceivedCount);

        public override async  Task OnActivateAsync()
        {
            if (_persistentState.State.ChatMessageStreamSubscriptionHandle != null)
            {
                await _persistentState.State.ChatMessageStreamSubscriptionHandle.ResumeAsync(ChatMessageHandler);
            }
            else
            {
                var stream = GetStreamProvider("Default").GetStream<ChatMessage>(Guid.Empty, null);
                _persistentState.State.ChatMessageStreamSubscriptionHandle = await stream.SubscribeAsync(ChatMessageHandler);
                await _persistentState.WriteStateAsync();
            }

            await base.OnActivateAsync();
        }
    }
}
