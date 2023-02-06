namespace TestInterfaces;

public interface IChatty : IGrainWithIntegerKey
{
    Task<(string Message, int Id)> GetMessage();

    Task SendChat(string msg);

    Task SendChatBatch(string[] msgs);

    Task Subscribe();
}
