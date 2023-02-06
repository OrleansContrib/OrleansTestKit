using TestInterfaces;

namespace TestGrains;

public class HelloGrain : Grain, IHello
{
    public bool Deactivated { get; set; }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        Deactivated = true;

        return base.OnDeactivateAsync(reason, cancellationToken);
    }

    public Task<string> SayHello(string greeting) => Task.FromResult("You said: '" + greeting + "', I say: Hello!");
}
