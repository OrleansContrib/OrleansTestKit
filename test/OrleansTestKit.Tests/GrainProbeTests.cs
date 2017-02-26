using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Orleans.TestKit;
using TestGrains;
using TestInterfaces;
using Xunit;

namespace Orleans.TestKit.Tests
{
    public class GrainProbeTests : TestKitBase
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

            //There should not be an exception, since we are using loose grain generation
            grain.Invoking(p => p.Ping()).ShouldNotThrow();
        }

        [Fact]
        public void InvalidProbe()
        {
            IPing grain = Silo.CreateGrain<PingGrain>(1);

            //This uses the wrong id for the IPong since this is hard coded within PingGrain
            var pong = Silo.AddProbe<IPong>(0);

            grain.Invoking(p => p.Ping()).ShouldNotThrow();

            pong.Verify(p => p.Pong(), Times.Never);
        }

        [Fact]
        public void InvalidProbeType()
        {
            IPing grain = Silo.CreateGrain<PingGrain>(1);

            //This correct id, but a different grain type
            var pong = Silo.AddProbe<IPong2>(22);

            grain.Invoking(p => p.Ping()).ShouldNotThrow();

            pong.Verify(p => p.Pong2(), Times.Never);
        }

        [Fact]
        public async Task GuidKeyProbe()
        {
            var probe = Silo.AddProbe<IGuidKeyGrain>(Guid.NewGuid());

            var key = await probe.Object.GetKey();

            probe.Should().NotBeNull();
            key.Should().Be(Guid.Empty);
        }

        [Fact]
        public async Task IntKeyProbe()
        {
            var probe = Silo.AddProbe<IIntegerKeyGrain>(2);

            var key = await probe.Object.GetKey();

            probe.Should().NotBeNull();
            key.Should().Be(0);
        }

        [Fact]
        public async Task StringKeyProbe()
        {
            var probe = Silo.AddProbe<IStringKeyGrain>("ABC");

            var key = await probe.Object.GetKey();

            probe.Should().NotBeNull();
            key.Should().BeNull();
        }
    }
}
