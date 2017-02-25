using System.Threading.Tasks;

namespace Orleans.TestKit.Tests.TestGrainInterfaces
{
    public interface IPing : IGrainWithIntegerKey
    {
        Task Ping();
    }
}