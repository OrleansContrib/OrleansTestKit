using TestInterfaces;

namespace TestGrains;

public class DeactivationGrainPoco : IGrainBase, IDeactivationGrain
{
    public IGrainContext GrainContext { get; }

    public new Task DeactivateOnIdle()
    {
        GrainBaseExtensions.DeactivateOnIdle(this);

        return Task.CompletedTask;
    }

    public new Task DelayDeactivation(TimeSpan timeSpan)
    {
        return Task.CompletedTask;
    }
}
