using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Streams;

namespace Orleans.TestKit.Streams
{
    public sealed class TestStreamProvider :
        IStreamProvider
    {
        private readonly TestKitOptions _options;

        private readonly Dictionary<TestStreamId, IStreamIdentity> _streams = new Dictionary<TestStreamId, IStreamIdentity>();

        public TestStreamProvider(TestKitOptions options) =>
            _options = options ?? throw new ArgumentNullException(nameof(options));

        public string Name { get; private set; }

        public bool IsRewindable { get; }

        public IAsyncStream<T> GetStream<T>(Guid streamId, string streamNamespace)
        {
            if (_streams.TryGetValue(new TestStreamId(streamId, streamNamespace), out var stream))
            {
                return (IAsyncStream<T>)stream;
            }

            if (_options.StrictStreamProbes)
            {
                throw new Exception($"Unable to find stream {streamId}-{streamNamespace}. Ensure a stream probe was added");
            }

            stream = AddStreamProbe<T>(streamId, streamNamespace);
            return (IAsyncStream<T>)stream;
        }

        public Task Init(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            return Task.CompletedTask;
        }

        public TestStream<T> AddStreamProbe<T>(Guid streamId, string streamNamespace)
        {
            var id = new TestStreamId(streamId, streamNamespace);
            var stream = new TestStream<T>(streamId, streamNamespace, Name);
            _streams.Add(id, stream);
            return stream;
        }
    }
}
