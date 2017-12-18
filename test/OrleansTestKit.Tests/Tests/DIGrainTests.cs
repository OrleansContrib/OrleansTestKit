using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TestGrains;
using TestInterfaces;
using Xunit;

namespace Orleans.TestKit.Tests
{
    public class DIGrainTests : TestKitBase
    {
        [Fact]
        public void CreateGrainWithService()
        {
            var grain = Silo.CreateGrain<DIGrain>(Guid.NewGuid());

            grain.Service.Should().NotBeNull();
        }

        [Fact]
        public void SetupGrainService()
        {
            var mockSvc = new Mock<IDIService>();
            mockSvc.Setup(x => x.GetValue()).Returns(true);

            Silo.ServiceProvider.AddServiceProbe(mockSvc);
            var grain = Silo.CreateGrain<DIGrain>(Guid.NewGuid());

            grain.GetServiceValue().Should().BeTrue();
            mockSvc.Verify(x => x.GetValue(), Times.Once);
        }
    }
}
