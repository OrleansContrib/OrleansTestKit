using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using Moq;
using Orleans.Core;
using Orleans.Runtime;
using Orleans.Storage;
using Orleans.TestKit.Storage;

namespace Orleans.TestKit
{
    public static class StorageExtensions
    {
        public static TState State<TGrain, TState>(this TestKitSilo silo)
            where TGrain : Grain<TState>
            where TState : class, new()
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            return silo.StorageManager.GetGrainStorage<TGrain, TState>().State;
        }

        public static TestStorageStats StorageStats<TGrain, TState>(this TestKitSilo silo)
            where TGrain : Grain<TState>
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            return silo.StorageManager.GetStorageStats<TGrain, TState>();
        }

        public static IStorage<T> AddGrainState<TGrain, T>(
            this TestKitSilo silo,
            T state = default)
            where TGrain : Grain<T>
            where T : new()
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            var storage = silo.StorageManager.GetGrainStorage<TGrain, T>();
            storage.State = state ?? new T();
            return storage;
        }

        /// <summary>
        /// Add persistent state to the silo for a given type. If a state is provided that will be the loaded state otherwise a new <typeparamref name="T"/>
        /// </summary>
        /// <remarks>
        /// If neither StateName or StorageName are provided then we resolve just based on type, otherwise we try state name and optionally a storage name
        /// </remarks>
        /// <typeparam name="T">The type of data in the state</typeparam>
        /// <param name="silo">The silo to add the state to</param>
        /// <param name="stateName">The state name on the persistent state parameter</param>
        /// <param name="storageName">The storage name on the persistent state parameter</param>
        /// <param name="state">The state to set as default if any</param>
        /// <returns>The persistent state</returns>
        public static IPersistentState<T> AddPersistentState<T>(
            this TestKitSilo silo,
            string stateName,
            string storageName = default,
            T state = default)
            where T : new()
        {
            return silo.AddPersistentStateStorage(
                stateName,
                storageName,
                new TestStorage<T>(state ?? new T()));
        }

        /// <summary>
        /// Add persistent state to the silo for a given type. If a storage is provided that will provide the loaded state otherwise a new <typeparamref name="T"/>
        /// </summary>
        /// <remarks>
        /// If neither StateName or StorageName are provided then we resolve just based on type, otherwise we try state name and optionally a storage name
        /// </remarks>
        /// <typeparam name="T">The type of data in the state</typeparam>
        /// <param name="silo">The silo to add the state to</param>
        /// <param name="stateName">The state name on the persistent state parameter</param>
        /// <param name="storageName">The storage name on the persistent state parameter</param>
        /// <param name="storage">The storage to use, if null a default implementation will be created</param>
        /// <returns>The persistent state</returns>
        public static IPersistentState<T> AddPersistentStateStorage<T>(
            this TestKitSilo silo,
            string stateName,
            string storageName = default,
            IStorage<T> storage = default)
            where T : new()
        {
            var normalizedStorage = storage ?? new TestStorage<T>(new T());
            var normalizedStorageName = storageName ?? "Default";

            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            if (string.IsNullOrWhiteSpace(stateName))
            {
                throw new ArgumentException("A state name must be provided", nameof(stateName));
            }

            silo.StorageManager.AddStorage(storage, stateName);
            return silo.StorageManager.StateAttributeFactoryMapper.AddPersistentState(normalizedStorage, stateName, normalizedStorageName);
        }
    }
}
