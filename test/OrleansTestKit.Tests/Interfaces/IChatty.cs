using System.Threading.Tasks;
using Orleans;

namespace TestInterfaces
{
    public interface IChatty : IGrainWithIntegerKey
    {
        Task SendChat(string msg);

        Task Subscribe();

        Task<(string Message, int Id)> GetMessage();
    }
}
