#if NSUBSTITUTE

using NSubstitute;
using NSubstitute.Core;

#else
using Moq;
#endif

namespace Orleans.TestKit.Services;

public sealed class TestServiceProvider : IServiceProvider
{
    private readonly TestKitOptions _options;

    private readonly Dictionary<Type, object> _services;

    public TestServiceProvider(TestKitOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _services = new Dictionary<Type, object>();
    }

    public T AddService<T>(T instance)
    {
        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        _services.Add(typeof(T), instance);
        return instance;
    }

#if NSUBSTITUTE

    public T AddServiceProbe<T>(T mock)
        where T : class
    {
        if (mock == null && typeof(T) is not ICallRouterProvider)
        {
            throw new ArgumentNullException(nameof(mock));
        }

        _services.Add(typeof(T), mock);
        return mock;
    }

    public T AddServiceProbe<T>()
        where T : class
    {
        var mock = Substitute.For<T>();
        _services.Add(typeof(T), mock);
        return mock;
    }

#else

    public Mock<T> AddServiceProbe<T>(Mock<T> mock)
        where T : class
    {
        if (mock == null)
        {
            throw new ArgumentNullException(nameof(mock));
        }

        _services.Add(typeof(T), mock.Object);
        return mock;
    }

    public Mock<T> AddServiceProbe<T>()
        where T : class
    {
        var mock = new Mock<T>();
        _services.Add(typeof(T), mock.Object);
        return mock;
    }
#endif

    public object GetService(Type serviceType)
    {
        if (serviceType == null)
        {
            throw new ArgumentNullException(nameof(serviceType));
        }

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
#if NSUBSTITUTE
            if (Substitute.For(new[] { serviceType }, null) is not ICallRouterProvider mock)
#else
            if (Activator.CreateInstance(typeof(Mock<>).MakeGenericType(serviceType)) is not IMock<object> mock)
#endif

            {
                throw new Exception($"Failed to instantiate {serviceType.Name}.");
            }

            service = mock;

            // Save the newly created grain for the next call
            _services.Add(serviceType, service);

            return service;
        }
    }
}
