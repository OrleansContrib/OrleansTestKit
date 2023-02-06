namespace TestGrains;

public interface IStatefulActivationGrain : IGrainWithIntegerKey
{
    Task<int> GetActivationValue();

    Task<int> GetStateValue();
}

public sealed class StatefulActivationGrain : Grain<StatefulActivationGrainState>, IStatefulActivationGrain
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

public sealed class StatefulActivationGrainState
{
    public int Value { get; set; } = 123;
}
