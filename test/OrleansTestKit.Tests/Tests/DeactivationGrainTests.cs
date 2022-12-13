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

            var context = Silo.GetContextFromGrain(grain);
            // Assert
            Silo.VerifyRuntime(i => i.DeactivateOnIdle(context), Times.Once);
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
            Silo.VerifyRuntime(i => i.DelayDeactivation(context,timeSpan), Times.Once);
        }
    }
}
