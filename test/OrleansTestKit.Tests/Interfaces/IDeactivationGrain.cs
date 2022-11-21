using System;
using System.Threading.Tasks;
using Orleans;

namespace TestInterfaces
{
    public interface IDeactivationGrain : IGrainWithIntegerKey
    {
        public Task DeactivateOnIdle();

        public Task DelayDeactivation(TimeSpan timeSpan);
    }
}
