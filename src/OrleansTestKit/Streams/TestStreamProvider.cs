using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Providers;
using Orleans.Streams;

namespace Orleans.TestKit.Streams
{
    public class TestStreamProvider : IStreamProviderImpl
    {
        private readonly Dictionary<TestStreamId, object> _streams = new Dictionary<TestStreamId, object>();

        public string Name { get; private set; }

        public bool IsRewindable { get; } = false;

        public IAsyncStream<T> GetStream<T>(Guid streamId, string streamNamespace)
        {
            object stream;

            if (!_streams.TryGetValue(new TestStreamId(streamId, streamNamespace), out stream))
            {
                throw new Exception(
                    $"Unable to find stream {streamId}-{streamNamespace}. Ensure a stream probe was added");
            }

            return stream as IAsyncStream<T>;
        }

        public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            Name = name;

            return TaskDone.Done;
        }

        public Task Close() => TaskDone.Done;

        public Task Start() => TaskDone.Done;

        public TestStream<T> AddStreamProbe<T>(Guid streamId, string streamNamespace, string providerName)
        {
            var id = new TestStreamId(streamId, streamNamespace);

            var stream = new TestStream<T>(streamId, streamNamespace, providerName);

            _streams.Add(id, stream);

            return stream;
        }
    }
}