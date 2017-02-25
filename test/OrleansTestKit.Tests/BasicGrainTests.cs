using System;
using System.Threading.Tasks;
using TestGrains;
using TestInterfaces;
using Xunit;

namespace Orleans.TestKit.Tests
{
    public class BasicGrainTests : TestKitBase
    {
        [Fact]
        public async Task SiloSayHelloTest()
        {
            long id = new Random().Next();
            const string greeting = "Bonjour";

            IHello grain = Silo.CreateGrain<HelloGrain>(id);

            // This will create and call a Hello grain with specified 'id' in one of the test silos.
            string reply = await grain.SayHello(greeting);

            Assert.NotNull(reply);
            Assert.Equal($"You said: '{greeting}', I say: Hello!", reply);
        }
    }
}
