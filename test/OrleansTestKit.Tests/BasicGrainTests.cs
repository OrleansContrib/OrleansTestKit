using System;
using System.Threading.Tasks;
using FluentAssertions;
using TestGrains;
using TestInterfaces;
using Xunit;

namespace Orleans.TestKit.Tests
{
    public class BasicGrainTests : TestKitBase
    {
        [Fact]
        public async Task SiloSayHelloTest()
        {
            long id = new Random().Next();
            const string greeting = "Bonjour";

            IHello grain = Silo.CreateGrain<HelloGrain>(id);

            // This will create and call a Hello grain with specified 'id' in one of the test silos.
            string reply = await grain.SayHello(greeting);

            Assert.NotNull(reply);
            Assert.Equal($"You said: '{greeting}', I say: Hello!", reply);
        }

        [Fact]
        public void GrainActivation()
        {
            var grain = Silo.CreateGrain<LifecycleGrain>(new Random().Next());

            grain.IsActivated.Should().BeTrue();
        }

        [Fact]
        public void SecondGrainCreated()
        {
            Silo.CreateGrain<LifecycleGrain>(new Random().Next());

            Silo.Invoking(s=>s.CreateGrain<LifecycleGrain>(new Random().Next())).ShouldThrow<Exception>();
        }

        [Fact]
        public void GrainDeactivation()
        {
            var grain = Silo.CreateGrain<LifecycleGrain>(new Random().Next());

            grain.IsDeactivated.Should().BeFalse();

            Silo.Deactivate(grain);

            grain.IsDeactivated.Should().BeTrue();
        }

        [Fact]
        public async Task IntegerKeyGrain()
        {
            const int id = int.MaxValue;

            var grain = Silo.CreateGrain<IntegerKeyGrain>(id);

            var key = await grain.GetKey();

            key.Should().Be(id);
        }

        [Fact]
        public async Task GuidKeyGrain()
        {
            var id = Guid.NewGuid();

            var grain = Silo.CreateGrain<GuidKeyGrain>(id);

            var key = await grain.GetKey();

            key.Should().Be(id);
        }

        [Fact]
        public async Task StringKeyGrain()
        {
            const string id = "TestId";

            var grain = Silo.CreateGrain<StringKeyGrain>(id);

            var key = await grain.GetKey();

            key.Should().Be(id);
        }

        [Fact]
        public async Task StatefulIntegerKeyGrain()
        {
            const int id = int.MaxValue;

            var grain = Silo.CreateGrain<StatefulIntegerKeyGrain>(id);

            var key = await grain.GetKey();

            key.Should().Be(id);
        }

        [Fact]
        public async Task StatefulGuidKeyGrain()
        {
            var id = Guid.NewGuid();

            var grain = Silo.CreateGrain<StatefulGuidKeyGrain>(id);

            var key = await grain.GetKey();

            key.Should().Be(id);
        }

        [Fact]
        public async Task StatefulStringKeyGrain()
        {
            const string id = "TestId";

            var grain = Silo.CreateGrain<StatefulStringKeyGrain>(id);

            var key = await grain.GetKey();

            key.Should().Be(id);
        }
    }
}