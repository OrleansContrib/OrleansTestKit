using System;
using System.Threading;
using System.Threading.Tasks;
using Orleans;
using TestInterfaces;

namespace TestGrains
{
    public interface IStatefulUnsupportedActivationGrain : IGrainWithIntegerKey
    {
        Task<int> GetActivationValue();

        Task<int> GetStateValue();
    }
    public sealed class StatefulUnsupportedActivationGrain : Grain<StatefulNonSupportedActivationGrainState>, IStatefulUnsupportedActivationGrain
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

    public sealed class StatefulNonSupportedActivationGrainState
    {
        public StatefulNonSupportedActivationGrainState(int activationValue)
        {
            Value = activationValue;
        }

        public int Value { get; set; }
    }
}
