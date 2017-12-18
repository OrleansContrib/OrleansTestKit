using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Streams;

namespace Orleans.TestKit.Streams
{
    public class TestStreamProviderManager : IKeyedServiceCollection<string, IStreamProvider>
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
            if (_streamProviders.TryGetValue(name, out provider))
                return provider;

            if (_options.StrictGrainProbes)
                throw new Exception($"Could not find stream provider {name}");

            return Add(name);
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

            return Add(name);
        }

        private TestStreamProvider Add(string name)
        {
            var provider = new TestStreamProvider(_options);

            provider.Init(name, null, null).Wait();
            _streamProviders.Add(name, provider);

            return provider;
        }

        IStreamProvider IKeyedServiceCollection<string, IStreamProvider>.GetService(IServiceProvider services, string key) => _streamProviders[key];
    }
}
