using Moq;
using Orleans.Runtime;

namespace Orleans.TestKit;

/// <summary>
/// Grain context used for tests.
/// </summary>
public sealed class TestGrainActivationContext : IGrainContext
{
    public readonly Mock<IGrainContext> Mock = new Mock<IGrainContext>();

    /// <inheritdoc/>
    public ActivationId ActivationId { get; set; }

    /// <inheritdoc/>
    public IServiceProvider ActivationServices { get; set; } = default!;

    /// <inheritdoc/>
    public GrainAddress Address { get; set; } = default!;

    /// <inheritdoc/>
    public Task Deactivated { get; set; } = Task.CompletedTask;

    /// <inheritdoc/>
    public GrainId GrainId { get; set; } = default!;

    public IAddressable GrainIdentity { get; set; } = default!;

    /// <inheritdoc/>
    public object GrainInstance { get; set; } = default!;

    public void Migrate(Dictionary<string, object>? requestContext, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    /// <inheritdoc/>
    public GrainReference GrainReference { get; set; } = default!;

    /// <summary>
    /// Gets or sets the grain type used for this test context.
    /// </summary>
    public Type GrainType { get; set; } = default!;

    /// <inheritdoc/>
    public IGrainLifecycle ObservableLifecycle { get; set; } = default!;

    /// <inheritdoc/>
    public IWorkItemScheduler Scheduler { get; set; } = default!;

    /// <inheritdoc/>
    public void Activate(Dictionary<string, object> requestContext, CancellationToken? cancellationToken = null) => throw new NotImplementedException();

    /// <inheritdoc/>
    public void Deactivate(DeactivationReason deactivationReason, CancellationToken? cancellationToken = null)
    {
        Mock.Object.Deactivate(deactivationReason);
    }

    /// <inheritdoc/>
    public bool Equals(IGrainContext? other) => ReferenceEquals(this, other);

    /// <inheritdoc/>
    public TComponent GetComponent<TComponent>() where TComponent : class => throw new NotImplementedException();

    /// <inheritdoc/>
    public TTarget GetTarget<TTarget>() where TTarget : class => throw new NotImplementedException();

    /// <inheritdoc/>
    public void Migrate(Dictionary<string, object> requestContext, CancellationToken? cancellationToken = null) => throw new NotImplementedException();

    /// <inheritdoc/>
    public void ReceiveMessage(object message) => throw new NotImplementedException();

    public void Activate(Dictionary<string, object>? requestContext, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public void Deactivate(DeactivationReason deactivationReason, CancellationToken cancellationToken = new CancellationToken())
    {
        Mock.Object.Deactivate(deactivationReason, cancellationToken);
    }

    /// <inheritdoc/>
    public void Rehydrate(IRehydrationContext context) => throw new NotImplementedException();

    /// <inheritdoc/>
    public void SetComponent<TComponent>(TComponent value) where TComponent : class => throw new NotImplementedException();
}
