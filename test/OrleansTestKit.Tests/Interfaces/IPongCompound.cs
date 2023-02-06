namespace TestInterfaces;

public interface IPongCompound : IGrainWithIntegerCompoundKey
{
    Task Pong();
}
