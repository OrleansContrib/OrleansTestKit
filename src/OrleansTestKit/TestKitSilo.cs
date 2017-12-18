using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Orleans.Core;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans.TestKit.Reminders;
using Orleans.TestKit.Services;
using Orleans.TestKit.Storage;
using Orleans.TestKit.Streams;
using Orleans.TestKit.Timers;

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

        private readonly TestGrainCreator _grainCreator;

        private readonly TestGrainRuntime _grainRuntime;

        private readonly TestGrainLifecycle _grainLifecycle = new TestGrainLifecycle();

        /// <summary>
        /// Silo service provider used when creating new grain instances.
        /// </summary>
        /// <returns></returns>
        public TestServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Silo grain factory used by the test grain with creating other grains
        /// This should only be used by the grain, not any test code.
        /// </summary>
        public TestGrainFactory GrainFactory { get; }

        /// <summary>
        /// Manages all test silo timers.
        /// </summary>
        public TestTimerRegistry TimerReistry { get; }

        /// <summary>
        /// Manages all test silo reminders
        /// </summary>
        /// <returns></returns>
        public TestReminderRegistry ReminderRegistry { get; }

        /// <summary>
        /// Manages all test silo streams
        /// </summary>
        /// <returns></returns>
        public TestStreamProviderManager StreamProviderManager { get; }

        /// <summary>
        /// Manages all test silo storage
        /// </summary>
        /// <returns></returns>
        public StorageManager StorageManager { get; }

        /// <summary>
        /// Configures the test silo
        /// </summary>
        /// <returns></returns>
        public TestKitOptions Options { get; } = new TestKitOptions();

        public TestKitSilo()
        {
            GrainFactory = new TestGrainFactory(Options);

            ServiceProvider = new TestServiceProvider(Options);

            StorageManager = new StorageManager();

            TimerReistry = new TestTimerRegistry();

            ReminderRegistry = new TestReminderRegistry();

            StreamProviderManager = new TestStreamProviderManager(Options);

            ServiceProvider.AddService<IKeyedServiceCollection<string, IStreamProvider>>(StreamProviderManager);

            _grainRuntime = new TestGrainRuntime(GrainFactory, TimerReistry, ReminderRegistry, ServiceProvider, StorageManager);

            _grainCreator = new TestGrainCreator(_grainRuntime, ServiceProvider);
        }

        #region CreateGrains 

        public T CreateGrain<T>(long id) where T : Grain, IGrainWithIntegerKey
            => CreateGrain<T>(new TestGrainIdentity(id));

        public T CreateGrain<T>(Guid id) where T : Grain, IGrainWithGuidKey
            => CreateGrain<T>(new TestGrainIdentity(id));

        public T CreateGrain<T>(string id) where T : Grain, IGrainWithStringKey
            => CreateGrain<T>(new TestGrainIdentity(id));

        private T CreateGrain<T>(IGrainIdentity identity) where T : Grain
        {
            if (_isGrainCreated)
                throw new Exception(
                    "A grain has already been created in this silo. Only 1 grain per test silo should every be created. Add grain probes for supporting grains.");

            _isGrainCreated = true;

            Grain grain;

            var grainContext = new TestGrainActivationContext
            {
                ActivationServices = ServiceProvider,
                GrainIdentity = identity,
                GrainType = typeof(T),
                ObservableLifecycle = _grainLifecycle,
            };

            //Create a stateless grain
            grain = _grainCreator.CreateGrainInstance(grainContext) as T;

            if (grain == null)
                throw new Exception($"Unable to instantiate grain {typeof(T)} properly");

            //Check if there are any reminders for this grain
            var remindable = grain as IRemindable;

            //Set the reminder target
            if (remindable != null)
                ReminderRegistry.SetGrainTarget(remindable);

            //Trigger the lifecycle hook that will get the grain's state from the runtime
            _grainLifecycle.TriggerStart();

            return grain as T;
        }

        #endregion CreateGrains

        #region Verifies

        public void VerifyRuntime(Expression<Action<IGrainRuntime>> expression, Func<Times> times)
        {
            _grainRuntime.Mock.Verify(expression, times);
        }

        #endregion Verifies

        /// <summary>
        /// Deactivate the given <see cref="Grain"/>
        /// </summary>
        /// <param name="grain">Grain to Deactivate</param>
        public void Deactivate(Grain grain)
        {
            _grainLifecycle.TriggerStop();
        }
    }
}
