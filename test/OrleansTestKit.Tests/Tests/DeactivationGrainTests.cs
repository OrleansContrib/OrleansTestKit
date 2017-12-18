using System;
using System.Threading.Tasks;
using Moq;
using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests
{
    public class DeactivationGrainTests : TestKitBase
    {
        [Fact]
        public async Task ShouldCallDeactivateOnIdle()
        {
            // Arrange
            var grain = Silo.CreateGrain<DeactivationGrain>(0);

            // Act
            await grain.DeactivateOnIdle();

            // Assert
            Silo.VerifyRuntime(i => i.DeactivateOnIdle(grain), Times.Once);
        }

        [Fact]
        public async Task ShouldCallDelayDeactivation()
        {
            // Arrange
            var grain = Silo.CreateGrain<DeactivationGrain>(0);
            var timeSpan = TimeSpan.FromSeconds(5);

            // Act
            await grain.DelayDeactivation(timeSpan);

            // Assert
            Silo.VerifyRuntime(i => i.DelayDeactivation(grain, timeSpan), Times.Once);
        }
    }
}
