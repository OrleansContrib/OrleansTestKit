#if NSUBSTITUTE

using NSubstitute;

#else
using Moq;
#endif

using FluentAssertions;
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

#if NSUBSTITUTE

    [Fact]
    public async Task SetupGrainService()
    {
        var mockSvc = Substitute.For<IDIService>();
        mockSvc.GetValue().Returns(true);
        Silo.ServiceProvider.AddServiceProbe(mockSvc);
        var grain = await Silo.CreateGrainAsync<DIGrain>(Guid.NewGuid());

        grain.GetServiceValue().Should().BeTrue();
        mockSvc.Received(1).GetValue();
    }

#else

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
#endif
}
