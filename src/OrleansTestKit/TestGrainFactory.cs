using System.Collections.Concurrent;
using Moq;
using Orleans.Runtime;

namespace Orleans.TestKit;

/// <summary>
/// The test grain factory
/// </summary>
/// <remarks>
/// Utilized to provide grains for both silos and the configured IClusterClient
/// </remarks>
public sealed class TestGrainFactory : IGrainFactory
{
    private readonly TestKitOptions _options;

    private readonly ConcurrentDictionary<Type, Func<IdSpan, IGrain>> _probeFactories = new();

    private readonly ConcurrentDictionary<string, IGrain> _probes = new();

    internal TestGrainFactory(TestKitOptions options)
    {
        _options = options;
    }

    public TGrainObserverInterface CreateObjectReference<TGrainObserverInterface>(IGrainObserver obj)
        where TGrainObserverInterface : IGrainObserver =>
        throw new NotImplementedException();

    public void DeleteObjectReference<TGrainObserverInterface>(IGrainObserver obj)
        where TGrainObserverInterface : IGrainObserver =>
        throw new NotImplementedException();

    public TGrainInterface GetGrain<TGrainInterface>(Guid primaryKey, string? grainClassNamePrefix = null)
        where TGrainInterface : IGrainWithGuidKey =>
        GetProbe<TGrainInterface>(GrainIdKeyExtensions.CreateGuidKey(primaryKey), grainClassNamePrefix);

    public TGrainInterface GetGrain<TGrainInterface>(long primaryKey, string? grainClassNamePrefix = null)
        where TGrainInterface : IGrainWithIntegerKey =>
        GetProbe<TGrainInterface>(GrainIdKeyExtensions.CreateIntegerKey(primaryKey), grainClassNamePrefix);

    public TGrainInterface GetGrain<TGrainInterface>(string primaryKey, string? grainClassNamePrefix = null)
        where TGrainInterface : IGrainWithStringKey =>
        GetProbe<TGrainInterface>(IdSpan.Create(primaryKey), grainClassNamePrefix);

    public TGrainInterface GetGrain<TGrainInterface>(Guid primaryKey, string? keyExtension, string? grainClassNamePrefix = null)
        where TGrainInterface : IGrainWithGuidCompoundKey =>
        GetProbe<TGrainInterface>(GrainIdKeyExtensions.CreateGuidKey(primaryKey, keyExtension), grainClassNamePrefix);

    public TGrainInterface GetGrain<TGrainInterface>(long primaryKey, string keyExtension, string? grainClassNamePrefix = null)
        where TGrainInterface : IGrainWithIntegerCompoundKey =>
        GetProbe<TGrainInterface>(GrainIdKeyExtensions.CreateIntegerKey(primaryKey, keyExtension), grainClassNamePrefix);

    public TGrainInterface GetGrain<TGrainInterface>(GrainId grainId) where TGrainInterface : IAddressable =>
        throw new NotImplementedException();

    public IAddressable GetGrain(GrainId grainId) =>
        throw new NotImplementedException();

    public IAddressable GetGrain(GrainId grainId, GrainInterfaceType interfaceType) =>
        throw new NotImplementedException();

    public IGrain GetGrain(Type grainInterfaceType, Guid grainPrimaryKey) =>
        GetProbe(grainInterfaceType, GrainIdKeyExtensions.CreateGuidKey(grainPrimaryKey), null);

    public IGrain GetGrain(Type grainInterfaceType, long grainPrimaryKey) =>
        GetProbe(grainInterfaceType, GrainIdKeyExtensions.CreateIntegerKey(grainPrimaryKey), null);

    public IGrain GetGrain(Type grainInterfaceType, string grainPrimaryKey) =>
        GetProbe(grainInterfaceType, IdSpan.Create(grainPrimaryKey), null);

    public IGrain GetGrain(Type grainInterfaceType, Guid grainPrimaryKey, string keyExtension) =>
        GetProbe(grainInterfaceType, GrainIdKeyExtensions.CreateGuidKey(grainPrimaryKey, keyExtension), null);

    public IGrain GetGrain(Type grainInterfaceType, long grainPrimaryKey, string keyExtension) =>
        GetProbe(grainInterfaceType, GrainIdKeyExtensions.CreateIntegerKey(grainPrimaryKey, keyExtension), null);

    internal Mock<T> AddProbe<T>(IdSpan identity, string? grainClassNamePrefix = null)
        where T : class, IGrain
    {
        var key = GetKey(identity, typeof(T), grainClassNamePrefix);
        var mock = new Mock<T>();
        // we expect AddProbe to be called in a serialized fashion
        if (!_probes.TryAdd(key, mock.Object))
        {
            throw new InvalidOperationException($"Probe Key: {key} already exists");
        }
        return mock;
    }

    internal void AddProbe<T>(Func<IdSpan, T> factory)
        where T : class, IGrain
    {
        // we expect AddProbe to be called in a serialized fashion
        if (!_probeFactories.TryAdd(typeof(T), factory))
        {
            throw new InvalidOperationException($"Probe Factory for {typeof(T).Name} already exists");
        }
    }

    internal void AddProbe<T>(Func<IdSpan, IMock<T>> factory)
        where T : class, IGrain
    {
        var adaptedFactory = new Func<IdSpan, T>(grainIdentity => factory(grainIdentity).Object);
        AddProbe<T>(adaptedFactory);
    }

    private static string GetKey(IdSpan identity, Type stateType, string? classPrefix = null) =>
        classPrefix == null
            ? $"{stateType.FullName}-{identity}"
            : $"{stateType.FullName}-{classPrefix}-{identity}";

    private T GetProbe<T>(IdSpan identity, string? grainClassNamePrefix)
        where T : IGrain
    {
        var grain = GetProbe(typeof(T), identity, grainClassNamePrefix);
        return (T)grain;
    }

    private IGrain GetProbe(Type grainType, IdSpan identity, string? grainClassNamePrefix)
    {
        var key = GetKey(identity, grainType, grainClassNamePrefix);
        return _probes.GetOrAdd(key, (key) =>
        {
            // If using strict grain probes, throw the exception
            if (_options.StrictGrainProbes)
            {
                throw new InvalidOperationException($"Probe {identity} does not exist for type {grainType.Name}. " +
                    "Ensure that it is added before the grain is tested.");
            }

            IGrain grain;

            if (_probeFactories.TryGetValue(grainType, out var factory))
            {
                grain = factory(identity);
            }
            else
            {
                var mock = Activator.CreateInstance(typeof(Mock<>).MakeGenericType(grainType)) as IMock<IGrain>;
                grain = mock?.Object!;
            }

            // Save the newly created grain for the next call
            return grain;
        });
    }
}
