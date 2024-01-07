using System.Diagnostics.CodeAnalysis;
using Orleans.Core;

namespace Orleans.TestKit.Storage;

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
        => GetStorage<TState>(typeof(TGrain).FullName!);

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

        return (IStorage<TState>)storage;
    }

    /// <summary>
    /// Get the storage stats for the provided grain with <typeparamref name="TState"></typeparamref> state object.
    /// </summary>
    /// <typeparam name="TGrain">The type of the grain with the state</typeparam>
    /// <typeparam name="TState">The type of the state</typeparam>
    /// <returns>The storage stats if they exist</returns>
    public TestStorageStats? GetStorageStats<TGrain, TState>() where TGrain : Grain<TState>
        => GetStorageStats(typeof(TGrain).FullName);

    /// <summary>
    /// Get the storage stats for the state with the provided <paramref name="stateName"/> or "Default" if not provided.
    /// </summary>
    /// <param name="stateName">The name of the state or if not provided "Default"</param>
    /// <returns>The storage stats if they exist for the state</returns>
    public TestStorageStats? GetStorageStats(string? stateName)
    {
        var normalisedStateName = stateName ?? "Default";

        if (_storages.TryGetValue(normalisedStateName, out var storage))
        {
            var stats = storage as IStorageStats;
            return stats?.Stats;
        }

        return null;
    }

    /// <summary>
    /// Try to get the storage stats for the provided provided grain with <typeparamref name="TState"></typeparamref> object.
    /// </summary>
    /// <param name="stats">The <see cref="TestStorageStats"/> if they are found or <see langword="null"/> if not.</param>
    /// <typeparam name="TGrain">The type of the grain with the state</typeparam>
    /// <typeparam name="TState">The type of the state</typeparam>
    /// <returns><see langword="true"/> if <see cref="TestStorageStats"/> are found or <see langword="false"/> if not</returns>
    public bool TryGetStorageStats<TGrain, TState>([NotNullWhen(true)] out TestStorageStats? stats)
        where TGrain : Grain<TState>
        => TryGetStorageStats(typeof(TGrain).FullName, out stats);

    /// <summary>
    /// Try to get the storage stats for the state with the provided <paramref name="stateName"/> or "Default" if not provided.
    /// </summary>
    /// <param name="stateName">The name of the state or if not provided "Default"</param>
    /// <param name="stats">The <see cref="TestStorageStats"/> if they are found or <see langword="null"/> if not.</param>
    /// <returns><see langword="true"/> if <see cref="TestStorageStats"/> are found or <see langword="false"/> if not</returns>
    public bool TryGetStorageStats(string? stateName, [NotNullWhen(true)] out TestStorageStats? stats)
    {
        var normalisedStateName = stateName ?? "Default";

        if (_storages.TryGetValue(normalisedStateName, out var storage) && storage is IStorageStats storageWithStats)
        {
            stats = storageWithStats.Stats;
            return true;
        }

        stats = default;
        return false;
    }

    internal void AddStorage<TState>(IStorage<TState> storage, string? stateName = default)
    {
        var normalisedStateName = stateName ?? "Default";

        _storages[normalisedStateName] = storage;
    }
}
