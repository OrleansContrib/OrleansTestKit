namespace TestInterfaces;

public interface IPong : IGrainWithIntegerKey
{
    Task Pong();
}
