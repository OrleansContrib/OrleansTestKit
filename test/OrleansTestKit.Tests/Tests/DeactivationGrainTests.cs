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
            var grain = await Silo.CreateGrainAsync<DeactivationGrain>(0);

            // Act
            await grain.DeactivateOnIdle();

            // Assert
            Silo.VerifyRuntime(i => grain.DeactivateOnIdle(), Times.Once);
        }

        [Fact]
        public async Task ShouldCallDelayDeactivation()
        {
            // Arrange
            var grain = await Silo.CreateGrainAsync<DeactivationGrain>(0);
            var timeSpan = TimeSpan.FromSeconds(5);

            // Act
            await grain.DelayDeactivation(timeSpan);

            // Assert
            Silo.VerifyRuntime(i => grain.DelayDeactivation(timeSpan), Times.Once);
        }
    }
}
