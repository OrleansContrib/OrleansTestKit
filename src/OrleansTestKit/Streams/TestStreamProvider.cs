using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Runtime;
using Orleans.Streams;

namespace Orleans.TestKit.Streams
{
    public sealed class TestStreamProvider : IStreamProvider
    {
        private readonly TestKitOptions _options;

        private readonly Dictionary<StreamId, IStreamIdentity> _streams = new();

        public TestStreamProvider(TestKitOptions options) =>
            _options = options ?? throw new ArgumentNullException(nameof(options));

        public string Name { get; private set; }

        public bool IsRewindable { get; }

        public IAsyncStream<T> GetStream<T>(StreamId streamId)
        {
            if (_streams.TryGetValue(streamId, out var stream))
            {
                return stream as IAsyncStream<T>;
            }

            if (_options.StrictStreamProbes)
            {
                throw new Exception($"Unable to find stream {streamId}. Ensure a stream probe was added");
            }

            stream = AddStreamProbe<T>(streamId);
            return stream as IAsyncStream<T>;
        }

        public Task Init(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            return Task.CompletedTask;
        }

        public TestStream<T> AddStreamProbe<T>(StreamId streamId)
        {
            var stream = new TestStream<T>(streamId, Name);
            _streams.Add(streamId, stream);
            return stream;
        }
    }
}
