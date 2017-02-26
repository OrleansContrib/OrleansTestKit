using System.Threading.Tasks;
using Orleans;
using TestInterfaces;

namespace TestGrains
{
    public sealed class LifecycleGrain : Grain, ILifecycleGrain
    {
        public bool IsActivated { get; set; }

        public bool IsDeactivated { get; set; }

        public override Task OnActivateAsync()
        {
            IsActivated = true;
            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            IsDeactivated = true;
            return base.OnDeactivateAsync();
        }
    }
}