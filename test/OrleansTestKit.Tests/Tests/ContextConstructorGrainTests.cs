using Orleans.TestKit;
using OrleansTestKit.Tests.Grains;
using Xunit;

namespace OrleansTestKit.Tests.Tests;
public class ContextConstructorGrainTests : TestKitBase
{
    [Fact]
    public async Task CanAccess_GrainContext_InConstructorAsync() => await Silo.CreateGrainAsync<ContextConstructorGrain>(0);
}
