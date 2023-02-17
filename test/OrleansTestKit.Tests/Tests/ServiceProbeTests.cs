#if NSUBSTITUTE

using NSubstitute;

#else
using Moq;
#endif

using TestGrains;
using TestInterfaces;
using Xunit;

namespace Orleans.TestKit.Tests;

public class ServiceProbeTests : TestKitBase
{
    [Fact]
    public async Task SayHelloTestShouldPrintDate()
    {
        // Arrange
        const string greeting = "Bonjour";
        var date = DateTime.UtcNow.Date;

        var dateServiceMock = Silo.AddServiceProbe<IDateTimeService>();

#if NSUBSTITUTE
        dateServiceMock.GetCurrentDate().Returns(date);
#else
        dateServiceMock.Setup(i => i.GetCurrentDate())
                .ReturnsAsync(() => date);
#endif
        var grain = await Silo.CreateGrainAsync<HelloGrainWithServiceDependency>(10);

        // Act
        var reply = await grain.SayHello(greeting);

        // Assert
        Assert.NotNull(reply);
        Assert.Equal($"[{date}]: You said: '{greeting}', I say: Hello!", reply);

#if NSUBSTITUTE
        dateServiceMock.Received(1).GetCurrentDate();
#else
        dateServiceMock.Verify(i => i.GetCurrentDate(), Times.Once);
#endif
    }

    [Fact]
    public async Task SayHelloTestShouldPrintDateWhenServiceProvided()
    {
        // Arrange
        const string greeting = "Bonjour";
        var date = DateTime.UtcNow.Date;

#if NSUBSTITUTE
        var dateServiceMock = Substitute.For<IDateTimeService>();
        dateServiceMock.GetCurrentDate().Returns(date);

        Silo.AddService(dateServiceMock);
#else
        var dateServiceMock = new Mock<IDateTimeService>();
        dateServiceMock.Setup(i => i.GetCurrentDate())
            .ReturnsAsync(() => date);

        Silo.AddService(dateServiceMock.Object);

#endif

        var grain = await Silo.CreateGrainAsync<HelloGrainWithServiceDependency>(10);

        // Act
        var reply = await grain.SayHello(greeting);

        // Assert
        Assert.NotNull(reply);
        Assert.Equal($"[{date}]: You said: '{greeting}', I say: Hello!", reply);

#if NSUBSTITUTE
        dateServiceMock.Received(1).GetCurrentDate();
#else
        dateServiceMock.Verify(i => i.GetCurrentDate(), Times.Once);
#endif
    }

    [Fact]
    public async Task SayHelloTestShouldPrintDefaultDateWhenServiceProbeNotDefined()
    {
        // Arrange
        const string greeting = "Bonjour";

        var grain = await Silo.CreateGrainAsync<HelloGrainWithServiceDependency>(10);

        // Act
        var reply = await grain.SayHello(greeting);

        // Assert
        Assert.NotNull(reply);
        Assert.Equal($"[{default(DateTime).Date}]: You said: '{greeting}', I say: Hello!", reply);
    }
}
