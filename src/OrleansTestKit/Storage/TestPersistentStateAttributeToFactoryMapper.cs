using System.Reflection;
using Orleans.Core;
using Orleans.Runtime;

namespace Orleans.TestKit.Storage;

public sealed class TestPersistentStateAttributeToFactoryMapper : IAttributeToFactoryMapper<PersistentStateAttribute>
{
    private static readonly MethodInfo _addEmptyStateMethod = typeof(TestPersistentStateAttributeToFactoryMapper)
        .GetMethod(nameof(AddEmptyState), BindingFlags.Instance | BindingFlags.NonPublic)!;

    private readonly Dictionary<Type, Dictionary<(string StateName, string StorageName), object>>
        _registeredStorage = new();

    private readonly StorageManager _storageManager;

    public TestPersistentStateAttributeToFactoryMapper(StorageManager storageManager) =>
        _storageManager = storageManager;

    public Factory<IGrainContext, object> GetFactory(ParameterInfo parameter,
        PersistentStateAttribute metadata)
    {
        ArgumentNullException.ThrowIfNull(parameter);
        ArgumentNullException.ThrowIfNull(metadata);

        if (parameter.ParameterType.IsGenericType &&
            parameter.ParameterType.GetGenericTypeDefinition() != typeof(IPersistentState<>))
        {
            throw new InvalidOperationException($"No persistent state for the parameter '{parameter.Name}'");
        }

        var parameterType = parameter.ParameterType.GenericTypeArguments[0];
        var stateName = metadata.StateName;
        var storageName = metadata.StorageName ?? "Default";

        if (_registeredStorage.TryGetValue(parameterType, out var typeStateRegistry))
        {
            // If we must have the state and the storage name so lookup by both
            if (typeStateRegistry.TryGetValue((metadata.StateName, storageName), out var persistentState))
            {
                return _ => persistentState;
            }
        }

        var state = _addEmptyStateMethod.MakeGenericMethod(parameterType).Invoke(
            this,
            new object?[] { stateName, storageName })!;

        return _ => state;
    }

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

        var dict = _registeredStorage.TryGetValue(typeof(T), out var typeStateRegistry)
            ? typeStateRegistry
            : _registeredStorage[typeof(T)] = new Dictionary<(string StateName, string StorageName), object>(1);

        var fake = new PersistentStateFake<T>(storage);
        dict[(stateName, storageName)] = fake;

        return fake;
    }

    private IPersistentState<TState> AddEmptyState<TState>(string stateName, string storageName)
    {
        var storage = _storageManager.GetStorage<TState>(stateName);
        return AddPersistentState(storage, stateName, storageName);
    }
}

internal class PersistentStateFake<TState> : IPersistentState<TState>, IStorageStats
{
    private readonly IStorage<TState> _storage;
    private readonly TestStorageStats? _stats;

    public PersistentStateFake(IStorage<TState> storage)
    {
        _storage = storage;

        // If the underlying storage doesn't have stats we should wrap it in our own.
        if (_storage is IStorageStats is false)
        {
            _stats = new TestStorageStats();
        }
    }

    public TState State
    {
        get => _storage.State;
        set => _storage.State = value;
    }

    public string Etag => _storage.Etag;

    public bool RecordExists => _storage.RecordExists;

    public Task ClearStateAsync()
    {
        if (_stats is not null)
        {
            _stats.Clears++;
        }

        return _storage.ClearStateAsync();
    }

    public Task ReadStateAsync()
    {
        if (_stats is not null)
        {
            _stats.Reads++;
        }

        return _storage.ReadStateAsync();
    }

    public Task WriteStateAsync()
    {
        if (_stats is not null)
        {
            _stats.Writes++;
        }

        return _storage.WriteStateAsync();
    }

    public TestStorageStats Stats =>
        _storage is IStorageStats statsStorage
            ? statsStorage.Stats
            : _stats!;
}
