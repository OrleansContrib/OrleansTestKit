using System;
using System.Collections.Generic;
using Orleans.Core;

namespace Orleans.TestKit.Storage
{
    internal sealed class StorageManager
    {
        private readonly Dictionary<string, object> _storages = new Dictionary<string, object>();

        private static string GetKey(string identityString, Type stateType)
            => $"{stateType.FullName}-{identityString}";

        public IStorage AddStorage<T>(IGrainIdentity identity) where T : Grain
        {
            var key = GetKey(identity.IdentityString, typeof(T));

            var storage = new TestStorage();

            _storages.Add(key, storage);

            return storage;
        }

        public TestStorageStats GetStorageStats<T>(T grain) where T : Grain
        {
            var key = GetKey(grain.IdentityString, grain.GetType());

            var storage = _storages[key] as TestStorage;

            return storage?.Stats;
        }
    }
}