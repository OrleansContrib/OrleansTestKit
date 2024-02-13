using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TestInterfaces;
using Xunit;

namespace Orleans.TestKit.Tests.Tests;

public class ConcurrentGrainFactoryTests : TestKitBase
{
    private readonly IClusterClient _clusterClient;

    public ConcurrentGrainFactoryTests()
    {
        Silo.AddProbe(id => Mock.Of<IPong>());
        _clusterClient = Silo.ServiceProvider.GetRequiredService<IClusterClient>();
    }

    [Fact]
    public void ParallelInvoke_GrainFactory_DifferentKeys_Success()
    {
        Parallel.For(0, 100, i =>
        {
            _ = _clusterClient.GetGrain<IPong>(i);
        });
    }

    [Fact]
    public void ParallelInvoke_GrainFactory_SameKey_ReturnsSameGrain()
    {
        var results = Enumerable.Range(0, 100).AsParallel().Select(x => _clusterClient.GetGrain<IPong>(0)).ToArray();

        results.Distinct().Should().HaveCount(1);
    }
}
