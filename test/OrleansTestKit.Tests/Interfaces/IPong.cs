using System.Threading.Tasks;
using Orleans;

namespace TestInterfaces
{
    public interface IPong : IGrainWithIntegerKey
    {
        Task Pong();
    }
}