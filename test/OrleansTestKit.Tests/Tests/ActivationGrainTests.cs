using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests
{
    public class ActivationGrainTests : TestKitBase
    {
        [Fact]
        public async Task ShouldActivateWithValidState()
        {
            // Arrange


            // Act
            var grain = Silo.CreateGrain<StatefulActivationGrain>(0);
            var value = await grain.GetActivationValue();

            // Assert
            value.Should().Be(123);
        }
    }
}
