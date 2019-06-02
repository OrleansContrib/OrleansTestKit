using System;
using System.ComponentModel;
using Orleans.TestKit.Storage;

namespace Orleans.TestKit
{
    public static class StorageExtensions
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use the State extension method without passing the grain reference.")]
        public static TState State<TState>(this TestKitSilo silo, Grain<TState> grain) where TState : class, new() =>
            State<TState>(silo);

        public static TState State<TState>(this TestKitSilo silo) where TState : class, new() =>
            silo.StorageManager.GetStorage<TState>().State;

        public static TestStorageStats StorageStats(this TestKitSilo silo) =>
            silo.StorageManager.GetStorageStats();
    }
}
