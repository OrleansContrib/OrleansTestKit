using Moq;

namespace Orleans.TestKit;

public static class TestServicesExtensions
{
    public static T AddKeyedService<T>(this TestKitSilo silo, object? serviceKey, T instance)
        where T : class
    {
        if (silo == null)
        {
            throw new ArgumentNullException(nameof(silo));
        }

        return silo.ServiceProvider.AddKeyedService(serviceKey, instance);
    }

    public static Mock<T> AddKeyedServiceProbe<T>(this TestKitSilo silo, object? serviceKey)
        where T : class
    {
        if (silo == null)
        {
            throw new ArgumentNullException(nameof(silo));
        }

        return silo.ServiceProvider.AddKeyedServiceProbe<T>(serviceKey);
    }

    public static Mock<T> AddKeyedServiceProbe<T>(this TestKitSilo silo, object? serviceKey, Mock<T> mock)
        where T : class
    {
        if (silo == null)
        {
            throw new ArgumentNullException(nameof(silo));
        }

        return silo.ServiceProvider.AddKeyedServiceProbe(serviceKey, mock);
    }

    public static T AddService<T>(this TestKitSilo silo, T instance)
            where T : class
    {
        if (silo == null)
        {
            throw new ArgumentNullException(nameof(silo));
        }

        return silo.ServiceProvider.AddService(instance);
    }

    public static Mock<T> AddServiceProbe<T>(this TestKitSilo silo)
        where T : class
    {
        if (silo == null)
        {
            throw new ArgumentNullException(nameof(silo));
        }

        return silo.ServiceProvider.AddServiceProbe<T>();
    }

    public static Mock<T> AddServiceProbe<T>(this TestKitSilo silo, Mock<T> mock)
        where T : class
    {
        if (silo == null)
        {
            throw new ArgumentNullException(nameof(silo));
        }

        return silo.ServiceProvider.AddServiceProbe(mock);
    }
}
