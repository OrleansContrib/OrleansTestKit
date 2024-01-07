using Orleans.Runtime;

namespace Orleans.TestKit.Storage;

internal class TestStorage<TState> : IStorageStats, IPersistentState<TState>
{
    public TestStorage() : this(CreateState())
    {
    }

    public TestStorage(TState state)
    {
        Stats = new TestStorageStats { Reads = -1 };
        State = state;
    }

    public string Etag => string.Empty;

    public bool RecordExists { get; private set; }

    public TState State { get; set; }

    public Task ClearStateAsync()
    {
        State = CreateState();
        Stats.Clears++;
        RecordExists = false;
        return Task.CompletedTask;
    }

    public Task ReadStateAsync()
    {
        Stats.Reads++;
        return Task.CompletedTask;
    }

    public Task WriteStateAsync()
    {
        Stats.Writes++;
        RecordExists = true;
        return Task.CompletedTask;
    }

    public TestStorageStats Stats { get; }

    private static TState CreateState()
    {
        if (!typeof(TState).IsValueType && typeof(TState).GetConstructor(Type.EmptyTypes) == null)
        {
            throw new NotSupportedException(
                $"No parameterless constructor defined for {typeof(TState).Name}. This is currently not supported");
        }

        return Activator.CreateInstance<TState>();
    }
}
