using System;
using System.Threading.Tasks;
using Orleans;
using TestInterfaces;

namespace TestGrains
{
    public class DeactivationGrain : Grain, IDeactivationGrain
    {
        public new Task DeactivateOnIdle()
        {
            base.DeactivateOnIdle();

            return TaskDone.Done;
        }

        public new Task DelayDeactivation(TimeSpan timeSpan)
        {
            base.DelayDeactivation(timeSpan);

            return TaskDone.Done;
        }
    }
}