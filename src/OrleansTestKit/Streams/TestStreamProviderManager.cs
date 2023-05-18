﻿using Orleans.Runtime;
using Orleans.Streams;

namespace Orleans.TestKit.Streams;

public sealed class TestStreamProviderManager : IKeyedServiceCollection<string, IStreamProvider>
{
    private readonly TestKitOptions _options;

    private readonly Dictionary<string, TestStreamProvider> _streamProviders = new();

    public TestStreamProviderManager(TestKitOptions options) =>
        _options = options ?? throw new ArgumentNullException(nameof(options));

    public TestStream<T> AddStreamProbe<T>(Guid streamId, string ns, string providerName) =>
        AddStreamProbe<T>(StreamId.Create(ns, streamId), providerName);

    public TestStream<T> AddStreamProbe<T>(StreamId streamId, string providerName)
    {
        var provider = GetOrAdd(providerName);
        return provider.AddStreamProbe<T>(streamId);
    }

    public IStreamProvider GetProvider(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (_streamProviders.TryGetValue(name, out var provider))
        {
            return provider;
        }

        if (_options.StrictGrainProbes)
        {
            throw new Exception($"Could not find stream provider {name}");
        }

        return Add(name);
    }

    IStreamProvider IKeyedServiceCollection<string, IStreamProvider>.GetService(IServiceProvider services, string key) =>
        GetProvider(key);

    IEnumerable<IKeyedService<string, IStreamProvider>> IKeyedServiceCollection<string, IStreamProvider>.GetServices(IServiceProvider services) =>
        (IEnumerable<IKeyedService<string, IStreamProvider>>)_streamProviders;

    private TestStreamProvider Add(string name)
    {
        var provider = new TestStreamProvider(_options);
        provider.Init(name).Wait();
        _streamProviders.Add(name, provider);
        return provider;
    }

    private TestStreamProvider GetOrAdd(string name) =>
        _streamProviders.TryGetValue(name, out var provider) ? provider : Add(name);
}
