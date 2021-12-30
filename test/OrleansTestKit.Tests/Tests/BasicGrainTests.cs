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

            IHello grain = await Silo.CreateGrainAsync<HelloGrain>(id);

            // This will create and call a Hello grain with specified 'id' in one of the test silos.
            string reply = await grain.SayHello(greeting);

            Assert.NotNull(reply);
            Assert.Equal($"You said: '{greeting}', I say: Hello!", reply);
        }

        [Fact]
        public async Task GrainActivation()
        {
            var grain = await Silo.CreateGrainAsync<LifecycleGrain>(new Random().Next());

            grain.ActivateCount.Should().Be(1);
        }

        [Fact]
        public async Task SecondGrainCreated()
        {
            await Silo.CreateGrainAsync<LifecycleGrain>(new Random().Next());

            Func<Task> creatingSecondGrainAsync = async () => await Silo.CreateGrainAsync<LifecycleGrain>(new Random().Next());
            await creatingSecondGrainAsync.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task GrainDeactivation()
        {
            var grain = await Silo.CreateGrainAsync<LifecycleGrain>(new Random().Next());

            grain.DeactivateCount.Should().Be(0);

            await Silo.DeactivateAsync(grain);

            grain.DeactivateCount.Should().Be(1);
        }

        [Fact]
        public async Task IntegerKeyGrain()
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

            var grain = await Silo.CreateGrainAsync<IntegerCompoundKeyGrain>(id, ext);

            var key = await grain.GetKey();

            key.Item1.Should().Be(id);
            key.Item2.Should().Be(ext);
        }

        [Fact]
        public async Task GuidKeyGrain()
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

            var grain = await Silo.CreateGrainAsync<GuidCompoundKeyGrain>(id, ext);

            var key = await grain.GetKey();

            key.Item1.Should().Be(id);
            key.Item2.Should().Be(ext);
        }

        [Fact]
        public async Task StringKeyGrain()
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

            var grain = await Silo.CreateGrainAsync<StatefulIntegerKeyGrain>(id);

            var key = await grain.GetKey();

            key.Should().Be(id);
        }

        [Fact]
        public async Task StatefulGuidKeyGrain()
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

            var grain = await Silo.CreateGrainAsync<StatefulStringKeyGrain>(id);

            var key = await grain.GetKey();

            key.Should().Be(id);
        }
    }
}
