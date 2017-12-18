using System;
using System.Collections.Generic;
using System.Linq;
using Orleans.Core;

namespace Orleans.TestKit.Storage
{
    public sealed class StorageManager
    {
        private object _storage;

        public IStorage<TState> GetStorage<TState>() where TState : new()
        {
            if (_storage == null)
            {
                _storage = new TestStorage<TState>();
            }

            return _storage as IStorage<TState>;
        }

        public TestStorageStats GetStorageStats()
        {
            //There should only be one state in here since there is only 1 grain under test
            var stats = _storage as IStorageStats;

            return stats?.Stats;
        }
    }
}