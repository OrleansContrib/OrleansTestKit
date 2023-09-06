using Moq;
using Orleans.Core;
using Orleans.Runtime;
using Orleans.TestKit.Storage;
using Orleans.Timers;

namespace Orleans.TestKit;

/// <summary>
/// A test instance of the grain runtime that respects the TestKitSilo's storage manager
/// </summary>
public sealed class TestGrainRuntime : IGrainRuntime
{
    private readonly StorageManager _storageManager;

    internal TestGrainRuntime(IGrainFactory grainFactory, ITimerRegistry timerRegistry, IReminderRegistry reminderRegistry, IServiceProvider serviceProvider, StorageManager storageManager)
    {
        GrainFactory = grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
        TimerRegistry = timerRegistry ?? throw new ArgumentNullException(nameof(timerRegistry));
        ReminderRegistry = reminderRegistry ?? throw new ArgumentNullException(nameof(reminderRegistry));
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _storageManager = storageManager ?? throw new ArgumentNullException(nameof(storageManager));
    }

    /// <inheritdoc/>
    public IGrainFactory GrainFactory { get; }

    /// <summary>
    /// The underlying mock used for verification etc
    /// </summary>
    public Mock<IGrainRuntime> Mock { get; } = new Mock<IGrainRuntime>();

    /// <summary>
    /// The test reminder registry
    /// </summary>
    public IReminderRegistry ReminderRegistry { get; }

    /// <inheritdoc/>
    public IServiceProvider ServiceProvider { get; }

    /// <inheritdoc/>
    public SiloAddress SiloAddress => SiloAddress.Zero;

    /// <inheritdoc/>
    public string SiloIdentity => "TestSilo";

    /// <inheritdoc/>
    public ITimerRegistry TimerRegistry { get; }

    /// <inheritdoc/>
    public void DeactivateOnIdle(IGrainContext grain)
    {
        if (grain == null)
        {
            throw new ArgumentNullException(nameof(grain));
        }

        Mock.Object.DeactivateOnIdle(grain);
    }

    /// <inheritdoc/>
    public void DelayDeactivation(IGrainContext grain, TimeSpan timeSpan)
    {
        if (grain == null)
        {
            throw new ArgumentNullException(nameof(grain));
        }

        Mock.Object.DelayDeactivation(grain, timeSpan);
    }

    /// <inheritdoc/>
    public IStorage<TGrainState> GetStorage<TGrainState>(IGrainContext grainContext)
    {
        ArgumentNullException.ThrowIfNull(grainContext);

        // Getting storage from the IGrainContext either use the grain type
        // from the TestGrainActivationContext if possible otherwise
        // use GrainInstance.
        var grainType = grainContext is TestGrainActivationContext testContext
            ? testContext.GrainType
            : grainContext.GrainInstance?.GetType();

        ArgumentNullException.ThrowIfNull(grainType);

        return _storageManager.GetStorage<TGrainState>(grainType.FullName!);
    }
}
