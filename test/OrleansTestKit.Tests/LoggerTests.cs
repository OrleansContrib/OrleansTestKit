using FluentAssertions;
using TestGrains;
using TestInterfaces;
using Xunit;
using Xunit.Abstractions;

namespace Orleans.TestKit.Tests
{
    public class LoggerTests : TestKitBase
    {
        private readonly ITestOutputHelper _output;

        public LoggerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ConsoleLog()
        {
            const string greeting = "Bonjour";

            IHello grain = Silo.CreateGrain<HelloGrain>(1);

            grain.Invoking(g => g.SayHello(greeting)).ShouldNotThrow();
        }

        [Fact]
        public void XUnitLog()
        {
            const string greeting = "Bonjour";

            IHello grain = Silo.CreateGrain<HelloGrain>(2);

            grain.Invoking(g => g.SayHello(greeting)).ShouldNotThrow();
        }
    }
}
