using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace TestGrains
{
    public interface IUnknownGrainResolver : IGrainWithStringKey
    {
        Task<List<string>> GetResolvedUnknownGrainIdsAsync();

        Task CreateAndPingMultiple();
    }
}
