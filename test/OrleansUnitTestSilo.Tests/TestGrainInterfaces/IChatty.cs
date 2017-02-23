using System.Threading.Tasks;
using Orleans;

namespace OrleansUnitTestSilo.Tests.TestGrainInterfaces
{
    public interface IChatty : IGrainWithIntegerKey
    {
        Task SendChat(string msg);
    }
}