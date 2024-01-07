using Orleans.Core;
using Orleans.Runtime;
using Orleans.TestKit.Storage;

namespace Orleans.TestKit;

public static class StorageExtensions
{
    public static TState State<TGrain, TState>(this TestKitSilo silo)
        where TGrain : Grain<TState>
    {
        ArgumentNullException.ThrowIfNull(silo);

        return silo.StorageManager.GetGrainStorage<TGrain, TState>().State;
    }

    public static TestStorageStats? StorageStats<TGrain, TState>(this TestKitSilo silo)
        where TGrain : Grain<TState>
    {
        ArgumentNullException.ThrowIfNull(silo);

        return silo.StorageManager.GetStorageStats<TGrain, TState>();
    }

    public static IStorage<TState> AddGrainState<TGrain, TState>(this TestKitSilo silo, TState? state = default)
        where TGrain : Grain<TState>
    {
        ArgumentNullException.ThrowIfNull(silo);

        var storage = silo.StorageManager.GetGrainStorage<TGrain, TState>();
        storage.State = state ?? Activator.CreateInstance<TState>();
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
        string? storageName = default,
        T? state = default) =>
        silo.AddPersistentStateStorage(
            stateName,
            storageName,
            new TestStorage<T>(state ?? Activator.CreateInstance<T>()));

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
        string? storageName = default,
        IStorage<T>? storage = default)
    {
        ArgumentNullException.ThrowIfNull(silo);

        if (string.IsNullOrWhiteSpace(stateName))
        {
            throw new ArgumentException("A state name must be provided", nameof(stateName));
        }

        var normalizedStorage = storage ?? new TestStorage<T>(Activator.CreateInstance<T>());
        var normalizedStorageName = storageName ?? "Default";

        silo.StorageManager.AddStorage(normalizedStorage, stateName);
        return silo.StorageManager.StateAttributeFactoryMapper.AddPersistentState(normalizedStorage, stateName, normalizedStorageName);
    }
}
