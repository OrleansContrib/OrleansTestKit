using System.Threading.Tasks;

namespace Orleans.TestKit.Tests.TestGrainInterfaces
{
    public interface IChatty : IGrainWithIntegerKey
    {
        Task SendChat(string msg);
    }
}