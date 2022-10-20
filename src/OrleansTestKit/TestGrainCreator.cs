using System;
using System.Reflection;
using Castle.Core;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration;
using Orleans.Runtime;
using Orleans.TestKit.Storage;

namespace Orleans.TestKit
{
    public sealed class TestGrainCreator
    {

        private readonly PropertyInfo _contextProperty;

        private readonly IGrainRuntime _runtime;
        private readonly IServiceProvider _serviceProvider;
        private readonly PropertyInfo _runtimeProperty;

        public TestGrainCreator(IGrainRuntime runtime, IServiceProvider serviceProvider)
        {
            _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(runtime));
            _contextProperty = typeof(Grain).GetProperty("GrainContext", BindingFlags.Instance | BindingFlags.NonPublic);
            _runtimeProperty = typeof(Grain).GetProperty("Runtime", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public Grain CreateGrainInstance<T>(IGrainContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var factory = ActivatorUtilities.CreateFactory(typeof(T),Array.Empty<Type>());

            var grain = (Grain)factory.Invoke(_serviceProvider,null);

            var participant = grain as ILifecycleParticipant<IGrainLifecycle>;
            participant?.Participate(context.ObservableLifecycle);

            //Set the runtime and identity. This is equivalent to what Orleans' GrainCreator does
            //when creating new grains. It is messier but easier than trying to wrangle the values
            //in via a constructor which may or may exist on types inheriting from Grain.
            var runtimeBackfield = _runtimeProperty.DeclaringType.GetField($"<{_runtimeProperty.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            runtimeBackfield.SetValue(grain, _runtime);
            _contextProperty.SetValue(grain, context);

            var baseType = typeof(T).BaseType;

            if(baseType.BaseType == typeof(Grain) && baseType.GenericTypeArguments.Length == 1)
            {
                var stateType = baseType.GenericTypeArguments[0];
                var storageType = typeof(TestStorage<>).MakeGenericType(stateType);

                var storageField = baseType.GetField("storage", BindingFlags.Instance | BindingFlags.NonPublic);

                if(storageField != null)
                {
                    try
                    {
                        var state = Activator.CreateInstance(stateType);
                        var storage = Activator.CreateInstance(storageType, state);
                        storageField.SetValue(grain, storage);
                    }
                    catch(Exception ex)
                    {
                        throw new NotSupportedException($"Could not invoke State {nameof(stateType)}", ex);
                    }
                   
                }
            }

            return grain;
        }
    }
}
