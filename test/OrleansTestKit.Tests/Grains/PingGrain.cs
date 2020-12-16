using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using TestInterfaces;

namespace TestGrains
{
    public class PingGrain : Grain, IPing
    {

        public List<long> WhatsMyIdResults { get; } = new List<long>();

        public Task Ping()
        {
            var pong = GrainFactory.GetGrain<IPong>(22);

            return pong.Pong();
        }

        public Task PingCompound()
        {
            var pong = GrainFactory.GetGrain<IPongCompound>(44, keyExtension: "Test");

            return pong.Pong();
        }

        public async Task CreateAndPingMultiple()
        {
            var pongOne = GrainFactory.GetGrain<IPong>(1);
            var pongTwo = GrainFactory.GetGrain<IPong>(2);

            WhatsMyIdResults.Add(await pongOne.WhatsMyId());
            WhatsMyIdResults.Add(await pongTwo.WhatsMyId());
        }
    }
}
