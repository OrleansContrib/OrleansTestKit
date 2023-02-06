namespace TestInterfaces;

public interface IPing : IGrainWithIntegerKey
{
    Task Ping();

    Task PingCompound();
}
