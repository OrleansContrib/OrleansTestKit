using FluentAssertions;
using Orleans.Runtime;
using Orleans.TestKit.Tests.Grains;
using Xunit;

namespace Orleans.TestKit.Tests.Tests;
public class GrainContextTests : TestKitBase
{

    [Fact]
    public async Task Same_WhenMatches_IdAndType()
    {
        var context = Silo.GetOrAddGrainContext<GrainContextGrain>(Guid.NewGuid());

        var grain = await Silo.CreateGrainAsync<GrainContextGrain>(context.GrainId.Key);

        context.Should().Be(grain.Context);
    }

    [Fact]
    public async Task Different_When_TypeMismatch()
    {
        var context = Silo.GetOrAddGrainContext<GrainContextGrain>(Guid.NewGuid());

        var grain = await Silo.CreateGrainAsync<GrainContextGrain2>(context.GrainId.Key);

        context.Should().NotBe(grain.Context);
    }

    [Fact]
    public async Task Different_When_IdMismatch()
    {
        var context = Silo.GetOrAddGrainContext<GrainContextGrain>(Guid.NewGuid());

        var grain = await Silo.CreateGrainAsync<GrainContextGrain2>(Guid.NewGuid());

        context.Should().NotBe(grain.Context);
    }

    [Fact]
    public async Task Different_When_MockIsSupplied()
    {
        var mockContext = Silo.AddServiceProbe<IGrainContext>();

        var id = GrainIdKeyExtensions.CreateGuidKey(Guid.NewGuid());
        var grainId = GrainId.Create(new GrainType(id), id);

        mockContext.Setup(x => x.GrainId).Returns(grainId);

        var grain = await Silo.CreateGrainAsync<GrainContextGrain2>(Guid.NewGuid());

        mockContext.Object.Should().NotBe(grain.Context);
    }

    [Fact]
    public void Populated_WithGuidId()
    {
        var expected = Guid.NewGuid();
        var context = Silo.GetOrAddGrainContext<GrainContextGuidGrain>(expected);
        context.GrainId.GetGuidKey().Should().Be(expected);
    }

    [Fact]
    public void Populated_WithIntegerId()
    {
        var expected = 1234L;
        var context = Silo.GetOrAddGrainContext<GrainContextIntegerGrain>(expected);
        context.GrainId.GetIntegerKey().Should().Be(expected);
    }

    [Fact]
    public void Populated_WithStringId()
    {
        var expected = "asdf";
        var context = Silo.GetOrAddGrainContext<GrainContextStringGrain>(expected);
        context.GrainId.Key.ToString().Should().Be(expected);
    }

    [Fact]
    public void Populated_WithIntegerId_AndExtension()
    {
        var expected = Guid.NewGuid();
        var expectedExtension = "asdf";
        var context = Silo.GetOrAddGrainContext<GrainContextGuidCompoundGrain>(expected, expectedExtension);
        context.GrainId.TryGetGuidKey(out var id, out var ext).Should().BeTrue();

        id.Should().Be(expected);
        ext.Should().Be(expectedExtension);
    }

    [Fact]
    public void Populated_WithGuidId_AndExtension()
    {
        var expected = 1234L;
        var expectedExtension = "asdf";
        var context = Silo.GetOrAddGrainContext<GrainContextIntegerCompoundGrain>(expected, expectedExtension);
        context.GrainId.TryGetIntegerKey(out var id, out var ext).Should().BeTrue();

        id.Should().Be(expected);
        ext.Should().Be(expectedExtension);
    }
}
