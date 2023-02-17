#if NSUBSTITUTE

using NSubstitute;

#else
using Moq;
#endif

using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests;

public class DeactivationGrainTests : TestKitBase
{
    [Fact]
    public async Task ShouldCallDeactivateOnIdle()
    {
        // Arrange
        var grain = await Silo.CreateGrainAsync<DeactivationGrain>(0);

        // Act
        await grain.DeactivateOnIdle();

        var context = Silo.GetContextFromGrain(grain);

        // Assert
#if NSUBSTITUTE
        Silo.GrainRuntime.Mock.Received(1).DeactivateOnIdle(context);
#else
        Silo.VerifyRuntime(i => i.DeactivateOnIdle(context), Times.Once);
#endif
    }

    [Fact]
    public async Task ShouldCallDelayDeactivation()
    {
        // Arrange
        var grain = await Silo.CreateGrainAsync<DeactivationGrain>(0);
        var timeSpan = TimeSpan.FromSeconds(5);

        // Act
        await grain.DelayDeactivation(timeSpan);

        var context = Silo.GetContextFromGrain(grain);

        // Assert
#if NSUBSTITUTE
        Silo.GrainRuntime.Mock.Received(1).DelayDeactivation(context, timeSpan);
#else
Silo.VerifyRuntime(i => i.DelayDeactivation(context, timeSpan), Times.Once);
#endif
    }
}
