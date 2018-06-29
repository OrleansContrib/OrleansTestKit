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
        public async Task SiloSayHelloTestAsync()
        {
            long id = new Random().Next();
            const string greeting = "Bonjour";

            IHello grain = await Silo.CreateGrainAsync<HelloGrain>(id);

            // This will create and call a Hello grain with specified 'id' in one of the test silos.
            string reply = await grain.SayHello(greeting);

            Assert.NotNull(reply);
            Assert.Equal($"You said: '{greeting}', I say: Hello!", reply);
        }

        [Fact]
        public void GrainActivation()
        {
            var grain = Silo.CreateGrain<LifecycleGrain>(new Random().Next());

            grain.ActivateCount.Should().Be(1);
        }

        [Fact]
        public async Task GrainActivationAsync()
        {
            var grain = await Silo.CreateGrainAsync<LifecycleGrain>(new Random().Next());

            grain.ActivateCount.Should().Be(1);
        }

        [Fact]
        public void SecondGrainCreated()
        {
            Silo.CreateGrain<LifecycleGrain>(new Random().Next());

            Silo.Invoking(s => s.CreateGrain<LifecycleGrain>(new Random().Next())).ShouldThrow<Exception>();
        }

        [Fact]
        public async Task SecondGrainCreatedAsync()
        {
            await Silo.CreateGrainAsync<LifecycleGrain>(new Random().Next());

            Func<Task> creatingSecondGrainAsync = async () => await Silo.CreateGrainAsync<LifecycleGrain>(new Random().Next());
            creatingSecondGrainAsync.ShouldThrow<Exception>();
        }

        [Fact]
        public void GrainDeactivation()
        {
            var grain = Silo.CreateGrain<LifecycleGrain>(new Random().Next());

            grain.DeactivateCount.Should().Be(0);

            Silo.Deactivate(grain);

            grain.DeactivateCount.Should().Be(1);
        }

        [Fact]
        public async Task GrainDeactivationAsync()
        {
            var grain = Silo.CreateGrain<LifecycleGrain>(new Random().Next());

            grain.DeactivateCount.Should().Be(0);

            await Silo.DeactivateAsync(grain);

            grain.DeactivateCount.Should().Be(1);
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
        public async Task IntegerKeyGrainAsync()
        {
            const int id = int.MaxValue;

            var grain = await Silo.CreateGrainAsync<IntegerKeyGrain>(id);

            var key = await grain.GetKey();

            key.Should().Be(id);
        }

        [Fact]
        public async Task IntegerCompoundKeyGrain()
        {
            const int id = int.MaxValue;
            var ext = "Thing";

            var grain = Silo.CreateGrain<IntegerCompoundKeyGrain>(id, ext);

            var key = await grain.GetKey();

            key.Item1.Should().Be(id);
            key.Item2.Should().Be(ext);
        }

        [Fact]
        public async Task IntegerCompoundKeyGrainAsync()
        {
            const int id = int.MaxValue;
            var ext = "Thing";

            var grain = await Silo.CreateGrainAsync<IntegerCompoundKeyGrain>(id, ext);

            var key = await grain.GetKey();

            key.Item1.Should().Be(id);
            key.Item2.Should().Be(ext);
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
        public async Task GuidKeyGrainAsync()
        {
            var id = Guid.NewGuid();

            var grain = await Silo.CreateGrainAsync<GuidKeyGrain>(id);

            var key = await grain.GetKey();

            key.Should().Be(id);
        }

        [Fact]
        public async Task GuidCompoundKeyGrain()
        {
            var id = Guid.NewGuid();
            var ext = "Thing";

            var grain = Silo.CreateGrain<GuidCompoundKeyGrain>(id, ext);

            var key = await grain.GetKey();

            key.Item1.Should().Be(id);
            key.Item2.Should().Be(ext);
        }

        [Fact]
        public async Task GuidCompoundKeyGrainAsync()
        {
            var id = Guid.NewGuid();
            var ext = "Thing";

            var grain = await Silo.CreateGrainAsync<GuidCompoundKeyGrain>(id, ext);

            var key = await grain.GetKey();

            key.Item1.Should().Be(id);
            key.Item2.Should().Be(ext);
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
        public async Task StringKeyGrainAsync()
        {
            const string id = "TestId";

            var grain = await Silo.CreateGrainAsync<StringKeyGrain>(id);

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
        public async Task StatefulIntegerKeyGrainAsync()
        {
            const int id = int.MaxValue;

            var grain = await Silo.CreateGrainAsync<StatefulIntegerKeyGrain>(id);

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
        public async Task StatefulGuidKeyGrainAsync()
        {
            var id = Guid.NewGuid();

            var grain = await Silo.CreateGrainAsync<StatefulGuidKeyGrain>(id);

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

        [Fact]
        public async Task StatefulStringKeyGrainAsync()
        {
            const string id = "TestId";

            var grain = await Silo.CreateGrainAsync<StatefulStringKeyGrain>(id);

            var key = await grain.GetKey();

            key.Should().Be(id);
        }
    }
}
