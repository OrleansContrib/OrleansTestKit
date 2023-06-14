using FluentAssertions;
using Orleans.Runtime;
using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests;

public class GrainIdTests : TestKitBase
{
    [Fact]
    public async Task CreatedGrainShouldHaveCorrectGrainType()
    {
        var grain = await Silo.CreateGrainAsync<HelloGrain>(0);
        GrainId id = grain.GetGrainId();

        id.Type.Should().Be(GrainType.Create("hello"));
    }

    [Fact]
    public async Task CreatedGrainWithAliasShouldHaveCorrectGrainType()
    {
        var grain = await Silo.CreateGrainAsync<AliasGrain>(0);
        GrainId id = grain.GetGrainId();

        id.Type.Should().Be(GrainType.Create("special-alias"));
    }
}
