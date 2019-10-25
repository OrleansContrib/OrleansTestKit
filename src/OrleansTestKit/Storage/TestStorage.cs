using System.Threading.Tasks;
using Orleans.Core;

namespace Orleans.TestKit.Storage
{
    internal sealed class TestStorage<TState> :
        IStorageStats,
        IStorage<TState>
        where TState : new()
    {
        public TestStorageStats Stats { get; }

        public TState State { get; set; } = new TState();

        public string Etag => throw new System.NotImplementedException();

        public TestStorage() =>
            Stats = new TestStorageStats() { Reads = -1 };

        public Task ClearStateAsync()
        {
            State = new TState();
            Stats.Clears++;
            return Task.CompletedTask;
        }

        public Task WriteStateAsync()
        {
            Stats.Writes++;
            return Task.CompletedTask;
        }

        public Task ReadStateAsync()
        {
            Stats.Reads++;
            return Task.CompletedTask;
        }
    }
}
