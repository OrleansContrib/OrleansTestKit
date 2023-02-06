namespace TestGrains;

public interface IStatefulUnsupportedActivationGrain : IGrainWithIntegerKey
{
    Task<int> GetActivationValue();

    Task<int> GetStateValue();
}

public sealed class StatefulNonSupportedActivationGrainState
{
    public StatefulNonSupportedActivationGrainState(int activationValue)
    {
        Value = activationValue;
    }

    public int Value { get; set; }
}

public sealed class StatefulUnsupportedActivationGrain : Grain<StatefulNonSupportedActivationGrainState>, IStatefulUnsupportedActivationGrain
{
    private int _activationValue;

    public Task<int> GetActivationValue() => Task.FromResult(_activationValue);

    public Task<int> GetStateValue() => Task.FromResult(State.Value);

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _activationValue = this.State.Value;
        return Task.CompletedTask;
    }
}
