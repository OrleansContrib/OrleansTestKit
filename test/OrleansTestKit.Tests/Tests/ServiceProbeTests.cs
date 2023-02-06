using Moq;
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
        dateServiceMock.Setup(i => i.GetCurrentDate())
                .ReturnsAsync(() => date);

        var grain = await Silo.CreateGrainAsync<HelloGrainWithServiceDependency>(10);

        // Act
        var reply = await grain.SayHello(greeting);

        // Assert
        Assert.NotNull(reply);
        Assert.Equal($"[{date}]: You said: '{greeting}', I say: Hello!", reply);

        dateServiceMock.Verify(i => i.GetCurrentDate(), Times.Once);
    }

    [Fact]
    public async Task SayHelloTestShouldPrintDateWhenServiceProvided()
    {
        // Arrange
        const string greeting = "Bonjour";
        var date = DateTime.UtcNow.Date;

        var dateServiceMock = new Mock<IDateTimeService>();
        dateServiceMock.Setup(i => i.GetCurrentDate())
            .ReturnsAsync(() => date);

        Silo.AddService(dateServiceMock.Object);

        var grain = await Silo.CreateGrainAsync<HelloGrainWithServiceDependency>(10);

        // Act
        var reply = await grain.SayHello(greeting);

        // Assert
        Assert.NotNull(reply);
        Assert.Equal($"[{date}]: You said: '{greeting}', I say: Hello!", reply);

        dateServiceMock.Verify(i => i.GetCurrentDate(), Times.Once);
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
