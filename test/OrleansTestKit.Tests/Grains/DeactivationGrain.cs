using System;
using System.Threading.Tasks;
using Orleans;
using TestInterfaces;

namespace TestGrains
{
    public class DeactivationGrain : Grain, IDeactivationGrain
    {
        public Task DeactivateOnIdle() {
            base.DeactivateOnIdle();

            return Task.CompletedTask;
        }

        public Task DelayDeactivation(TimeSpan timeSpan)
        {
            base.DelayDeactivation(timeSpan);

            return Task.CompletedTask;
        }
    }
}
