using System;
using System.Threading.Tasks;
using OrleansNonSiloTesting;
using OrleansUnitTestSilo.Tests.TestGrainInterfaces;
using OrleansUnitTestSilo.Tests.TestGrains;
using Xunit;

namespace OrleansUnitTestSilo.Tests.Tests
{
    public class BasicGrainTests : UnitTestSiloBase
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
