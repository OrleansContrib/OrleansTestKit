using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests
{
    public class StorageTests : TestKitBase
    {
        /// <summary>This test demonstrates how the grain is initialized with a default state object.</summary>
        [Fact]
        public async Task GreetingArchiveGrain_GetGreetings_DefaultInitialState()
        {
            // Arrange
            const long id = 1000;

            var grain = await this.Silo.CreateGrainAsync<GreetingArchiveGrain>(id);

            // Act
            var greetings = (await grain.GetGreetings()).ToList();

            // Assert
            greetings.Should().BeEmpty();
        }

        /// <summary>
        ///     This test demonstrates how to customize the state object before the grain is initialized using the
        ///     instance returned by the <see cref="StorageExtensions.State{TState}(TestKitSilo)"/> extension method.
        /// </summary>
        [Fact]
        public async Task GreetingArchiveGrain_GetGreetings_CustomInitialState()
        {
            // Arrange
            const long id = 2000;
            const string greeting = "Hola";

            var state = this.Silo.State<GreetingArchiveGrainState>();
            state.Greetings.Add(greeting);

            var grain = await this.Silo.CreateGrainAsync<GreetingArchiveGrain>(id);

            // Act
            var greetings = (await grain.GetGreetings()).ToList();

            // Assert
            greetings.Should().Equal(greeting);
        }

        /// <summary>
        ///     This test demonstrates how to assert changes occurred to the state object using the instance returned by
        ///     the <see cref="StorageExtensions.State{TState}(TestKitSilo)"/> extension method.
        /// </summary>
        [Fact]
        public async Task GreetingArchiveGrain_AddGreeting_StateChanged()
        {
            // Arrange
            const long id = 3000;
            const string greeting1 = "Bonjour";
            const string greeting2 = "Hei";

            var grain = await this.Silo.CreateGrainAsync<GreetingArchiveGrain>(id);

            // Act
            await grain.AddGreeting(greeting1);
            await grain.AddGreeting(greeting2);

            var greetings = (await grain.GetGreetings()).ToList();

            // Assert
            var state = this.Silo.State<GreetingArchiveGrainState>();
            state.Greetings.Should().Equal(greeting1, greeting2);

            greetings.Should().Equal(greeting1, greeting2);
        }

        /// <summary>
        ///     This test demonstrates how to assert a `WriteStateAsync` call occurred using the counters returned by the
        ///     <see cref="StorageExtensions.StorageStats(TestKitSilo)"/> extension method.
        /// </summary>
        [Fact]
        public async Task GreetingArchiveGrain_AddGreeting_StatsChanged()
        {
            // Arrange
            const long id = 4000;
            const string greeting1 = "Salve";
            const string greeting2 = "Konnichiwa";

            var grain = await this.Silo.CreateGrainAsync<GreetingArchiveGrain>(id);

            // Act
            await grain.AddGreeting(greeting1);
            await grain.AddGreeting(greeting2);

            var greetings = (await grain.GetGreetings()).ToList();

            // Assert
            var stats = this.Silo.StorageStats();
            stats.Clears.Should().Be(0);
            stats.Reads.Should().Be(0);
            stats.Writes.Should().Be(2);

            greetings.Should().Equal(greeting1, greeting2);
        }

        /// <summary>
        ///     This test demonstrates how to assert changes occurred to the state object using the instance returned by
        ///     the <see cref="StorageExtensions.State{TState}(TestKitSilo)"/> extension method.
        /// </summary>
        [Fact]
        public async Task GreetingArchiveGrain_ResetGreetings_StateChanged()
        {
            // Arrange
            const long id = 5000;
            const string greeting = "Olá";

            var state = this.Silo.State<GreetingArchiveGrainState>();
            state.Greetings.Add(greeting);

            var grain = await this.Silo.CreateGrainAsync<GreetingArchiveGrain>(id);

            // Act
            await grain.ResetGreetings();

            var greetings = (await grain.GetGreetings()).ToList();

            // Assert
            state = this.Silo.State<GreetingArchiveGrainState>();
            state.Greetings.Should().BeEmpty();

            greetings.Should().BeEmpty();
        }

        /// <summary>
        ///     This test demonstrates how to assert a `ClearStateAsync` call occurred using the counters returned by the
        ///     <see cref="StorageExtensions.StorageStats(TestKitSilo)"/> extension method.
        /// </summary>
        [Fact]
        public async Task GreetingArchiveGrain_ResetGreetings_StatsChanged()
        {
            // Arrange
            const long id = 6000;
            const string greeting = "Hallo";

            var state = this.Silo.State<GreetingArchiveGrainState>();
            state.Greetings.Add(greeting);

            var grain = await this.Silo.CreateGrainAsync<GreetingArchiveGrain>(id);

            // Act
            await grain.ResetGreetings();

            var greetings = (await grain.GetGreetings()).ToList();

            // Assert
            var stats = this.Silo.StorageStats();
            stats.Clears.Should().Be(1);
            stats.Reads.Should().Be(0);
            stats.Writes.Should().Be(0);

            greetings.Should().BeEmpty();
        }
    }
}
