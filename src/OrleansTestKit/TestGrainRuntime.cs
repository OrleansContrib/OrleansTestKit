using System;
using Moq;
using Orleans.Core;
using Orleans.Runtime;
using Orleans.TestKit.Storage;
using Orleans.Timers;

namespace Orleans.TestKit
{
    internal class TestGrainRuntime : IGrainRuntime
    {
        public readonly Mock<IGrainRuntime> Mock = new Mock<IGrainRuntime>();

        private readonly StorageManager _storageManager;

        public string ServiceId { get; } = "TestService";

        public string SiloIdentity { get; } = "TestSilo";

        public IGrainFactory GrainFactory { get; }

        public ITimerRegistry TimerRegistry { get; }

        public IReminderRegistry ReminderRegistry { get; }

        public IServiceProvider ServiceProvider { get; }

        public SiloAddress SiloAddress { get { return SiloAddress.Zero; } }

        public TestGrainRuntime(IGrainFactory grainFactory,
            ITimerRegistry timerRegistry,
            IReminderRegistry reminderRegistry,
            IServiceProvider serviceProvider,
            StorageManager storageManager)
        {
            GrainFactory = grainFactory;
            TimerRegistry = timerRegistry;
            ReminderRegistry = reminderRegistry;
            ServiceProvider = serviceProvider;
            _storageManager = storageManager;
        }

        public void DeactivateOnIdle(Grain grain)
        {
            Mock.Object.DeactivateOnIdle(grain);
        }

        public void DelayDeactivation(Grain grain, TimeSpan timeSpan)
        {
            Mock.Object.DelayDeactivation(grain, timeSpan);
        }

        public IStorage<TGrainState> GetStorage<TGrainState>(Grain grain) where TGrainState : new() =>
            _storageManager.GetStorage<TGrainState>();
    }
}
