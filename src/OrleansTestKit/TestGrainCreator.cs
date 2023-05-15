using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Core;
using Orleans.Runtime;
using Orleans.TestKit.Reminders;
using Orleans.TestKit.Storage;

// Took the Get Factory Method from the Orleans Implementation :
// https://github.com/dotnet/orleans/blob/10af0f4af588cd4aa45cb3e250dfbffa389d59c7/src/Orleans.Runtime/Facet/ConstructorArgumentFactory.cs
namespace Orleans.TestKit;

/// <summary>
/// Contains necessary logic for creating grains in a unit test context -- emulates the Orleans runtime behavior of triggering lifecycle events, etc.
/// </summary>
public sealed class TestGrainCreator
{
    private static readonly MethodInfo GetFactoryMethod = typeof(TestGrainCreator).GetMethod("GetFactory", BindingFlags.NonPublic | BindingFlags.Static)!;

    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestGrainCreator"/> class.
    /// </summary>
    /// <param name="runtime">The grain runtime.</param>
    /// <param name="serviceProvider">Service provider.</param>
    /// <exception cref="ArgumentNullException">Both runtime and services should be not null.</exception>
    public TestGrainCreator(IGrainRuntime runtime, IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(runtime));
    }

    /// <summary>
    /// Create a grain instance emulating Orleans runtime behavior.
    /// </summary>
    /// <typeparam name="T">The grain instance type to create.</typeparam>
    /// <param name="context">The grain context.</param>
    /// <returns>The grain.</returns>
    /// <exception cref="ArgumentNullException">context is required.</exception>
    public Grain CreateGrainInstance<T>(IGrainContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        using var runtimeContextScope = RuntimeContextManager.StartExecutionContext(context);

        var (instances, types) = GetConstructorParameters(typeof(T), context);
        var factory = ActivatorUtilities.CreateFactory(typeof(T), types.ToArray());
        var grain = (Grain)factory.Invoke(_serviceProvider, instances.ToArray());

        var participant = grain as ILifecycleParticipant<IGrainLifecycle>;

        participant?.Participate(context.ObservableLifecycle);

        return grain;
    }

    /// <summary>
    ///     https://github.com/dotnet/orleans/blob/10af0f4af588cd4aa45cb3e250dfbffa389d59c7/src/Orleans.Runtime/Facet/ConstructorArgumentFactory.cs.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via reflection")]
    private static Factory<IGrainContext, object> GetFactory<TMetadata>(IServiceProvider services, ParameterInfo parameter, IFacetMetadata metadata, Type type)
         where TMetadata : IFacetMetadata
    {
        var factoryMapper = services.GetService<IAttributeToFactoryMapper<TMetadata>>()
            ?? throw new OrleansException($"Missing attribute mapper for attribute {metadata.GetType()} used in grain constructor for grain type {type}.");

        var factory = factoryMapper.GetFactory(parameter, (TMetadata)metadata)
            ?? throw new OrleansException($"Attribute mapper {factoryMapper.GetType()} failed to create a factory for grain type {type}.");

        return factory;
    }

    private static void CreateIStorageParameter(ParameterInfo parameter, ICollection<object> instances, HashSet<Type> types)
    {
        var stateType = parameter.ParameterType.GenericTypeArguments[0];
        var storageType = typeof(TestStorage<>).MakeGenericType(stateType);

        try
        {
            var state = Activator.CreateInstance(stateType);
            var storage = Activator.CreateInstance(storageType, state)!;

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

    /// <summary>Creates the Constructor Parameters for the given Grain Type.</summary>
    /// <param name="grainType">Grain to create to.</param>
    /// <param name="context">Grain Context of the Grain.</param>
    /// <returns>Created instances of the Constructor Parameters, with their type.</returns>
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
                    .FirstOrDefault(x => typeof(IFacetMetadata).IsInstanceOfType(x));

                // avoid duplicate instances of same parameter type when multiple constructors are available
                if (types.Contains(parameter.ParameterType))
                {
                    continue;
                }

                if (attribute != null)
                {
                    var getFactory = GetFactoryMethod.MakeGenericMethod(attribute.GetType());
                    var argumentFactory = (Factory<IGrainContext, object>)getFactory.Invoke(this, new object[] { _serviceProvider, parameter, attribute, grainType })!;

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
                            CreateIStorageParameter(parameter, instances, types);
                            continue;
                        }
                    }
                }
            }
        }

        return (instances.ToArray(), types.ToArray());
    }
}
