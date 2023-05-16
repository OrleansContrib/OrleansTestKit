using OrleansTestKit.Tests.Grains;
using Xunit;

namespace Orleans.TestKit.Tests;

public class ContextConstructorGrainTests : TestKitBase
{
    [Fact]
    public async Task CanAccess_GrainContext_InConstructorAsync() => await Silo.CreateGrainAsync<ContextConstructorGrain>(0);
}
