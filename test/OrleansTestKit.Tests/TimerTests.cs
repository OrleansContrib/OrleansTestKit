using FluentAssertions;
using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests
{
    public class TimerTests : TestKitBase
    {
        [Fact]
        public void ShouldFireAllTimers()
        {
            // Arrange
            var grain = Silo.CreateGrain<HelloTimers>(0);

            // Act
            Silo.FireAllTimers();

            // Assert
            var state = Silo.State<HelloTimersState>(grain);
            state.Timer0Fired.Should().BeTrue();
            state.Timer1Fired.Should().BeTrue();
        }

        [Fact]
        public void ShouldFireFirstTimer()
        {
            // Arrange
            var grain = Silo.CreateGrain<HelloTimers>(0);

            // Act
            Silo.FireTimer(0);

            // Assert
            var state = Silo.State<HelloTimersState>(grain);
            state.Timer0Fired.Should().BeTrue();
            state.Timer1Fired.Should().BeFalse();
        }

        [Fact]
        public void ShouldFireSecondTimer()
        {
            // Arrange
            var grain = Silo.CreateGrain<HelloTimers>(0);

            // Act
            Silo.FireTimer(1);

            // Assert
            var state = Silo.State<HelloTimersState>(grain);
            state.Timer0Fired.Should().BeFalse();
            state.Timer1Fired.Should().BeTrue();
        }
    }
}