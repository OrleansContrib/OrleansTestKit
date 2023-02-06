using FluentAssertions;
using Moq;
using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests;

public class DIGrainTests : TestKitBase
{
    [Fact]
    public async Task CreateGrainWithService()
    {
        var grain = await Silo.CreateGrainAsync<DIGrain>(Guid.NewGuid());

        grain.Service.Should().NotBeNull();
    }

    [Fact]
    public async Task SetupGrainService()
    {
        var mockSvc = new Mock<IDIService>();
        mockSvc.Setup(x => x.GetValue()).Returns(true);

        Silo.ServiceProvider.AddServiceProbe(mockSvc);
        var grain = await Silo.CreateGrainAsync<DIGrain>(Guid.NewGuid());

        grain.GetServiceValue().Should().BeTrue();
        mockSvc.Verify(x => x.GetValue(), Times.Once);
    }
}
