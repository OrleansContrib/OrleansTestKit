using System.Threading.Tasks;
using Orleans;

namespace TestInterfaces
{
    public interface IPong2 : IGrainWithIntegerKey
    {
        Task Pong2();
    }
}