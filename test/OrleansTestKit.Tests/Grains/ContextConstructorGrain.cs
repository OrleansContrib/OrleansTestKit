using FluentAssertions;
using Orleans.Runtime;

namespace OrleansTestKit.Tests.Grains;
public class ContextConstructorGrain : Grain, IGrainWithIntegerKey
{
    public ContextConstructorGrain(IGrainContext injectedContext)
    {
        injectedContext.Should().NotBeNull();

        var context = ((IGrainBase)this).GrainContext;

        context.Should().NotBeNull();

        context.Should().Be(injectedContext);

        context.GrainId.Should().NotBeNull();

        GrainFactory.Should().NotBeNull();
    }
}

// TODO -- re-architect service provider to use automocking solution instead of of rolling by hand to allow for bigger "units" of testing
public class LifecycleComponent : ILifecycleParticipant<IGrainLifecycle>
{
    private readonly IGrainContext _context;

    public LifecycleComponent(IGrainContext context) => _context = context;

    public bool IsInitialized { get; private set; }

    public void Participate(IGrainLifecycle observer) => _context.ObservableLifecycle.Subscribe(nameof(LifecycleComponent), GrainLifecycleStage.Activate - 1, PreActivateAsync);

    private Task PreActivateAsync(CancellationToken arg)
    {
        IsInitialized = true;
        return Task.CompletedTask;
    }
}
