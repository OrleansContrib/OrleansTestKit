using Microsoft.Extensions.DependencyInjection;
using Moq;
using Orleans.Streams;
using Orleans.TestKit.Streams;

namespace Orleans.TestKit.Services;

/// <summary>The test service provider</summary>
public sealed class TestServiceProvider : IServiceProvider, IKeyedServiceProvider
{
    private readonly Dictionary<(object?, Type), object> _keyedServices = new();

    private readonly TestKitOptions _options;

    private readonly Dictionary<Type, object> _services = new();

    /// <summary>Initializes a new instance of the <see cref="TestServiceProvider"/> class.</summary>
    /// <param name="options">The test kit options to use</param>
    public TestServiceProvider(TestKitOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options;
    }

    /// <summary>Adds or updates a keyed service to the provider</summary>
    /// <typeparam name="T">The service type</typeparam>
    /// <param name="serviceKey">The service key</param>
    /// <param name="instance">The instance to add</param>
    /// <returns>The instance</returns>
    /// <exception cref="ArgumentNullException">Instance must be not null</exception>
    public T AddKeyedService<T>(object? serviceKey, T instance)
    {
        ArgumentNullException.ThrowIfNull(instance);

        _keyedServices[(serviceKey, typeof(T))] = instance;
        return instance;
    }

    /// <summary>Adds or updates a keyed mock to the service provider</summary>
    /// <typeparam name="T">The underlying service type</typeparam>
    /// <param name="serviceKey">The service key</param>
    /// <returns>The newly created mock</returns>
    public Mock<T> AddKeyedServiceProbe<T>(object? serviceKey)
        where T : class =>
        AddKeyedServiceProbe(serviceKey, new Mock<T>());

    /// <summary>Adds or updates a keyed mock to the service provider</summary>
    /// <typeparam name="T">The underlying service type</typeparam>
    /// <param name="serviceKey">The service key</param>
    /// <param name="mock">The mock to add</param>
    /// <returns>The mock</returns>
    public Mock<T> AddKeyedServiceProbe<T>(object? serviceKey, Mock<T> mock)
        where T : class
    {
        AddKeyedService(serviceKey, mock.Object);
        return mock;
    }

    /// <summary>Adds or updates a service to the provider</summary>
    /// <typeparam name="T">The service type</typeparam>
    /// <param name="instance">The instance to add</param>
    /// <returns>The instance</returns>
    /// <exception cref="ArgumentNullException">Instance must be not null</exception>
    public T AddService<T>(T instance)
    {
        ArgumentNullException.ThrowIfNull(instance);

        _services[typeof(T)] = instance;
        return instance;
    }

    /// <summary>Adds or updates a mock to the service provider</summary>
    /// <typeparam name="T">The underlying service type</typeparam>
    /// <param name="mock">The mock to add</param>
    /// <returns>The mock</returns>
    public Mock<T> AddServiceProbe<T>(Mock<T> mock)
        where T : class
    {
        AddService(mock.Object);
        return mock;
    }

    /// <summary>Adds a mock to the service provider</summary>
    /// <typeparam name="T">The underlying service type</typeparam>
    /// <returns>The newly created mock</returns>
    public Mock<T> AddServiceProbe<T>()
        where T : class =>
        AddServiceProbe(new Mock<T>());

    public object? GetKeyedService(Type serviceType, object? serviceKey)
    {
        ArgumentNullException.ThrowIfNull(serviceType);

        if (_keyedServices.TryGetValue((serviceKey, serviceType), out var service))
        {
            return service;
        }

        // If using strict service probes, throw the exception
        if (_options.StrictServiceProbes)
        {
            throw new Exception($"Service probe does not exist for type {serviceType.Name} and key {serviceKey}. Ensure that it is added before the grain is tested.");
        }
        else
        {
            if (serviceType == typeof(IStreamProvider))
            {
                service = new TestStreamProvider(_options);
            }
            else
            {
                // Create a new mock
                if (Activator.CreateInstance(typeof(Mock<>).MakeGenericType(serviceType)) is not IMock<object> mock)
                {
                    throw new Exception($"Failed to instantiate {serviceType.Name}.");
                }

                service = mock.Object;
            }

            // Save the newly created grain for the next call
            _keyedServices.Add((serviceKey, serviceType), service);

            return service;
        }
    }

    public object GetRequiredKeyedService(Type serviceType, object? serviceKey)
    {
        var service = GetKeyedService(serviceType, serviceKey);

        if (service is not object)
        {
            throw new Exception($"Service probe does not exist for type {serviceType.Name} and key {serviceKey}. Ensure that it is added before the grain is tested.");
        }

        return service;
    }

    /// <inheritdoc/>
    public object GetService(Type serviceType)
    {
        ArgumentNullException.ThrowIfNull(serviceType);

        if (_services.TryGetValue(serviceType, out var service))
        {
            return service;
        }

        // If using strict service probes, throw the exception
        if (_options.StrictServiceProbes)
        {
            throw new Exception($"Service probe does not exist for type {serviceType.Name}. Ensure that it is added before the grain is tested.");
        }
        else
        {
            // Create a new mock
            if (Activator.CreateInstance(typeof(Mock<>).MakeGenericType(serviceType)) is not IMock<object> mock)
            {
                throw new Exception($"Failed to instantiate {serviceType.Name}.");
            }

            service = mock.Object;

            // Save the newly created grain for the next call
            _services.Add(serviceType, service);

            return service;
        }
    }
}
