using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Orleans.Core;

namespace Orleans.TestKit
{
    internal class TestGrainFactory : IGrainFactory
    {
        private readonly TestKitOptions _options;

        private readonly Dictionary<string, IGrain> _probes = new Dictionary<string, IGrain>();

        public TestGrainFactory(TestKitOptions options)
        {
            _options = options;
        }

        public TGrainInterface GetGrain<TGrainInterface>(Guid primaryKey, string grainClassNamePrefix = null)
            where TGrainInterface : IGrainWithGuidKey
        {
            return GetProbe<TGrainInterface>(new TestGrainIdentity(primaryKey), grainClassNamePrefix);
        }

        public TGrainInterface GetGrain<TGrainInterface>(long primaryKey, string grainClassNamePrefix = null)
            where TGrainInterface : IGrainWithIntegerKey
        {
            return GetProbe<TGrainInterface>(new TestGrainIdentity(primaryKey), grainClassNamePrefix);
        }

        public TGrainInterface GetGrain<TGrainInterface>(string primaryKey, string grainClassNamePrefix = null)
            where TGrainInterface : IGrainWithStringKey
        {
            return GetProbe<TGrainInterface>(new TestGrainIdentity(primaryKey), grainClassNamePrefix);
        }

        public TGrainInterface GetGrain<TGrainInterface>(Guid primaryKey, string keyExtension,
            string grainClassNamePrefix = null) where TGrainInterface : IGrainWithGuidCompoundKey
        {
            throw new NotImplementedException();
        }

        public TGrainInterface GetGrain<TGrainInterface>(long primaryKey, string keyExtension,
            string grainClassNamePrefix = null) where TGrainInterface : IGrainWithIntegerCompoundKey
        {
            throw new NotImplementedException();
        }

        public Task<TGrainObserverInterface> CreateObjectReference<TGrainObserverInterface>(IGrainObserver obj)
            where TGrainObserverInterface : IGrainObserver
        {
            throw new NotImplementedException();
        }

        public Task DeleteObjectReference<TGrainObserverInterface>(IGrainObserver obj)
            where TGrainObserverInterface : IGrainObserver
        {
            throw new NotImplementedException();
        }

        private static string GetKey(IGrainIdentity identity, Type stateType)
            => $"{stateType.FullName}-{identity.IdentityString}";

        private T GetProbe<T>(IGrainIdentity identity, string grainClassNamePrefix) where T : IGrain
        {
            if (grainClassNamePrefix != null)
                throw new ArgumentException($"Class prefix not supported in {nameof(TestGrainFactory)}",
                    $"{nameof(grainClassNamePrefix)}");

            var key = GetKey(identity, typeof(T));

            IGrain grain;

            if (_probes.TryGetValue(key, out grain))
                return (T) grain;

            //If using strict grain probes, throw the exception
            if (_options.StrictGrainProbes)
                throw new Exception(
                    $"Probe {identity.IdentityString} does not exist for type {typeof(T).Name}. Ensure that it is added before the grain is tested.");
            else
            {
                //Create a new mock
                var mock = Activator.CreateInstance(typeof(Mock<>).MakeGenericType(typeof(T))) as IMock<IGrain>;
                
                grain = mock?.Object;

                //Save the newly created grain for the next call
                _probes.Add(key, grain);
            }

            return (T) grain;
        }


        internal Mock<T> AddProbe<T>(IGrainIdentity identity) where T : class, IGrain
        {
            var key = GetKey(identity, typeof(T));

            var mock = new Mock<T>();

            _probes.Add(key, mock.Object);

            return mock;
        }
    }
}