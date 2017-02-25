using System.Threading.Tasks;

namespace Orleans.TestKit.Tests.TestGrainInterfaces
{
    public interface IPong2 : IGrainWithIntegerKey
    {
        Task Pong2();
    }
}