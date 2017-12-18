using System.Threading.Tasks;
using Orleans;
using TestInterfaces;

namespace TestGrains
{
    public sealed class LifecycleGrain : Grain, ILifecycleGrain
    {
        public int ActivateCount { get; set; }

        public int DeactivateCount { get; set; }

        public override Task OnActivateAsync()
        {
            ActivateCount++;
            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            DeactivateCount++;
            return base.OnDeactivateAsync();
        }
    }
}