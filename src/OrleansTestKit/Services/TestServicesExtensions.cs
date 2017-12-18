using Moq;

namespace Orleans.TestKit.Services
{
    public static class TestServicesExtensions
    {

        public static Mock<T> AddServiceProbe<T>(this TestKitSilo silo) where T : class
            => silo.ServiceProvider.AddServiceProbe<T>();

        public static Mock<T> AddServiceProbe<T>(this TestKitSilo silo, Mock<T> mock) where T : class
            => silo.ServiceProvider.AddServiceProbe(mock);

        public static T AddService<T>(this TestKitSilo silo, T instance) where T : class
            => silo.ServiceProvider.AddService(instance);
    }
}