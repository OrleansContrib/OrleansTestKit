using FluentAssertions;
using Orleans.TestKit.Timers;
using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests;

public class TimerTests : TestKitBase
{
    [Fact]
    public async Task ShouldFirstGrainTimerAsync()
    {
        // Arrange
        var grain = await Silo.CreateGrainAsync<HelloTimers>(0);

        // Act
        await Silo.FireTimerAsync(HelloTimers.GrainTimer0);

        // Assert
        var state = Silo.State<HelloTimers, HelloTimersState>();
        state.GrainTimer0Fired.Should().BeTrue();
        state.GrainTimer0Cancelled.Should().BeFalse();
    }

    [Fact]
        public async Task ShouldCancelFirstGrainTimerAsync()
    {
        // Arrange
        var grain = await Silo.CreateGrainAsync<HelloTimers>(0);

        // Act
        _ = Silo.FireTimerAsync(HelloTimers.GrainTimer0);
        await Task.Delay(100);
        grain._grainTimer0.Dispose();
        await Task.Delay(100);

        // Assert
        var state = Silo.State<HelloTimers, HelloTimersState>();
        state.GrainTimer0Fired.Should().BeTrue();
        state.GrainTimer0Cancelled.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldFireAllTimersAsync()
    {
        // Arrange
        var grain = await Silo.CreateGrainAsync<HelloTimers>(0);

        // Act
        await Silo.FireAllTimersAsync();

        // Assert
        var state = Silo.State<HelloTimers, HelloTimersState>();
        state.Timer0Fired.Should().BeTrue();
        state.Timer1Fired.Should().BeTrue();
        state.Timer2Fired.Should().BeTrue();
        state.GrainTimer0Fired.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldFireAllTimersRepeatedlyAsync()
    {
        // Arrange
        var grain = await Silo.CreateGrainAsync<HelloTimers>(0);

        // Act
        await Silo.FireAllTimersAsync();
        await Silo.FireAllTimersAsync();

        // Assert
        var state = Silo.State<HelloTimers, HelloTimersState>();
        state.Timer0Fired.Should().BeTrue();
        state.Timer1Fired.Should().BeTrue();
        state.Timer2Fired.Should().BeTrue();
        state.GrainTimer0Fired.Should().BeTrue();

    }

    [Fact]
    public async Task ShouldFireFirstTimerAsync()
    {
        // Arrange
        var grain = await Silo.CreateGrainAsync<HelloTimers>(0);

        // Act
        await Silo.FireTimerAsync(0);

        // Assert
        var state = Silo.State<HelloTimers, HelloTimersState>();
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
        var state = Silo.State<HelloTimers, HelloTimersState>();
        state.Timer0Fired.Should().BeFalse();
        state.Timer1Fired.Should().BeTrue();
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
}
