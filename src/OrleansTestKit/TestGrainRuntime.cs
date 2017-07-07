using System;
using Moq;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans.TestKit.Loggers;
using Orleans.Timers;

namespace Orleans.TestKit
{
    internal class TestGrainRuntime : IGrainRuntime
    {
        private readonly TestLogManager _logManager = new TestLogManager();

        public readonly Mock<IGrainRuntime> Mock = new Mock<IGrainRuntime>();

        public Guid ServiceId { get; } = Guid.NewGuid();

        public string SiloIdentity { get; } = "TestSilo";

        public IGrainFactory GrainFactory { get; }

        public IStreamProviderManager StreamProviderManager { get; }

        public ITimerRegistry TimerRegistry { get; }

        public IReminderRegistry ReminderRegistry { get; }

        public IServiceProvider ServiceProvider { get; }

        public SiloAddress SiloAddress { get { return SiloAddress.Zero; } }

        public TestGrainRuntime(IGrainFactory grainFactory, ITimerRegistry timerRegistry, IStreamProviderManager streamProviderManager)
        {
            GrainFactory = grainFactory;
            TimerRegistry = timerRegistry;
            StreamProviderManager = streamProviderManager;
        }

        public Orleans.Runtime.Logger GetLogger(string loggerName) => _logManager.GetLogger(loggerName);

        public void DeactivateOnIdle(Grain grain)
        {
            Mock.Object.DeactivateOnIdle(grain);
        }

        public void DelayDeactivation(Grain grain, TimeSpan timeSpan)
        {
            Mock.Object.DelayDeactivation(grain, timeSpan);
        }
    }
}