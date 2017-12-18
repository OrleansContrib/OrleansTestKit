using System.Threading.Tasks;
using Orleans;

namespace TestInterfaces
{
    public interface IListener : IGrainWithIntegerKey
    {
        Task<int> ReceivedCount();
    }
}