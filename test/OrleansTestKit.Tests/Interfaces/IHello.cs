namespace TestInterfaces;

public interface IHello : IGrainWithIntegerKey
{
    Task<string> SayHello(string greeting);
}
