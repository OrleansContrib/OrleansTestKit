using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Providers;
using Orleans.Streams;

namespace Orleans.TestKit.Streams
{
    public class TestStreamProvider : IStreamProviderImpl
    {
        private readonly TestKitOptions _options;
        private readonly Dictionary<TestStreamId, IStreamIdentity> _streams = new Dictionary<TestStreamId, IStreamIdentity>();

        public TestStreamProvider(TestKitOptions options)
        {
            _options = options;
        }

        public string Name { get; private set; }

        public bool IsRewindable { get; } = false;

        public IAsyncStream<T> GetStream<T>(Guid streamId, string streamNamespace)
        {
            IStreamIdentity stream;

            if (_streams.TryGetValue(new TestStreamId(streamId, streamNamespace), out stream))
                return stream as IAsyncStream<T>;

            if (_options.StrictStreamProbes)

                throw new Exception(
                    $"Unable to find stream {streamId}-{streamNamespace}. Ensure a stream probe was added");

            else
                stream = AddStreamProbe<T>(streamId, streamNamespace);

            return (IAsyncStream<T>) stream;
        }

        public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            Name = name;

            return TaskDone.Done;
        }

        public Task Close() => TaskDone.Done;

        public Task Start() => TaskDone.Done;

        public TestStream<T> AddStreamProbe<T>(Guid streamId, string streamNamespace)
        {
            var id = new TestStreamId(streamId, streamNamespace);

            var stream = new TestStream<T>(streamId, streamNamespace, Name);

            _streams.Add(id, stream);

            return stream;
        }
    }
}