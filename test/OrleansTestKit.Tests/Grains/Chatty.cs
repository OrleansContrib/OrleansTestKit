using Orleans.Streams;
using TestInterfaces;

namespace TestGrains;

public readonly record struct ChattyMessage(string Message, int Id);

public class Chatty : Grain, IChatty
{
    private ChattyMessage _recievedMessage;

    private StreamSubscriptionHandle<ChattyMessage>? _subscription;

    public Task<ChattyMessage> GetMessage()
    {
        return Task.FromResult(_recievedMessage);
    }

    protected IStreamProvider Provider => this.GetStreamProvider("Default");
    protected IAsyncStream<ChattyMessage> ChattyStream => Provider.GetStream<ChattyMessage>(Guid.Empty);

    public async Task SendChat(string msg)
    {
        var stream = Provider.GetStream<ChatMessage>(Guid.Empty);

        await stream.OnNextAsync(new ChatMessage(msg));
    }

    public async Task SendChatBatch(params string[] msgs)
    {
        var stream = Provider.GetStream<ChatMessage>(Guid.Empty);

        await stream.OnNextBatchAsync(msgs.Select(chat => new ChatMessage(chat)));
    }

    public async Task Subscribe()
    {
        _subscription = await ChattyStream.SubscribeAsync(async (item, _) =>
        {
            _recievedMessage = item;
            if ("goodbye".Equals(_recievedMessage.Message, StringComparison.OrdinalIgnoreCase))
            {
                if (_subscription != null)
                {
                    await _subscription.UnsubscribeAsync();
                    _subscription = null;
                }
            }
        });
    }

    public async Task SubscribeBatch()
    {
        _subscription = await ChattyStream.SubscribeAsync(async (items) =>
        {
            _recievedMessage = items.Last().Item;
            if ("goodbye".Equals(_recievedMessage.Message, StringComparison.OrdinalIgnoreCase))
            {
                if (_subscription != null)
                {
                    await _subscription.UnsubscribeAsync();
                    _subscription = null;
                }
            }
        });
    }
}
