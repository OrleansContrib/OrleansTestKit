using System.Threading.Tasks;
using Orleans;
using OrleansUnitTestSilo.Tests.TestGrainInterfaces;

namespace OrleansUnitTestSilo.Tests.TestGrains
{
    public class PingGrain : Grain, IPing
    {
        public Task Ping()
        {
            var pong = GrainFactory.GetGrain<IPong>(22);

            return pong.Pong();
        }
    }
}