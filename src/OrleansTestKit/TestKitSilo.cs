using System;
using System.Reflection;
using Moq;
using Orleans.Core;
using Orleans.Runtime;
using Orleans.TestKit.Storage;
using Orleans.TestKit.Streams;

namespace Orleans.TestKit
{
    public sealed class TestKitSilo
    {
        /// <summary>
        /// Flag indicating if a grain has already been created in this test silo.
        /// Since this is all mocked up only the grain under test should be real, therefore
        /// only a single grain should ever be created.
        /// </summary>
        private bool _isGrainCreated;

        private readonly GrainCreator _grainCreator;

        private readonly TestGrainFactory _grainFactory;

        private readonly TestStreamProviderManager _streamProviderManager;

        private readonly GrainStateManager _grainStateManager = new GrainStateManager();

        private readonly StorageManager _storageManager = new StorageManager();

        public TestKitOptions Options { get; } = new TestKitOptions();

        public TestKitSilo()
        {
            _grainFactory = new TestGrainFactory(Options);

            _streamProviderManager = new TestStreamProviderManager(Options);

            var grainRuntime = new TestGrainRuntime(_grainFactory, _streamProviderManager);

            _grainCreator = new GrainCreator(null, () => grainRuntime);
        }

        #region CreateGrains

        public T CreateGrain<T>(long id) where T : Grain, IGrainWithIntegerKey
            => CreateGrain<T>(new TestGrainIdentity(id));

        public T CreateGrain<T>(Guid id) where T : Grain, IGrainWithGuidKey => CreateGrain<T>(new TestGrainIdentity(id));

        public T CreateGrain<T>(string id) where T : Grain, IGrainWithStringKey
            => CreateGrain<T>(new TestGrainIdentity(id));

        private T CreateGrain<T>(IGrainIdentity identity) where T : Grain
        {
            if (_isGrainCreated)
                throw new Exception(
                    "A grain has already been created in this silo. Only 1 grain per test silo should every be created. Add grain probes for supporting grains.");

            _isGrainCreated = true;

            Grain grain;

            //Check to see if the grain is stateful
            if (typeof(T).IsSubclassOfRawGeneric(typeof(Grain<>)))
            {
                var grainGenericArgs = typeof(T).BaseType?.GenericTypeArguments;

                if (grainGenericArgs == null || grainGenericArgs.Length == 0)
                    throw new Exception($"Unable to get grain state type info for {typeof(T)}");

                //Get the state type
                var stateType = grainGenericArgs[0];

                var storage = _storageManager.AddStorage<T>(identity);

                //Create a new stateful grain
                grain = _grainCreator.CreateGrainInstance(typeof(T), identity, stateType, storage);

                if (grain == null)
                    throw new Exception($"Unable to instantiate stateful grain {typeof(T)} properly");

                var stateProperty = GetProperty(typeof(T), "State");

                var state = stateProperty?.GetValue(grain);

                _grainStateManager.AddState(grain, state);
            }
            else
            {
                //Create a stateless grain
                grain = _grainCreator.CreateGrainInstance(typeof(T), identity) as T;

                if (grain == null)
                    throw new Exception($"Unable to instantiate grain {typeof(T)} properly");
            }

            //Emulate the grain's lifecycle
            grain.OnActivateAsync();

            return grain as T;

        }

        private PropertyInfo GetProperty(Type t, string name)
        {
            var info = t.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);

            if (info == null && t.BaseType != null)
                return GetProperty(t.BaseType, name);

            return info;
        }

        #endregion

        #region Timers

        //        protected void FireAllTimers()
        //        {
        //           _grainRuntime.FireAllTimers();
        //        }


        #endregion

        #region Streams

        public TestStream<T> AddStreamProbe<T>() where T : class => AddStreamProbe<T>(Guid.Empty);

        public TestStream<T> AddStreamProbe<T>(Guid id, string streamNamespace) where T : class
            => _streamProviderManager.AddStreamProbe<T>(id, streamNamespace, "Default");

        public TestStream<T> AddStreamProbe<T>(Guid id) where T : class
            => AddStreamProbe<T>(id, typeof(T).Name);

        #endregion

        #region Probes

        public Mock<T> AddProbe<T>(long id) where T : class, IGrain
            => _grainFactory.AddProbe<T>(new TestGrainIdentity(id));

        public Mock<T> AddProbe<T>(Guid id) where T : class, IGrain
            => _grainFactory.AddProbe<T>(new TestGrainIdentity(id));

        public Mock<T> AddProbe<T>(string id) where T : class, IGrain
            => _grainFactory.AddProbe<T>(new TestGrainIdentity(id));

        #endregion

        #region Storage & State

        public TestStorageStats Storage<T>(T grain) where T : Grain
            => _storageManager.GetStorageStats(grain);

        public TState State<TState>(Grain grain) where TState : class =>
            _grainStateManager.GetState<TState>(grain);

        #endregion
    }
}