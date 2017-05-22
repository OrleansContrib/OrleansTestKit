using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Moq;
using Orleans.Core;
using Orleans.Runtime;
using Orleans.TestKit.Reminders;
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

        public TestServiceProvider ServiceProvider { get; }

        private readonly TestGrainFactory _grainFactory;

        private readonly TestTimerRegistry _timerRegistry;

        public TestReminderRegistry ReminderRegistry { get; }

        private readonly TestStreamProviderManager _streamProviderManager;

        private readonly GrainStateManager _grainStateManager = new GrainStateManager();

        private readonly StorageManager _storageManager = new StorageManager();

        public TestKitOptions Options { get; } = new TestKitOptions();

        public TestKitSilo()
        {
            _grainFactory = new TestGrainFactory(Options);

            ServiceProvider = new TestServiceProvider(Options);

            _timerRegistry = new TestTimerRegistry();

            ReminderRegistry = new TestReminderRegistry();

            _streamProviderManager = new TestStreamProviderManager(Options);

            _grainRuntime = new TestGrainRuntime(_grainFactory, _timerRegistry, _streamProviderManager, ReminderRegistry, ServiceProvider);

            _grainCreator = new TestGrainCreator(_grainRuntime);
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

            var grainContext = new TestGrainActivationContext() 
            {
                ActivationServices = ServiceProvider,
                GrainIdentity = identity,
                GrainType = typeof(T),
            };

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
                grain = _grainCreator.CreateGrainInstance(grainContext, stateType, storage);

                if (grain == null)
                    throw new Exception($"Unable to instantiate stateful grain {typeof(T)} properly");

                var stateProperty = TypeHelper.GetProperty(typeof(T), "State");

                var state = stateProperty?.GetValue(grain);

                _grainStateManager.AddState(grain, state);
            }
            else
            {
                //Create a stateless grain
                grain = _grainCreator.CreateGrainInstance(grainContext) as T;

                if (grain == null)
                    throw new Exception($"Unable to instantiate grain {typeof(T)} properly");
            }

            //Check if there are any reminders for this grain
            var remindable = grain as IRemindable;

            //Set the reminder target
            if (remindable != null)
                ReminderRegistry.SetGrainTarget(remindable);

            //Emulate the grain's lifecycle
            grain.OnActivateAsync().Wait(1000);

            return grain as T;
        }        

        #endregion

        #region Timers

        public void FireTimer(int index)
        {
            _timerRegistry.Fire(index);
        }

        public void FireAllTimers()
        {
            _timerRegistry.FireAll();
        }

        #endregion

        #region Reminders

        public Task FireReminder(string reminderName, TickStatus tickStatus = new TickStatus()) => ReminderRegistry.FireReminder(reminderName, tickStatus);

        public Task FireAllReminders(TickStatus tickStatus = new TickStatus()) => ReminderRegistry.FireAllReminders(tickStatus);

        #endregion

        #region Streams

        public TestStream<T> AddStreamProbe<T>() where T : class => AddStreamProbe<T>(Guid.Empty);

        public TestStream<T> AddStreamProbe<T>(Guid id) where T : class
            => AddStreamProbe<T>(id, typeof(T).Name);

        public TestStream<T> AddStreamProbe<T>(Guid id, string streamNamespace) where T : class
            => AddStreamProbe<T>(id, streamNamespace, "Default");

        public TestStream<T> AddStreamProbe<T>(Guid id, string streamNamespace, string providerName) where T : class
            => _streamProviderManager.AddStreamProbe<T>(id, streamNamespace, providerName);

        

        #endregion

        #region Probes

        public Mock<T> AddProbe<T>(long id) where T : class, IGrain
            => _grainFactory.AddProbe<T>(new TestGrainIdentity(id));

        public Mock<T> AddProbe<T>(Guid id) where T : class, IGrain
            => _grainFactory.AddProbe<T>(new TestGrainIdentity(id));

        public Mock<T> AddProbe<T>(string id) where T : class, IGrain
            => _grainFactory.AddProbe<T>(new TestGrainIdentity(id));

        public void AddProbeFactory<T>(Func<IGrainIdentity, IMock<T>> factory) where T : class, IGrain
            => _grainFactory.AddProbeFactory<T>(factory);

        #endregion

        #region Storage & State

        public TestStorageStats Storage<T>(T grain) where T : Grain
            => _storageManager.GetStorageStats(grain);

        public TState State<TState>(Grain grain) where TState : class =>
            _grainStateManager.GetState<TState>(grain);

        #endregion

        #region Services

        public Mock<T> AddServiceProbe<T>() where T : class
            => ServiceProvider.AddServiceProbe<T>();

        #endregion

        #region Verifies

        public void VerifyRuntime(Expression<Action<IGrainRuntime>> expression, Func<Times> times)
        {
            _grainRuntime.Mock.Verify(expression, times);
        }

        #endregion

        /// <summary>
        /// Deactivate the given <see cref="Grain"/>
        /// </summary>
        /// <param name="grain">Grain to Deactivate</param>
        public void Deactivate(Grain grain)
        {
            grain.OnDeactivateAsync().Wait(1000);
        }
    }
}
