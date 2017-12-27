using System;
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

        public override async Task OnActivateAsync()
        {
            _activationValue = this.State.Value;
        }

        public Task<int> GetStateValue() => Task.FromResult(State.Value);

        public Task<int> GetActivationValue() => Task.FromResult(_activationValue);
    }

    public sealed class StatefulActivationGrainState
    {
        public int Value { get; set; } = 123;
    }
}