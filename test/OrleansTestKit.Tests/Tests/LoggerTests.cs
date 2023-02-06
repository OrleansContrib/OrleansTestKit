using FluentAssertions;
using TestGrains;
using TestInterfaces;
using Xunit;
using Xunit.Abstractions;

namespace Orleans.TestKit.Tests;

public class LoggerTests : TestKitBase
{
    private readonly ITestOutputHelper _output;

    public LoggerTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task ConsoleLog()
    {
        const string greeting = "Bonjour";

        IHello grain = await Silo.CreateGrainAsync<HelloGrain>(1);

        await grain.Invoking(g => g.SayHello(greeting)).Should().NotThrowAsync();
    }

    [Fact]
    public async Task XUnitLog()
    {
        const string greeting = "Bonjour";

        IHello grain = await Silo.CreateGrainAsync<HelloGrain>(2);

        await grain.Invoking(g => g.SayHello(greeting)).Should().NotThrowAsync();
    }
}
