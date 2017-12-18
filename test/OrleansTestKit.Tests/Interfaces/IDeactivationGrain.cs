using System;
using System.Threading.Tasks;
using Orleans;

namespace TestInterfaces
{
    public interface IDeactivationGrain : IGrainWithIntegerKey
    {
        Task DeactivateOnIdle();

        Task DelayDeactivation(TimeSpan timeSpan);
    }
}