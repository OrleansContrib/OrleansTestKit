using FluentAssertions;
using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests;

public class ActivationGrainTests : TestKitBase
{
    [Fact]
    public async Task ShouldActivateWithValidState()
    {
        // Arrange

        // Act
        var grain = await Silo.CreateGrainAsync<StatefulActivationGrain>(0);
        var value = await grain.GetActivationValue();

        // Assert
        value.Should().Be(123);
    }

    /// <summary>
    ///     This test demonstrates what happens when you try activating a grain with a state that does not have a a
    ///     parameterless constructor. The exception should help the user to find out what the problem is.
    /// </summary>
    [Fact]
    public async Task ShouldThrowNotSupportedException()
    {
        // Arrange

        // Act and Assert
        await Assert.ThrowsAsync<NotSupportedException>(
            async () => await Silo.CreateGrainAsync<StatefulUnsupportedActivationGrain>(0));
    }
}
