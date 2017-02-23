using System.Threading.Tasks;
using Orleans;
using Orleans.Core;

namespace OrleansNonSiloTesting.Storage
{
    internal sealed class TestStorage : IStorage
    {
        public TestStorageStats Stats { get; }

        public TestStorage()
        {
            Stats = new TestStorageStats();
        }

        public Task ClearStateAsync()
        {
            Stats.Clears++;

            return TaskDone.Done;
        }

        public Task WriteStateAsync()
        {
            Stats.Writes++;

            return TaskDone.Done;
        }

        public Task ReadStateAsync()
        {
            Stats.Reads++;

            return TaskDone.Done;
        }
    }
}