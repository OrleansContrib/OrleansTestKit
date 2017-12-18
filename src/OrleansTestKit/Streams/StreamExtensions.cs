using System;

namespace Orleans.TestKit.Streams
{
    public static class StreamExtensions
    {
        public static TestStream<T> AddStreamProbe<T>(this TestKitSilo silo) where T : class => AddStreamProbe<T>(silo, Guid.Empty);

        public static TestStream<T> AddStreamProbe<T>(this TestKitSilo silo, Guid id) where T : class
            => AddStreamProbe<T>(silo, id, typeof(T).Name);

        public static TestStream<T> AddStreamProbe<T>(this TestKitSilo silo, Guid id, string streamNamespace) where T : class
            => AddStreamProbe<T>(silo, id, streamNamespace, "Default");

        public static TestStream<T> AddStreamProbe<T>(this TestKitSilo silo, Guid id, string streamNamespace, string providerName) where T : class
            => silo.StreamProviderManager.AddStreamProbe<T>(id, streamNamespace, providerName);

    }
}