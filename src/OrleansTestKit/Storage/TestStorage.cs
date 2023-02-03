using System;
using System.Threading.Tasks;
using Orleans.Core;
using Orleans.Runtime;

namespace Orleans.TestKit.Storage
{
    internal class TestStorage<TState> :
        IStorageStats,
        IStorage<TState>,
        IPersistentState<TState>
    {
        public TestStorage()
        {
            Stats = new TestStorageStats() { Reads = -1 };
            InitializeState();
        }

        public TestStorage(TState state) : this()
        {
            State = state;
        }
        public string Etag => throw new System.NotImplementedException();

        public virtual bool RecordExists { get; set; }

        public TState State { get; set; }

        public TestStorageStats Stats { get; }

        public Task ClearStateAsync()
        {
            InitializeState();
            Stats.Clears++;
            RecordExists = false;
            return Task.CompletedTask;
        }

        public Task ReadStateAsync()
        {
            Stats.Reads++;
            return Task.CompletedTask;
        }

        public Task WriteStateAsync()
        {
            Stats.Writes++;
            RecordExists = true;
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
