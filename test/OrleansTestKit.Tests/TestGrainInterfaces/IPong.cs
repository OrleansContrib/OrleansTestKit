using System.Threading.Tasks;

namespace Orleans.TestKit.Tests.TestGrainInterfaces
{
    public interface IPong : IGrainWithIntegerKey
    {
        Task Pong();
    }
}