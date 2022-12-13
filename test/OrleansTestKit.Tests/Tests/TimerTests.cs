using System;
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
        public async Task ShouldFireAllTimersAsync()
        {
            // Arrange
            var grain = await Silo.CreateGrainAsync<HelloTimers>(0);

            // Act
            await Silo.FireAllTimersAsync();

            // Assert
            var state = Silo.State<HelloTimersState>();
            state.Timer0Fired.Should().BeTrue();
            state.Timer1Fired.Should().BeTrue();
        }



        [Fact]
        public async Task ShouldFireFirstTimerAsync()
        {
            // Arrange
            var grain = await Silo.CreateGrainAsync<HelloTimers>(0);

            // Act
            await Silo.FireTimerAsync(0);

            // Assert
            var state = Silo.State<HelloTimersState>();
            state.Timer0Fired.Should().BeTrue();
            state.Timer1Fired.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldFireSecondTimerAsync()
        {
            // Arrange
            var grain = await Silo.CreateGrainAsync<HelloTimers>(0);

            // Act
            await Silo.FireTimerAsync(1);

            // Assert
            var state = Silo.State<HelloTimersState>();
            state.Timer0Fired.Should().BeFalse();
            state.Timer1Fired.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldRegisterSecretTimerAsync()
        {
            //Arrange
            var grain = await Silo.CreateGrainAsync<HelloTimers>(0);
            var initialActiveTimers = Silo.TimerRegistry.NumberOfActiveTimers;

            //Act
            await grain.RegisterSecretTimer();

            //Assert
            Assert.Equal(initialActiveTimers + 1, Silo.TimerRegistry.NumberOfActiveTimers);
        }

        [Fact]
        public async Task ShouldNotCountDisposedTimersAsActive()
        {
            //Arrange
            var grain = (Grain)await Silo.CreateGrainAsync<HelloTimers>(0);
            var initialActiveTimers = Silo.TimerRegistry.NumberOfActiveTimers;

            var newTimer = Silo.TimerRegistry.RegisterTimer(((IGrainBase)grain).GrainContext, _ => Task.CompletedTask, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            Assert.Equal(initialActiveTimers + 1, Silo.TimerRegistry.NumberOfActiveTimers);

            //Act
            newTimer.Dispose();

            //Assert
            //Back to the original count
            Assert.Equal(initialActiveTimers, Silo.TimerRegistry.NumberOfActiveTimers);

        }
    }
}
