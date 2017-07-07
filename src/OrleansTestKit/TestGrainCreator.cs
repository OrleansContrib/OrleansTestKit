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
        private readonly MethodInfo _setStorageMethod;

        public TestGrainCreator(IGrainRuntime runtime)
        {
            _runtime = runtime;
            _activator = new DefaultGrainActivator();
            _runtimeProperty = typeof(Grain).GetProperty("Runtime", BindingFlags.Instance | BindingFlags.NonPublic);
            _identityField = typeof(Grain).GetField("Identity", BindingFlags.Instance | BindingFlags.NonPublic);
            _setStorageMethod = typeof(Grain<>).GetInterfaces().First(i => i.Name == "IStatefulGrain").GetMethod("SetStorage");
        }

        public Grain CreateGrainInstance(IGrainActivationContext context)
        {
            var grain = (Grain)_activator.Create(context);

            //Set the runtime and identity. This is equivalent to what Orleans' GrainCreator does
            //when creating new grains. It is messier but easier than trying to wrangle the values
            //in via a constructor which may or may exist on types inheriting from Grain.

            _runtimeProperty.SetValue(grain, _runtime);
            _identityField.SetValue(grain, context.GrainIdentity);

            return grain;
        }

        public Grain CreateGrainInstance(IGrainActivationContext context, Type stateType, IStorage storage)
        {
            var grain = CreateGrainInstance(context);
            var baseType = context.GrainType.BaseType;

            if(baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(Grain<>))
            {
                //Set the state value
                var stateProperty = baseType.GetProperty("State", BindingFlags.Instance | BindingFlags.NonPublic);
                var stateValue = Activator.CreateInstance(stateType);

                stateProperty.SetValue(grain, stateValue);

                //Set the storage provider
                _setStorageMethod.Invoke(grain, new object[] { storage });
            }

            return grain;
        }
    }
}
