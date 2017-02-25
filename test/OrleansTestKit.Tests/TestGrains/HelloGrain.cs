using System.Threading.Tasks;
using Orleans.TestKit.Tests.TestGrainInterfaces;

namespace Orleans.TestKit.Tests.TestGrains
{
    public class HelloGrain : Grain, IHello
    {
        Task<string> IHello.SayHello(string greeting)
        {
            return Task.FromResult("You said: '" + greeting + "', I say: Hello!");
        }
    }
}