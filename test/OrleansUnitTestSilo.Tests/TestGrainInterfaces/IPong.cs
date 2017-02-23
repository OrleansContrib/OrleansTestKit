using System.Threading.Tasks;
using Orleans;

namespace OrleansUnitTestSilo.Tests.TestGrainInterfaces
{
    public interface IPong : IGrainWithIntegerKey
    {
        Task Pong();
    }
}