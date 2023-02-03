using System.Threading;
using System.Threading.Tasks;
using Orleans;
using TestInterfaces;

namespace TestGrains
{
    public sealed class LifecycleGrain : Grain, ILifecycleGrain
    {
        public int ActivateCount { get; set; }

        public int DeactivateCount { get; set; }

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            ActivateCount++;
            return base.OnActivateAsync(cancellationToken);
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            DeactivateCount++;
            return base.OnDeactivateAsync(reason, cancellationToken);
        }
    }
}
