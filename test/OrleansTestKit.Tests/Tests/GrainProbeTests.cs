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
            IPing grain = await Silo.CreateGrainAsync<PingGrain>(1);

            var pong = Silo.AddProbe<IPong>(22);

            await grain.Ping();

            pong.Verify(p => p.Pong(), Times.Once);
        }

        [Fact]
        public async Task SetupCompoundProbe()
        {
            IPing grain = await Silo.CreateGrainAsync<PingGrain>(1);

            var pong = Silo.AddProbe<IPongCompound>(44, keyExtension: "Test");

            await grain.PingCompound();

            pong.Verify(p => p.Pong(), Times.Once);
        }

        [Fact]
        public async Task MissingProbe()
        {
            IPing grain = await Silo.CreateGrainAsync<PingGrain>(1);

            //There should not be an exception, since we are using loose grain generation
            grain.Invoking(p => p.Ping()).Should().NotThrow();
        }

        [Fact]
        public async Task InvalidProbe()
        {
            IPing grain = await Silo.CreateGrainAsync<PingGrain>(1);

            //This uses the wrong id for the IPong since this is hard coded within PingGrain
            var pong = Silo.AddProbe<IPong>(0);

            grain.Invoking(p => p.Ping()).Should().NotThrow();

            pong.Verify(p => p.Pong(), Times.Never);
        }

        [Fact]
        public async Task InvalidProbeType()
        {
            IPing grain = await Silo.CreateGrainAsync<PingGrain>(1);

            //This correct id, but a different grain type
            var pong = Silo.AddProbe<IPong2>(22);

            grain.Invoking(p => p.Ping()).Should().NotThrow();

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
        public async Task GuidCompoundKeyProbe()
        {
            var probe = Silo.AddProbe<IGuidCompoundKeyGrain>(Guid.NewGuid(), keyExtension: "Test");

            var key = await probe.Object.GetKey();

            probe.Should().NotBeNull();
            key.Item1.Should().Be(Guid.Empty);
            key.Item2.Should().BeNull();
        }

        [Fact]
        public async Task IntCompoundKeyProbe()
        {
            var probe = Silo.AddProbe<IIntegerCompoundKeyGrain>(2, keyExtension: "Test");

            var key = await probe.Object.GetKey();

            probe.Should().NotBeNull();
            key.Item1.Should().Be(0);
            key.Item2.Should().BeNull();
        }

        [Fact]
        public async Task StringKeyProbe()
        {
            var probe = Silo.AddProbe<IStringKeyGrain>("ABC");

            var key = await probe.Object.GetKey();

            probe.Should().NotBeNull();
            key.Should().BeNull();
        }

        [Fact]
        public async Task FactoryProbe()
        {

            var pong = new Mock<IPong>();

            this.Silo.AddProbe<IPong>(identity => pong);

            var grain = await this.Silo.CreateGrainAsync<PingGrain>(1);

            await grain.Ping();

            pong.Verify(p => p.Pong(), Times.Once);
        }

        [Fact]
        public async Task ProbeWithClassPrefix()
        {
            var androidMock = Silo.AddProbe<IDevice>("Android", "TestGrains.DeviceAndroidGrain");
            var iosMock = Silo.AddProbe<IDevice>("IOS", "TestGrains.DeviceIosGrain");

            var managerGrain = await this.Silo.CreateGrainAsync<DeviceManagerGrain>(0);
            var androidGrain = await managerGrain.GetDeviceGrain("Android");
            var iosGrain = await managerGrain.GetDeviceGrain("IOS");

            androidGrain.Should().BeSameAs(androidMock.Object);
            iosGrain.Should().BeSameAs(iosMock.Object);
        }

        [Fact]
        public async Task ProbeWithClassPrefix2()
        {
            var androidMock = Silo.AddProbe<IDevice>("Android", "TestGrains.DeviceAndroidGrain");
            androidMock.Setup(o => o.GetDeviceType()).ReturnsAsync("Linux");
            var iosMock = Silo.AddProbe<IDevice>("IOS", "TestGrains.DeviceIosGrain");
            iosMock.Setup(o => o.GetDeviceType()).ReturnsAsync("BSD");

            var managerGrain = await this.Silo.CreateGrainAsync<DeviceManagerGrain>(0);
            var androidType = await managerGrain.GetDeviceType("Android");
            var iosType = await managerGrain.GetDeviceType("IOS");

            androidType.Should().Be("Linux");
            iosType.Should().Be("BSD");
        }
    }
}
