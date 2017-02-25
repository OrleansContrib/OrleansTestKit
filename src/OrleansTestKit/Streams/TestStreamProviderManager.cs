using System;
using System.Collections.Generic;
using Orleans.Providers;
using Orleans.Streams;

namespace Orleans.TestKit.Streams
{
    public class TestStreamProviderManager : IStreamProviderManager
    {
        private readonly TestKitOptions _options;

        private readonly Dictionary<string, TestStreamProvider> _streamProviders =
            new Dictionary<string, TestStreamProvider>();

        public TestStreamProviderManager(TestKitOptions options)
        {
            _options = options;
        }

        public IProvider GetProvider(string name)
        {
            TestStreamProvider provider;

            if (!_streamProviders.TryGetValue(name, out provider))
                throw new Exception($"Could not find stream provider {name}");

            return provider;
        }

        public IEnumerable<IStreamProvider> GetStreamProviders()
        {
            return _streamProviders.Values;
        }

        public TestStream<T> AddStreamProbe<T>(Guid streamId, string streamNamespace, string providerName)
        {
            var provider = GetOrAdd(providerName);

            return provider.AddStreamProbe<T>(streamId, streamNamespace);
        }

        private TestStreamProvider GetOrAdd(string name)
        {
            TestStreamProvider provider;

            if (_streamProviders.TryGetValue(name, out provider))
                return provider;

            provider = new TestStreamProvider(_options);
            provider.Init(name, null, null).Wait();
            _streamProviders.Add(name, provider);

            return provider;
        }
    }
}