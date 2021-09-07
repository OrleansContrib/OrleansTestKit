using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Orleans.Core;
using Orleans.Runtime;

namespace Orleans.TestKit
{
    public sealed class TestGrainFactory :
        IGrainFactory
    {
        private readonly TestKitOptions _options;
        private readonly Dictionary<Type, Func<IGrainIdentity, IGrain>> _probeFactories;

        private readonly Dictionary<string, IGrain> _probes;

        internal TestGrainFactory(TestKitOptions options)
        {
            _options = options;
            _probeFactories = new Dictionary<Type, Func<IGrainIdentity, IGrain>>();
            _probes = new Dictionary<string, IGrain>();
        }

        public void BindGrainReference(IAddressable grain) =>
            throw new NotImplementedException();

        public Task<TGrainObserverInterface> CreateObjectReference<TGrainObserverInterface>(IGrainObserver obj)
            where TGrainObserverInterface : IGrainObserver =>
            throw new NotImplementedException();

        public Task DeleteObjectReference<TGrainObserverInterface>(IGrainObserver obj)
            where TGrainObserverInterface : IGrainObserver =>
            throw new NotImplementedException();

        public TGrainInterface GetGrain<TGrainInterface>(Guid primaryKey, string grainClassNamePrefix = null)
            where TGrainInterface : IGrainWithGuidKey =>
            GetProbe<TGrainInterface>(new TestGrainIdentity(primaryKey), grainClassNamePrefix);

        public TGrainInterface GetGrain<TGrainInterface>(long primaryKey, string grainClassNamePrefix = null)
            where TGrainInterface : IGrainWithIntegerKey =>
            GetProbe<TGrainInterface>(new TestGrainIdentity(primaryKey), grainClassNamePrefix);

        public TGrainInterface GetGrain<TGrainInterface>(string primaryKey, string grainClassNamePrefix = null)
            where TGrainInterface : IGrainWithStringKey =>
            GetProbe<TGrainInterface>(new TestGrainIdentity(primaryKey), grainClassNamePrefix);

        public TGrainInterface GetGrain<TGrainInterface>(Guid primaryKey, string keyExtension,
            string grainClassNamePrefix = null)
            where TGrainInterface : IGrainWithGuidCompoundKey =>
            GetProbe<TGrainInterface>(new TestGrainIdentity(primaryKey, keyExtension), grainClassNamePrefix);

        public TGrainInterface GetGrain<TGrainInterface>(long primaryKey, string keyExtension,
            string grainClassNamePrefix = null)
            where TGrainInterface : IGrainWithIntegerCompoundKey =>
            GetProbe<TGrainInterface>(new TestGrainIdentity(primaryKey, keyExtension), grainClassNamePrefix);

        public TGrainInterface GetGrain<TGrainInterface>(Type grainInterfaceType, Guid grainPrimaryKey)
            where TGrainInterface : IGrain =>
            throw new NotImplementedException();

        public TGrainInterface GetGrain<TGrainInterface>(Type grainInterfaceType, long grainPrimaryKey)
            where TGrainInterface : IGrain =>
            throw new NotImplementedException();

        public TGrainInterface GetGrain<TGrainInterface>(Type grainInterfaceType, string grainPrimaryKey)
            where TGrainInterface : IGrain =>
            throw new NotImplementedException();

        public TGrainInterface GetGrain<TGrainInterface>(Type grainInterfaceType, Guid grainPrimaryKey, string keyExtension)
            where TGrainInterface : IGrain =>
            throw new NotImplementedException();

        public TGrainInterface GetGrain<TGrainInterface>(Type grainInterfaceType, long grainPrimaryKey, string keyExtension)
            where TGrainInterface : IGrain =>
            throw new NotImplementedException();

        public IGrain GetGrain(Type grainInterfaceType, Guid grainPrimaryKey) =>
            throw new NotImplementedException();

        public IGrain GetGrain(Type grainInterfaceType, long grainPrimaryKey) =>
            throw new NotImplementedException();

        public IGrain GetGrain(Type grainInterfaceType, string grainPrimaryKey) =>
            throw new NotImplementedException();

        public IGrain GetGrain(Type grainInterfaceType, Guid grainPrimaryKey, string keyExtension) =>
            throw new NotImplementedException();

        public IGrain GetGrain(Type grainInterfaceType, long grainPrimaryKey, string keyExtension) =>
            throw new NotImplementedException();

        internal Mock<T> AddProbe<T>(IGrainIdentity identity, string classPrefix = null)
            where T : class, IGrain
        {
            var key = GetKey(identity, typeof(T), classPrefix);
            var mock = new Mock<T>();
            _probes.Add(key, mock.Object);
            return mock;
        }

        internal void AddProbe<T>(Func<IGrainIdentity, T> factory) where T : class, IGrain => _probeFactories.Add(typeof(T), factory);

        internal void AddProbe<T>(Func<IGrainIdentity, IMock<T>> factory)
            where T : class, IGrain
        {
            var adaptedFactory = new Func<IGrainIdentity, T>(grainIdentity => factory(grainIdentity)?.Object);
            AddProbe<T>(adaptedFactory);
        }

        private static string GetKey(IGrainIdentity identity, Type stateType, string classPrefix = null) =>
            classPrefix == null
                ? $"{stateType.FullName}-{identity.IdentityString}"
                : $"{stateType.FullName}-{classPrefix}-{identity.IdentityString}";

        private T GetProbe<T>(IGrainIdentity identity, string grainClassNamePrefix)
            where T : IGrain
        {
            var key = GetKey(identity, typeof(T), grainClassNamePrefix);
            if (_probes.TryGetValue(key, out var grain))
            {
                return (T)grain;
            }

            //If using strict grain probes, throw the exception
            if (_options.StrictGrainProbes)
            {
                throw new Exception($"Probe {identity.IdentityString} does not exist for type {typeof(T).Name}. " +
                    "Ensure that it is added before the grain is tested.");
            }
            else
            {
                if (_probeFactories.TryGetValue(typeof(T), out var factory))
                {
                    grain = factory(identity);
                }
                else
                {
                    var mock = Activator.CreateInstance(typeof(Mock<>).MakeGenericType(typeof(T))) as IMock<IGrain>;
                    grain = mock?.Object;
                }

                //Save the newly created grain for the next call
                _probes.Add(key, grain);
            }

            return (T)grain;
        }
    }
}
