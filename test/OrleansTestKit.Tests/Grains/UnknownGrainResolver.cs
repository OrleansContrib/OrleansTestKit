using TestInterfaces;

namespace TestGrains;

public class UnknownGrainResolver : Grain, IUnknownGrainResolver
{
    private List<string> _resolvedIds = new List<string>();

    public async Task CreateAndPingMultiple()
    {
        var unknownGrainOne = GrainFactory.GetGrain<IUnknownGrain>("unknownGrainOne");
        var unknownGrainTwo = GrainFactory.GetGrain<IUnknownGrain>("unknownGrainTwo");

        _resolvedIds.Add(await unknownGrainOne.WhatsMyId());
        _resolvedIds.Add(await unknownGrainTwo.WhatsMyId());
    }

    public Task<List<string>> GetResolvedUnknownGrainIdsAsync() => Task.FromResult(_resolvedIds);
}
