using Orleans;
using Orleans.TestKit;
using Orleans.TestKit.Storage;

namespace Orleans.TestKit.Storage
{
    public static class StorageExtensions
    {
        public static TestStorageStats StorageStats(this TestKitSilo silo) =>
            silo.StorageManager.GetStorageStats();

        public static TState State<TState>(this TestKitSilo silo, Grain<TState> grain) where TState : class, new() =>
            silo.StorageManager.GetStorage<TState>().State;

        public static TState State<TState>(this TestKitSilo silo) where TState : class, new() =>
            silo.StorageManager.GetStorage<TState>().State;
    }
}