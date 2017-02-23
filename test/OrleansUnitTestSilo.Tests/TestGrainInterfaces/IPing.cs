using System.Threading.Tasks;
using Orleans;

namespace OrleansUnitTestSilo.Tests.TestGrainInterfaces
{
    public interface IPing : IGrainWithIntegerKey
    {
        Task Ping();
    }
}