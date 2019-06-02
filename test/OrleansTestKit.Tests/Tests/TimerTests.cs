using System.Threading.Tasks;
using FluentAssertions;
using Orleans.TestKit.Storage;
using Orleans.TestKit.Timers;
using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests
{
    public class TimerTests : TestKitBase
    {
        [Fact]
        public async Task ShouldFireAllTimers()
        {
            // Arrange
            var grain = await Silo.CreateGrainAsync<HelloTimers>(0);

            // Act
            Silo.FireAllTimers();

            // Assert
            var state = Silo.State<HelloTimersState>();
            state.Timer0Fired.Should().BeTrue();
            state.Timer1Fired.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldFireFirstTimer()
        {
            // Arrange
            var grain = await Silo.CreateGrainAsync<HelloTimers>(0);

            // Act
            Silo.FireTimer(0);

            // Assert
            var state = Silo.State<HelloTimersState>();
            state.Timer0Fired.Should().BeTrue();
            state.Timer1Fired.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldFireSecondTimer()
        {
            // Arrange
            var grain = await Silo.CreateGrainAsync<HelloTimers>(0);

            // Act
            Silo.FireTimer(1);

            // Assert
            var state = Silo.State<HelloTimersState>();
            state.Timer0Fired.Should().BeFalse();
            state.Timer1Fired.Should().BeTrue();
        }
    }
}
