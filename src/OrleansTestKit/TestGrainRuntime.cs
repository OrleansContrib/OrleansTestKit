#if NSUBSTITUTE

using NSubstitute;

#else
using Moq;
#endif

using Orleans.Core;
using Orleans.Runtime;
using Orleans.TestKit.Storage;
using Orleans.Timers;

namespace Orleans.TestKit;

public sealed class TestGrainRuntime : IGrainRuntime
{
    private readonly StorageManager _storageManager;

    public TestGrainRuntime(IGrainFactory grainFactory, ITimerRegistry timerRegistry, IReminderRegistry reminderRegistry, IServiceProvider serviceProvider, StorageManager storageManager)
    {
        GrainFactory = grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
        TimerRegistry = timerRegistry ?? throw new ArgumentNullException(nameof(timerRegistry));
        ReminderRegistry = reminderRegistry ?? throw new ArgumentNullException(nameof(reminderRegistry));
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _storageManager = storageManager ?? throw new ArgumentNullException(nameof(storageManager));
    }

    public IGrainFactory GrainFactory { get; }

#if NSUBSTITUTE

    public IGrainRuntime Mock { get; } = Substitute.For<IGrainRuntime>();

#else
    public Mock<IGrainRuntime> Mock { get; } = new Mock<IGrainRuntime>();

#endif

    public IReminderRegistry ReminderRegistry { get; }

    public string ServiceId => "TestService";

    public IServiceProvider ServiceProvider { get; }

    public SiloAddress SiloAddress => SiloAddress.Zero;

    public string SiloIdentity => "TestSilo";

    public ITimerRegistry TimerRegistry { get; }

    public void DeactivateOnIdle(IGrainContext grain)
    {
        if (grain == null)
        {
            throw new ArgumentNullException(nameof(grain));
        }

#if NSUBSTITUTE
        Mock.DeactivateOnIdle(grain);
#else
        Mock.Object.DeactivateOnIdle(grain);
#endif
    }

    public void DelayDeactivation(IGrainContext grain, TimeSpan timeSpan)
    {
        if (grain == null)
        {
            throw new ArgumentNullException(nameof(grain));
        }
#if NSUBSTITUTE
        Mock.DelayDeactivation(grain, timeSpan);
#else
        Mock.Object.DelayDeactivation(grain, timeSpan);
#endif
    }

    public IStorage<TGrainState> GetStorage<TGrainState>(IGrainContext grain) =>
        _storageManager.GetStorage<TGrainState>();
}
