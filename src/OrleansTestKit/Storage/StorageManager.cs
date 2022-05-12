﻿using System;
using System.Collections.Generic;
using System.Linq;
using Orleans.Core;

namespace Orleans.TestKit.Storage
{
    public sealed class StorageManager
    {
        private readonly TestKitOptions _options;

        private readonly Dictionary<string, object> _storages = new();

        public StorageManager(TestKitOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            StateAttributeFactoryMapper = new TestPersistentStateAttributeToFactoryMapper(this);
        }

        internal TestPersistentStateAttributeToFactoryMapper StateAttributeFactoryMapper { get; }

        public IStorage<TState> GetGrainStorage<TGrain, TState>() where TGrain : Grain<TState>
            => GetStorage<TState>(typeof(TGrain).FullName);

        public IStorage<TState> GetStorage<TState>(string stateName)
        {
            if (string.IsNullOrWhiteSpace(stateName))
            {
                foreach (var kvp in _storages)
                {
                    if (kvp.Value is TestStorage<TState> typedStorage)
                    {
                        return typedStorage;
                    }
                }

                throw new InvalidOperationException($"Unable to find any storage with type '{typeof(TState).FullName}'");
            }

            if (_storages.TryGetValue(stateName, out var storage) is false)
            {
                storage = _storages[stateName] = _options.StorageFactory?.Invoke(typeof(TState)) ?? new TestStorage<TState>();
            }

            return storage as IStorage<TState>;
        }

        public TestStorageStats GetStorageStats<TGrain, TState>() where TGrain : Grain<TState>
            => GetStorageStats(typeof(TGrain).FullName);

        public TestStorageStats GetStorageStats(string stateName)
        {
            var normalisedStateName = stateName ?? "Default";

            if (_storages.TryGetValue(normalisedStateName, out var storage))
            {
                var stats = storage as IStorageStats;
                return stats?.Stats;
            }

            return null;
        }

        internal void AddStorage<TState>(IStorage<TState> storage, string stateName = default)
        {
            var normalisedStateName = stateName ?? "Default";

            _storages[normalisedStateName] = storage;
        }
    }
}
