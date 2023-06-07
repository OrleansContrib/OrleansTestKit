using TestInterfaces;

namespace TestGrains;

public class DeactivationGrain : Grain, IDeactivationGrain
{
    public new Task DeactivateOnIdle()
    {
        base.DeactivateOnIdle();

        return Task.CompletedTask;
    }

    public new Task DelayDeactivation(TimeSpan timeSpan)
    {
        base.DelayDeactivation(timeSpan);

        return Task.CompletedTask;
    }
}
