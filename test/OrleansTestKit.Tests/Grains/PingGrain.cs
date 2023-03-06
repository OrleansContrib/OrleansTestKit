using TestInterfaces;

namespace TestGrains;

public class PingGrain : Grain, IPing
{
    public Task Ping()
    {
        var pong = GrainFactory.GetGrain<IPong>(22);

        return pong.Pong();
    }

    public Task PingCompound()
    {
        var pong = (IPongCompound)GrainFactory.GetGrain(typeof(IPongCompound),44, keyExtension: "Test");

        return pong.Pong();
    }
}
