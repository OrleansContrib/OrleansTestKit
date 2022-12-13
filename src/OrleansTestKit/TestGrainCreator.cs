using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Core;
using Orleans.Runtime;
using Orleans.TestKit.Storage;

// Took the Get Factory Method from the Orleans Implementation : https://github.com/dotnet/orleans/blob/10af0f4af588cd4aa45cb3e250dfbffa389d59c7/src/Orleans.Runtime/Facet/ConstructorArgumentFactory.cs

namespace Orleans.TestKit
{

    public sealed class TestGrainCreator
    {
        private const string GRAINCONTEXT_PROPERTYNAME = "GrainContext";
        private const string RUNTIME_PROPERTYNAME = "Runtime";
        private static readonly Type FacetMarkerInterfaceType = typeof(IFacetMetadata);
        private static readonly MethodInfo GetFactoryMethod = typeof(TestGrainCreator).GetMethod("GetFactory", BindingFlags.NonPublic | BindingFlags.Static);

        private readonly PropertyInfo _contextProperty;
        private readonly IGrainRuntime _runtime;
        private readonly IServiceProvider _serviceProvider;
        private readonly PropertyInfo _runtimeProperty;

        public TestGrainCreator(IGrainRuntime runtime, IServiceProvider serviceProvider)
        {
            _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(runtime));
            _contextProperty = typeof(Grain).GetProperty(GRAINCONTEXT_PROPERTYNAME, BindingFlags.Instance | BindingFlags.NonPublic);
            _runtimeProperty = typeof(Grain).GetProperty(RUNTIME_PROPERTYNAME, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public Grain CreateGrainInstance<T>(IGrainContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }


            var constructorParam = GetConstructorParameters(typeof(T), context);

            var factory = ActivatorUtilities.CreateFactory(typeof(T), constructorParam.Types.ToArray());
            var grain = (Grain)factory.Invoke(_serviceProvider, constructorParam.Instances.ToArray());
            var participant = grain as ILifecycleParticipant<IGrainLifecycle>;

            participant?.Participate(context.ObservableLifecycle);

            //Set the runtime and identity. This is equivalent to what Orleans' GrainCreator does
            //when creating new grains. It is messier but easier than trying to wrangle the values
            //in via a constructor which may or may exist on types inheriting from Grain.
            var runtimeBackfield = _runtimeProperty.DeclaringType.GetField($"<{_runtimeProperty.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            runtimeBackfield.SetValue(grain, _runtime);
            _contextProperty.SetValue(grain, context);


            return grain;
        }

        /// <summary>
        /// Creates the Constructor Parameters for the given Grain Type
        /// </summary>
        /// <param name="grainType">Grain to create to</param>
        /// <param name="context">Grain Context of the Grain</param>
        /// <returns>Created instances of the Constructor Parameters, with their type</returns>
        private (object[] Instances, Type[] Types) GetConstructorParameters(Type grainType, IGrainContext context)
        {
            var constructors = grainType.GetConstructors();

            var greedyConstructor = constructors.Max(x => x.GetParameters().Length);

            var instances = new List<object>();
            var types = new HashSet<Type>();

            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();

                foreach (var parameter in parameters)
                {
                    var interfaces = parameter.ParameterType.GetInterfaces();

                    var attribute = parameter
                        .GetCustomAttributes()
                        .FirstOrDefault(x => FacetMarkerInterfaceType.IsInstanceOfType(x));

                    // avoid duplicate instances of same parameter type
                    // when multiple constructors are available
                    if (types.Contains(parameter.ParameterType))
                    {
                        continue;
                    }

                    if (attribute != null)
                    {
                        MethodInfo getFactory = GetFactoryMethod.MakeGenericMethod(attribute.GetType());
                        var argumentFactory = (Factory<IGrainContext, object>)getFactory.Invoke(this, new object[] { _serviceProvider, parameter, attribute, grainType });
                        // cache argument factory
                        instances.Add(argumentFactory(context));
                        // cache argument type
                        types.Add(parameter.ParameterType);
                        continue;
                    }
                    else
                    {
                        foreach (var interf in interfaces)
                        {
                            if (interf.IsGenericType && interf.GetGenericTypeDefinition() == typeof(IStorage<>))
                            {
                                CreateIStorageParameter(parameter, ref instances, ref types);
                                continue;
                            }
                        }
                    }
                }
            }

            return (instances.ToArray(), types.ToArray());
        }

        private void CreateIStorageParameter(ParameterInfo parameter, ref List<object> instances, ref HashSet<Type> types)
        {
            var stateType = parameter.ParameterType.GenericTypeArguments[0];
            var storageType = typeof(TestStorage<>).MakeGenericType(stateType);

            try
            {
                var state = Activator.CreateInstance(stateType);
                var storage = Activator.CreateInstance(storageType, state);
                // cache argument factory
                instances.Add(storage);
                // cache argument type
                types.Add(parameter.ParameterType);
            }
            catch (Exception ex)
            {
                throw new NotSupportedException($"Could not invoke Instance {nameof(stateType)}", ex);
            }
        }

        /// <summary>
        /// https://github.com/dotnet/orleans/blob/10af0f4af588cd4aa45cb3e250dfbffa389d59c7/src/Orleans.Runtime/Facet/ConstructorArgumentFactory.cs
        /// </summary>
        /// <typeparam name="TMetadata"></typeparam>
        /// <param name="services"></param>
        /// <param name="parameter"></param>
        /// <param name="metadata"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="OrleansException"></exception>
        private static Factory<IGrainContext, object> GetFactory<TMetadata>(IServiceProvider services, ParameterInfo parameter, IFacetMetadata metadata, Type type)
             where TMetadata : IFacetMetadata
        {
            var factoryMapper = services.GetService<IAttributeToFactoryMapper<TMetadata>>();
            if (factoryMapper == null)
                throw new OrleansException($"Missing attribute mapper for attribute {metadata.GetType()} used in grain constructor for grain type {type}.");
            Factory<IGrainContext, object> factory = factoryMapper.GetFactory(parameter, (TMetadata)metadata);
            if (factory == null)
                throw new OrleansException($"Attribute mapper {factoryMapper.GetType()} failed to create a factory for grain type {type}.");
            return factory;
        }
    }
}
