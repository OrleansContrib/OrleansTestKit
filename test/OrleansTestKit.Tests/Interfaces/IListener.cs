namespace TestInterfaces;

public interface IListener : IGrainWithIntegerKey
{
    Task<int> ReceivedCount();
}
