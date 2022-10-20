using System.Threading;
using System.Threading.Tasks;
using Orleans;
using TestInterfaces;

namespace TestGrains
{
    public class HelloGrain : Grain, IHello
    {
        public bool Deactivated { get; set; }

        public Task<string> SayHello(string greeting) => Task.FromResult("You said: '" + greeting + "', I say: Hello!");

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            Deactivated = true;

            return base.OnDeactivateAsync(reason, cancellationToken);
        }
    }
}
