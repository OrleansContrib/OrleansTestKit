using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Orleans.Core;
using Orleans.Runtime;

namespace Orleans.TestKit
{
    public class TestGrainFactory : IGrainFactory
    {
        private readonly TestKitOptions _options;

        private readonly Dictionary<string, IGrain> _probes = new Dictionary<string, IGrain>();

        private readonly Dictionary<Type, Func<IGrainIdentity, IMock<IGrain>>> _probeFactories = new Dictionary<Type, Func<IGrainIdentity, IMock<IGrain>>>();

        internal TestGrainFactory(TestKitOptions options)
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
            return GetProbe<TGrainInterface>(new TestGrainIdentity(primaryKey, keyExtension), grainClassNamePrefix);
        }

        public TGrainInterface GetGrain<TGrainInterface>(long primaryKey, string keyExtension,
            string grainClassNamePrefix = null) where TGrainInterface : IGrainWithIntegerCompoundKey
        {
            return GetProbe<TGrainInterface>(new TestGrainIdentity(primaryKey, keyExtension), grainClassNamePrefix);
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

        private static string GetKey(IGrainIdentity identity, Type stateType, string classPrefix = null)
        {
            if (classPrefix == null)
                return $"{stateType.FullName}-{identity.IdentityString}";
            else
                return $"{stateType.FullName}-{classPrefix}-{identity.IdentityString}";
        }

        private T GetProbe<T>(IGrainIdentity identity, string grainClassNamePrefix) where T : IGrain
        {
            var key = GetKey(identity, typeof(T), grainClassNamePrefix);

            IGrain grain;

            if (_probes.TryGetValue(key, out grain))
                return (T)grain;

            //If using strict grain probes, throw the exception
            if (_options.StrictGrainProbes)
                throw new Exception(
                    $"Probe {identity.IdentityString} does not exist for type {typeof(T).Name}. Ensure that it is added before the grain is tested.");
            else
            {
                IMock<IGrain> mock;
                Func<IGrainIdentity, IMock<IGrain>> factory;

                if (_probeFactories.TryGetValue(typeof(T), out factory))
                {
                    mock = factory(identity);
                }
                else
                {
                    mock = Activator.CreateInstance(typeof(Mock<>).MakeGenericType(typeof(T))) as IMock<IGrain>;
                }

                grain = mock?.Object;

                //Save the newly created grain for the next call
                _probes.Add(key, grain);
            }

            return (T)grain;
        }


        internal Mock<T> AddProbe<T>(IGrainIdentity identity, string classPrefix = null) where T : class, IGrain
        {
            var key = GetKey(identity, typeof(T), classPrefix);

            var mock = new Mock<T>();

            _probes.Add(key, mock.Object);

            return mock;
        }

        internal void AddProbe<T>(Func<IGrainIdentity, IMock<T>> factory) where T : class, IGrain
        {
            _probeFactories.Add(typeof(T), factory);
        }

        public void BindGrainReference(IAddressable grain)
        {
            throw new NotImplementedException();
        }
    }
}
