﻿using System;
using System.Collections.Generic;
using Moq;
using System.Diagnostics.CodeAnalysis;
using Orleans.Core;
using Orleans.Runtime;

namespace Orleans.TestKit.Storage
{
    public sealed class StorageManager
    {
        private readonly TestKitOptions _options;

        private readonly Dictionary<string, object> _storages = new Dictionary<string, object>();

        public StorageManager(TestKitOptions options) =>
            _options = options ?? throw new ArgumentNullException(nameof(options));

        internal readonly Mock<IAttributeToFactoryMapper<PersistentStateAttribute>> stateAttributeFactoryMapperMock =
            new Mock<IAttributeToFactoryMapper<PersistentStateAttribute>>();

        public IStorage<TState> GetStorage<TState>() => GetStorage<TState>("Default");

        public IStorage<TState> GetStorage<TState>(string stateName)
        {
            var normalisedStateName = stateName ?? "Default";

            if (_storages.TryGetValue(normalisedStateName, out var storage) is false)
            {
                storage = _storages[normalisedStateName] = _options.StorageFactory?.Invoke(typeof(TState)) ?? new TestStorage<TState>();
            }

            return storage as IStorage<TState>;
        }

        public TestStorageStats GetStorageStats() => GetStorageStats("Default");

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

        public TestStorageStats StorageStats => GetStorageStats();

        internal void AddStorage<TState>(IStorage<TState> storage, string stateName = default)
        {
            var normalisedStateName = stateName ?? "Default";

            _storages[normalisedStateName] = storage;
        }
    }
}
