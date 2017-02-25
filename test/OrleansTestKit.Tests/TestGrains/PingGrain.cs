using System.Threading.Tasks;
using Orleans.TestKit.Tests.TestGrainInterfaces;

namespace Orleans.TestKit.Tests.TestGrains
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