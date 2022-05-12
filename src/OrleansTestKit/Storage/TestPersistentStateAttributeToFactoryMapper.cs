using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Orleans.Core;
using Orleans.Runtime;

namespace Orleans.TestKit.Storage
{
    public sealed class TestPersistentStateAttributeToFactoryMapper : IAttributeToFactoryMapper<PersistentStateAttribute>
    {
        private readonly StorageManager storageManager;
        private readonly Dictionary<Type, Dictionary<(string StateName, string StorageName), object>>
            registeredStorage = new();
        private readonly MethodInfo AddEmptyStateMethod = typeof(TestPersistentStateAttributeToFactoryMapper)
            .GetMethod(nameof(TestPersistentStateAttributeToFactoryMapper.AddEmptyState), BindingFlags.Instance | BindingFlags.NonPublic);

        public TestPersistentStateAttributeToFactoryMapper(StorageManager storageManager) => this.storageManager = storageManager;

        public IPersistentState<T> AddPersistentState<T>(
            IStorage<T> storage,
            string stateName,
            string storageName)
        {
            if (storage is null)
            {
                throw new ArgumentNullException(nameof(storage));
            }

            if (string.IsNullOrWhiteSpace(stateName))
            {
                throw new ArgumentException($"'{nameof(stateName)}' cannot be null or whitespace.", nameof(stateName));
            }

            if (string.IsNullOrWhiteSpace(storageName))
            {
                throw new ArgumentException($"'{nameof(storageName)}' cannot be null or whitespace.", nameof(storageName));
            }

            var dict = registeredStorage.TryGetValue(typeof(T), out var typeStateRegistry)
                ? typeStateRegistry
                : registeredStorage[typeof(T)] = new Dictionary<(string StateName, string StorageName), object>(1);

            var fake = new PersistentStateFake<T>(storage);
            dict[(stateName, storageName)] = fake;

            return fake;
        }

        public Factory<IGrainActivationContext, object> GetFactory(ParameterInfo parameter, PersistentStateAttribute metadata)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (metadata is null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            if (parameter.ParameterType.IsGenericType &&
                parameter.ParameterType.GetGenericTypeDefinition() != typeof(IPersistentState<>))
            {
                throw new InvalidOperationException($"No persistent state for the parameter '{parameter.Name}'");
            }

            var parameterType = parameter.ParameterType.GenericTypeArguments[0];
            var stateName = metadata.StateName;
            var storageName = metadata.StorageName ?? "Default";

            if (registeredStorage.TryGetValue(parameterType, out var typeStateRegistry))
            {
                // If we must have the state and the storage name so lookup by both
                if (typeStateRegistry.TryGetValue((metadata.StateName, metadata.StorageName), out var persistentState))
                {
                    return _ => persistentState;
                }
            }

            var state = AddEmptyStateMethod.MakeGenericMethod(parameterType).Invoke(
                this,
                new[] { stateName, storageName });

            return _ => state;
        }

        private IPersistentState<TState> AddEmptyState<TState>(string stateName, string storageName)
        {
            var storage = storageManager.GetStorage<TState>(stateName);
            return AddPersistentState(storage, stateName, storageName);
        }
    }

    internal class PersistentStateFake<TState> : IPersistentState<TState>, IStorageStats
    {
        private readonly IStorage<TState> _storage;

        public PersistentStateFake(IStorage<TState> storage) => _storage = storage;

        public TState State
        {
            get => _storage.State;
            set => _storage.State = value;
        }

        public string Etag => _storage.Etag;

        public bool RecordExists => _storage.RecordExists;

        public TestStorageStats Stats => _storage is IStorageStats statsStorage
            ? statsStorage.Stats
            : null;

        public Task ClearStateAsync() => _storage.ClearStateAsync();

        public Task ReadStateAsync() => _storage.ReadStateAsync();

        public Task WriteStateAsync() => _storage.WriteStateAsync();
    }
}
