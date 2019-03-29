using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests
{
    public sealed class LegacyLoggerTests : TestKitBase
    {
        private readonly Mock<ILoggerFactory> loggerFactory;
        private readonly Mock<ILogger> defaultLogger;
        private readonly Mock<ILogger> customLogger;

        public LegacyLoggerTests()
        {
            this.defaultLogger = new Mock<ILogger>(MockBehavior.Strict);
            this.defaultLogger.Setup(mock => mock.IsEnabled(It.IsAny<LogLevel>())).Returns<LogLevel>(logLevel => logLevel == LogLevel.Information);

            this.customLogger = new Mock<ILogger>(MockBehavior.Strict);
            this.customLogger.Setup(mock => mock.IsEnabled(It.IsAny<LogLevel>())).Returns<LogLevel>(logLevel => logLevel == LogLevel.Information);

            this.loggerFactory = new Mock<ILoggerFactory>(MockBehavior.Strict);
            this.loggerFactory.Setup(mock =>  mock.CreateLogger("TestGrains.LegacyLoggerGrain")).Returns(this.defaultLogger.Object);
            this.loggerFactory.Setup(mock => mock.CreateLogger("Custom")).Returns(this.customLogger.Object);
        }

        [Fact]
        public async Task LegacyLoggerGrain_WriteToDefaultLog()
        {
            // Arrange
            this.defaultLogger.Setup(mock => mock.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<object>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()));
            this.Silo.AddService(this.loggerFactory.Object);

            var grainId = new Random().Next();
            var grain = await this.Silo.CreateGrainAsync<LegacyLoggerGrain>(grainId);

            // Act
            await grain.WriteToDefaultLog("Hello, world!");

            // Assert
            this.defaultLogger.Verify(mock => mock.Log(
                LogLevel.Information,
                0,
                It.Is<FormattedLogValues>(param => "Hello, world!".Equals(param.Single().Value)),
                null,
                It.IsAny<Func<object, Exception, string>>()));
        }

        [Fact]
        public async Task LegacyLoggerGrain_WriteToCustomLog()
        {
            // Arrange
            this.customLogger.Setup(mock => mock.Log<object>(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<object>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()));
            this.Silo.AddService(this.loggerFactory.Object);

            var grainId = new Random().Next();
            var grain = await this.Silo.CreateGrainAsync<LegacyLoggerGrain>(grainId);

            // Act
            await grain.WriteToCustomLog("Hello, Daniel!");

            // Assert
            this.customLogger.Verify(mock => mock.Log(
                LogLevel.Information,
                0,
                It.Is<FormattedLogValues>(param => "Hello, Daniel!".Equals(param.Single().Value)),
                null,
                It.IsAny<Func<object, Exception, string>>()));
        }
    }
}
