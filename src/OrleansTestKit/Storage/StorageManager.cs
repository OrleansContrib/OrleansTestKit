using System;
using System.Diagnostics.CodeAnalysis;
using Orleans.Core;

namespace Orleans.TestKit.Storage
{
    public sealed class StorageManager
    {
        private readonly TestKitOptions _options;

        private object _storage;

        public StorageManager(TestKitOptions options) =>
            _options = options ?? throw new ArgumentNullException(nameof(options));

        public IStorage<TState> GetStorage<TState>()
        {
            if (_storage == null)
            {
                _storage = _options.StorageFactory?.Invoke(typeof(TState)) ?? new TestStorage<TState>();
            }

            return _storage as IStorage<TState>;
        }

        public TestStorageStats StorageStats
        {
            get
            {
                //There should only be one state in here since there is only 1 grain under test
                var stats = _storage as IStorageStats;
                return stats?.Stats;
            }
        }
    }
}
