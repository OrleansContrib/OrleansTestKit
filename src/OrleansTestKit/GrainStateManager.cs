using System;
using System.Collections.Generic;

namespace Orleans.TestKit
{
    internal sealed class GrainStateManager
    {
        private readonly Dictionary<string, object> _states = new Dictionary<string, object>();

        public void AddState(Grain grain, object state)
        {
            if(state == null)
                throw new Exception("State can not be null");

            _states[grain.IdentityString] = state;
        }

        public TState GetState<TState>(Grain grain) where TState : class
        {
            object state;

            if (!_states.TryGetValue(grain.IdentityString, out state))
                throw new KeyNotFoundException($"No state found for grain {grain.IdentityString}");

            var typedState = state as TState;

            if(typedState == null)
                throw new InvalidCastException($"Grain state is not a {typeof(TState)}");

            return typedState;
        }
    }
}