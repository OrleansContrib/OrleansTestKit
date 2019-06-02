using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using TestInterfaces;

namespace TestGrains
{
    public sealed class GreetingArchiveGrain : Grain<GreetingArchiveGrainState>, IGreetingArchiveGrain
    {
        public Task AddGreeting(string greeting)
        {
            this.State.Greetings.Add(greeting);
            return this.WriteStateAsync();
        }

        public Task<IEnumerable<string>> GetGreetings() =>
            Task.FromResult<IEnumerable<string>>(this.State.Greetings);

        public Task ResetGreetings() =>
            this.ClearStateAsync();
    }

    public sealed class GreetingArchiveGrainState
    {
        public List<string> Greetings { get; private set; } = new List<string>();
    }
}
