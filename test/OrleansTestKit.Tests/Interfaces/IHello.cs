using System.Threading.Tasks;
using Orleans;

namespace TestInterfaces
{
    public interface IHello : IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }
}