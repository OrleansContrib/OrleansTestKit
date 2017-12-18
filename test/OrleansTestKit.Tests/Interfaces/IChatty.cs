using System.Threading.Tasks;
using Orleans;

namespace TestInterfaces
{
    public interface IChatty : IGrainWithIntegerKey
    {
        Task SendChat(string msg);
    }
}