using System;
using System.Collections.Generic;
using Moq;

namespace Orleans.TestKit
{
    public class TestServiceProvider : IServiceProvider
    {
        private readonly TestKitOptions _options;
        private readonly Dictionary<Type, object> _services;

        public TestServiceProvider(TestKitOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options;
            _services = new Dictionary<Type, object>();
        }

        public object GetService(Type serviceType)
        {
            object service;

            if (_services.TryGetValue(serviceType, out service))
                return service;

            //If using strict service probes, throw the exception
            if (_options.StrictServiceProbes)
                throw new Exception(
                    $"Service probe does not exist for type {serviceType.Name}. Ensure that it is added before the grain is tested.");
            else
            {
                //Create a new mock
                var mock = Activator.CreateInstance(typeof(Mock<>).MakeGenericType(serviceType)) as IMock<object>;
                service = mock.Object;

                //Save the newly created grain for the next call
                _services.Add(serviceType, service);

                return service;
            }
        }

        public Mock<T> AddServiceProbe<T>(Mock<T> mock) where T : class
        {
            _services.Add(typeof(T), mock.Object);

            return mock;
        }

        public Mock<T> AddServiceProbe<T>() where T : class
        {
            var mock = new Mock<T>();

            _services.Add(typeof(T), mock.Object);

            return mock;
        }

        public T AddService<T>(T instance)
        {
            _services.Add(typeof(T), instance);
           
            return instance;
        }
    }
}
