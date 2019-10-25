using System;
using System.ComponentModel;
using Orleans.TestKit.Storage;

namespace Orleans.TestKit
{
    public static class StorageExtensions
    {
        public static TState State<TState>(this TestKitSilo silo) where TState : class, new()
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            return silo.StorageManager.GetStorage<TState>().State;
        }

        public static TestStorageStats StorageStats(this TestKitSilo silo)
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            return silo.StorageManager.GetStorageStats();
        }
    }
}
