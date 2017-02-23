using System.Threading.Tasks;
using Orleans;

namespace OrleansUnitTestSilo.Tests.TestGrainInterfaces
{
    public interface IListener : IGrainWithIntegerKey
    {
        Task<int> ReceivedCount();
    }
}