using System.Threading.Tasks;

namespace Orleans.TestKit.Tests.TestGrainInterfaces
{
    public interface IHello : IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }
}