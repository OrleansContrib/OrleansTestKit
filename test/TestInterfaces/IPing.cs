using System.Threading.Tasks;
using Orleans;

namespace TestInterfaces
{
    public interface IPing : IGrainWithIntegerKey
    {
        Task Ping();
    }
}