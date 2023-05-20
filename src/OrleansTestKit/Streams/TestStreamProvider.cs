using Orleans.Runtime;
using Orleans.Streams;

namespace Orleans.TestKit.Streams;

public sealed class TestStreamProvider : IStreamProvider
{
    private readonly TestKitOptions _options;

    private readonly Dictionary<StreamId, IStreamIdentity> _streams = new();

    public TestStreamProvider(TestKitOptions options) =>
        _options = options ?? throw new ArgumentNullException(nameof(options));

    /// <inheritdoc/>
    public bool IsRewindable { get; }

    /// <inheritdoc/>
    public string Name { get; private set; } = string.Empty;

    public TestStream<T> AddStreamProbe<T>(StreamId streamId)
    {
        var stream = new TestStream<T>(streamId, Name);
        _streams.Add(streamId, stream);
        return stream;
    }

    /// <inheritdoc/>
    public IAsyncStream<T> GetStream<T>(StreamId streamId)
    {
        if (_streams.TryGetValue(streamId, out var stream))
        {
            return (IAsyncStream<T>)stream;
        }

        if (_options.StrictStreamProbes)
        {
            throw new Exception($"Unable to find stream {streamId}. Ensure a stream probe was added");
        }

        stream = AddStreamProbe<T>(streamId);
        return (IAsyncStream<T>)stream;
    }

    public Task Init(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        return Task.CompletedTask;
    }
}
