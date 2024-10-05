using FluentAssertions;
using Moq;
using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests;

public class DependencyGrainTests : TestKitBase
{
    [Fact]
    public async Task GrainWithoutServices()
    {
        // Arrange
        var grain = await Silo.CreateGrainAsync<DependencyGrain>(Guid.NewGuid());

        // Act
        var unkeyedServiceValue = await grain.GetUnkeyedServiceValue();
        var firstKeyedServiceValue = await grain.GetFirstKeyedServiceValue();
        var secondKeyedServiceValue = await grain.GetSecondKeyedServiceValue();

        // Assert
        unkeyedServiceValue.Should().BeNull();
        firstKeyedServiceValue.Should().BeNull();
        secondKeyedServiceValue.Should().BeNull();
    }

    [Fact]
    public async Task GrainWithServices()
    {
        // Arrange
        var unkeyedService = new Mock<IDependency>(MockBehavior.Strict);
        unkeyedService.Setup(x => x.GetValue()).Returns("");
        Silo.ServiceProvider.AddServiceProbe(unkeyedService);

        var firstKeyedService = new Mock<IDependency>(MockBehavior.Strict);
        firstKeyedService.Setup(x => x.GetValue()).Returns("first");
        Silo.ServiceProvider.AddKeyedServiceProbe("first", firstKeyedService);

        var secondKeyedService = new Mock<IDependency>(MockBehavior.Strict);
        secondKeyedService.Setup(x => x.GetValue()).Returns("second");
        Silo.ServiceProvider.AddKeyedServiceProbe("second", secondKeyedService);

        var grain = await Silo.CreateGrainAsync<DependencyGrain>(Guid.NewGuid());

        // Act
        var unkeyedServiceValue = await grain.GetUnkeyedServiceValue();
        var firstKeyedServiceValue = await grain.GetFirstKeyedServiceValue();
        var secondKeyedServiceValue = await grain.GetSecondKeyedServiceValue();

        // Assert
        unkeyedServiceValue.Should().BeEmpty();
        unkeyedService.Verify(x => x.GetValue(), Times.Once);

        firstKeyedServiceValue.Should().Be("first");
        firstKeyedService.Verify(x => x.GetValue(), Times.Once);

        secondKeyedServiceValue.Should().Be("second");
        secondKeyedService.Verify(x => x.GetValue(), Times.Once);
    }
}
