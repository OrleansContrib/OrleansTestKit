using System;
using System.Threading;
using System.Threading.Tasks;
using Orleans;
using TestInterfaces;

namespace TestGrains
{
    public interface IStatefulActivationGrain : IGrainWithIntegerKey
    {
        Task<int> GetActivationValue();

        Task<int> GetStateValue();
    }
    public sealed class StatefulActivationGrain : Grain<StatefulActivationGrainState>, IStatefulActivationGrain
    {
        private int _activationValue;

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _activationValue = this.State.Value;
            return Task.CompletedTask;
        }

        public Task<int> GetStateValue() => Task.FromResult(State.Value);

        public Task<int> GetActivationValue() => Task.FromResult(_activationValue);
    }

    public sealed class StatefulActivationGrainState
    {
        public int Value { get; set; } = 123;
    }
}
