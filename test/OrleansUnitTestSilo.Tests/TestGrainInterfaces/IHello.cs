using System.Threading.Tasks;
using Orleans;

namespace OrleansUnitTestSilo.Tests.TestGrainInterfaces
{
    public interface IHello : IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }
}