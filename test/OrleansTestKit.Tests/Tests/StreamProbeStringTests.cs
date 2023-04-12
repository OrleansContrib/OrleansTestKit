using FluentAssertions;
using FluentAssertions.Execution;
using Orleans.TestKit.Streams;
using Xunit;

namespace Orleans.TestKit.Tests;

public class StreamProbeStringTests : TestKitBase
{
    [Fact]
    public void AddStreamProbe_WithStringStreamId_Populates_Correctly()
    {
        var streamId = Guid.NewGuid().ToString();

        var stream = Silo.AddStreamProbe<object>(streamId);

        AssertStream(stream, "Default", typeof(object).Name, streamId);
    }

    [Fact]
    public void AddStreamProbe_WithStringStreamIdNamespace_Populates_Correctly()
    {
        var streamId = Guid.NewGuid().ToString();
        var ns = Guid.NewGuid().ToString();

        var stream = Silo.AddStreamProbe<object>(streamId, ns);

        AssertStream(stream, "Default", ns, streamId);
    }

    [Fact]
    public void AddStreamProbe_WithStringStreamIdNamespaceProvider_Populates_Correctly()
    {
        var streamId = Guid.NewGuid().ToString();
        var ns = Guid.NewGuid().ToString();
        var provider = Guid.NewGuid().ToString();

        var stream = Silo.AddStreamProbe<object>(streamId, ns, provider);

        AssertStream(stream, provider, ns, streamId);
    }

    private static void AssertStream(TestStream<object> stream, string provider, string streamNamespace, string streamId)
    {
        using var scope = new AssertionScope();

        stream.ProviderName.Should().Be(provider);
        stream.StreamId.GetNamespace().Should().Be(streamNamespace);
        stream.StreamId.GetKeyAsString().Should().Be(streamId);
    }
}
