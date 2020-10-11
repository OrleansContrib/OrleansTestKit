using System;
using System.Threading.Tasks;
using Orleans.Core;

namespace Orleans.TestKit.Storage
{
    internal class TestStorage<TState> :
        IStorageStats,
        IStorage<TState>
    {
        public TestStorageStats Stats { get; }

        public TState State { get; set; }

        public string Etag => throw new System.NotImplementedException();

        public virtual bool RecordExists => throw new NotImplementedException();

        public TestStorage()
        {
            Stats = new TestStorageStats() {Reads = -1};
            InitializeState();
        }

        public Task ClearStateAsync()
        {
            InitializeState();
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

        private void InitializeState()
        {
            if (!typeof(TState).IsValueType && typeof(TState).GetConstructor(Type.EmptyTypes) == null)
            {
                throw new NotSupportedException(
                    $"No parameterless constructor defined for {typeof(TState).Name}. This is currently not supported");
            }

            State = Activator.CreateInstance<TState>();
        }
    }
}
