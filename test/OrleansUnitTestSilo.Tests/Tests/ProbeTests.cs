using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using OrleansNonSiloTesting;
using OrleansUnitTestSilo.Tests.TestGrainInterfaces;
using OrleansUnitTestSilo.Tests.TestGrains;
using Xunit;

namespace OrleansUnitTestSilo.Tests.Tests
{
    public class ProbeTests : UnitTestSiloBase
    {
        [Fact]
        public async Task SetupProbe()
        {
            IPing grain = Silo.CreateGrain<PingGrain>(1);

            var pong = Silo.AddProbe<IPong>(22);

            await grain.Ping();

            pong.Verify(p => p.Pong(), Times.Once);
        }

        [Fact]
        public void MissingProbe()
        {
            IPing grain = Silo.CreateGrain<PingGrain>(1);

            grain.Invoking(p => p.Ping()).ShouldThrowExactly<Exception>();
        }

        [Fact]
        public void InvalidProbe()
        {
            IPing grain = Silo.CreateGrain<PingGrain>(1);

            //This uses the wrong id for the IPong since this is hard coded within PingGrain
            var pong = Silo.AddProbe<IPong>(0);

            grain.Invoking(p => p.Ping()).ShouldThrowExactly<Exception>();

            pong.Verify(p => p.Pong(), Times.Never);
        }

        [Fact]
        public void InvalidProbeType()
        {
            IPing grain = Silo.CreateGrain<PingGrain>(1);

            //This uses the wrong id for the IPong since this is hard coded within PingGrain
            var pong = Silo.AddProbe<IPong2>(22);

            grain.Invoking(p => p.Ping()).ShouldThrowExactly<Exception>();

            pong.Verify(p => p.Pong2(), Times.Never);
        }
    }
}
