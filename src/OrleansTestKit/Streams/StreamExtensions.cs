using System;
using Orleans.Runtime;
using Orleans.TestKit.Streams;

namespace Orleans.TestKit
{
    public static class StreamExtensions
    {
        public static TestStream<T> AddStreamProbe<T>(this TestKitSilo silo) =>
            AddStreamProbe<T>(silo, Guid.Empty);

        public static TestStream<T> AddStreamProbe<T>(this TestKitSilo silo, Guid id) =>
            AddStreamProbe<T>(silo, id, typeof(T).Name);

        public static TestStream<T> AddStreamProbe<T>(this TestKitSilo silo, Guid id, string streamNamespace) =>
            AddStreamProbe<T>(silo, id, streamNamespace, "Default");


        public static TestStream<T> AddStreamProbe<T>(this TestKitSilo silo, Guid id, string streamNamespace, string providerName)
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            if (providerName == null)
            {
                throw new ArgumentNullException(nameof(providerName));
            }

            return silo.StreamProviderManager.AddStreamProbe<T>(id, streamNamespace, providerName);
        }
    }
}
