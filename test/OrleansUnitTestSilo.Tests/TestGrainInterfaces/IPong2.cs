using System.Threading.Tasks;
using Orleans;

namespace OrleansUnitTestSilo.Tests.TestGrainInterfaces
{
    public interface IPong2 : IGrainWithIntegerKey
    {
        Task Pong2();
    }
}