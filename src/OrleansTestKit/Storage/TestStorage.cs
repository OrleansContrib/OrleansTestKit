using System.Threading.Tasks;
using Orleans.Core;

namespace Orleans.TestKit.Storage
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
