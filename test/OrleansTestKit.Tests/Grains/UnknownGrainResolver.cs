using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using TestInterfaces;

namespace TestGrains
{
    public class UnknownGrainResolver : Grain, IGrainWithStringKey
    {
        
        public List<string> ResolvedUnknownGrainIds { get; } = new List<string>();
        
        public async Task CreateAndPingMultiple()
        {
            var unknownGrainOne = GrainFactory.GetGrain<IUnknownGrain>("unknownGrainOne");
            var unknownGrainTwo = GrainFactory.GetGrain<IUnknownGrain>("unknownGrainTwo");

            ResolvedUnknownGrainIds.Add(await unknownGrainOne.WhatsMyId());
            ResolvedUnknownGrainIds.Add(await unknownGrainTwo.WhatsMyId());
        }
    }
}
