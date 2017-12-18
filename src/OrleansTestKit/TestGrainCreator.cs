using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Orleans.Core;
using Orleans.Runtime;

namespace Orleans.TestKit
{
    public class TestGrainCreator
    {
        private readonly IGrainActivator _activator;
        private readonly IGrainRuntime _runtime;
        private readonly PropertyInfo _runtimeProperty;
        private readonly FieldInfo _identityField;

        public TestGrainCreator(IGrainRuntime runtime, IServiceProvider serviceProvider)
        {
            _runtime = runtime;
            _activator = new DefaultGrainActivator(serviceProvider);
            _runtimeProperty = typeof(Grain).GetProperty("Runtime", BindingFlags.Instance | BindingFlags.NonPublic);
            _identityField = typeof(Grain).GetField("Identity", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public Grain CreateGrainInstance(IGrainActivationContext context)
        {
            var grain = (Grain)_activator.Create(context);

            var participant = grain as ILifecycleParticipant<IGrainLifecycle>;
            participant?.Participate(context.ObservableLifecycle);

            //Set the runtime and identity. This is equivalent to what Orleans' GrainCreator does
            //when creating new grains. It is messier but easier than trying to wrangle the values
            //in via a constructor which may or may exist on types inheriting from Grain.
            _runtimeProperty.SetValue(grain, _runtime);
            _identityField.SetValue(grain, context.GrainIdentity);

            return grain;
        }
    }
}
