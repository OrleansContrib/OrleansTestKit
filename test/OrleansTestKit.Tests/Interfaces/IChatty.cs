using TestGrains;

namespace TestInterfaces;

public interface IChatty : IGrainWithIntegerKey
{
    Task<ChattyMessage> GetMessage();

    Task SendChat(string msg);

    Task SendChatBatch(string[] msgs);

    Task Subscribe();
    Task SubscribeBatch();
}
