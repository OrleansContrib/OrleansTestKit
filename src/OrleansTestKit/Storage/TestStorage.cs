using System;
using System.Threading.Tasks;
using Orleans.Core;

namespace Orleans.TestKit.Storage
{
    internal interface IStorageStats
    {
        TestStorageStats Stats { get; }
    }
    internal sealed class TestStorage<TState> : IStorageStats, IStorage<TState> where TState : new()
    {
        //Start with a negative 1 since orleans will automatically do a read when the grain is created;
        public TestStorageStats Stats { get; }

        public TState State { get; set; } = new TState();

        public string Etag => throw new System.NotImplementedException();

        public TestStorage()
        {
            Stats = new TestStorageStats() { Reads = -1 };
        }

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
