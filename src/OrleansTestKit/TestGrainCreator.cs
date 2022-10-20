using System;
using System.Reflection;
using Orleans.Runtime;

namespace Orleans.TestKit
{
    public sealed class TestGrainCreator
    {
        private readonly GrainContextActivator _activator;

        private readonly FieldInfo _identityField;

        private readonly IGrainRuntime _runtime;

        private readonly PropertyInfo _runtimeProperty;

        public TestGrainCreator(IGrainRuntime runtime, IServiceProvider serviceProvider, GrainContextActivator contextActivator)
        {
            _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
            _activator =  new DefaultGrainActivator(serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)));
            _identityField = typeof(Grain).GetField("Identity", BindingFlags.Instance | BindingFlags.NonPublic);
            _runtimeProperty = typeof(Grain).GetProperty("Runtime", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public Grain CreateGrainInstance(IGrainContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var grain = (Grain)_activator.Create(context);

            var participant = grain as ILifecycleParticipant<IGrainLifecycle>;
            participant?.Participate(context.ObservableLifecycle);

            //Set the runtime and identity. This is equivalent to what Orleans' GrainCreator does
            //when creating new grains. It is messier but easier than trying to wrangle the values
            //in via a constructor which may or may exist on types inheriting from Grain.
            _runtimeProperty.SetValue(grain, _runtime);
            _identityField.SetValue(grain, context.GrainId);

            return grain;
        }
    }
}
