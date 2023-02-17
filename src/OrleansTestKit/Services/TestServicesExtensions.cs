#if NSUBSTITUTE

#else
using Moq;
#endif

namespace Orleans.TestKit;

public static class TestServicesExtensions
{
    public static T AddService<T>(this TestKitSilo silo, T instance)
        where T : class
    {
        if (silo == null)
        {
            throw new ArgumentNullException(nameof(silo));
        }

        return silo.ServiceProvider.AddService(instance);
    }

#if NSUBSTITUTE

    public static T AddServiceProbe<T>(this TestKitSilo silo)
            where T : class
    {
        if (silo == null)
        {
            throw new ArgumentNullException(nameof(silo));
        }

        return silo.ServiceProvider.AddServiceProbe<T>();
    }

    public static T AddServiceProbe<T>(this TestKitSilo silo, T mock)
        where T : class
    {
        if (silo == null)
        {
            throw new ArgumentNullException(nameof(silo));
        }

        return silo.ServiceProvider.AddServiceProbe(mock);
    }

#else
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
#endif
}
