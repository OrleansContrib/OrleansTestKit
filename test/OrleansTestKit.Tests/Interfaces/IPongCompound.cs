using System.Threading.Tasks;
using Orleans;

namespace TestInterfaces
{
    public interface IPongCompound : IGrainWithIntegerCompoundKey
    {
        Task Pong();
    }
}
