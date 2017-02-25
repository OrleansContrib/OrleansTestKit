using System.Threading.Tasks;

namespace Orleans.TestKit.Tests.TestGrainInterfaces
{
    public interface IListener : IGrainWithIntegerKey
    {
        Task<int> ReceivedCount();
    }
}