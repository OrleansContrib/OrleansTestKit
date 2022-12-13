using System;
using Moq;
using Orleans;
using Orleans.Runtime;

namespace Orleans.TestKit
{
    public static class GrainProbeExtensions
    {
        public static Mock<T> AddProbe<T>(this TestKitSilo silo, long id, string classPrefix = null)
            where T : class, IGrainWithIntegerKey
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            return silo.GrainFactory.AddProbe<T>(GrainIdKeyExtensions.CreateIntegerKey(id), classPrefix);
        }

        public static Mock<T> AddProbe<T>(this TestKitSilo silo, Guid id, string classPrefix = null)
            where T : class, IGrainWithGuidKey
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            return silo.GrainFactory.AddProbe<T>(GrainIdKeyExtensions.CreateGuidKey(id), classPrefix);
        }

        public static Mock<T> AddProbe<T>(this TestKitSilo silo, long id, string keyExtension, string classPrefix = null)
            where T : class, IGrainWithIntegerCompoundKey
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            return silo.GrainFactory.AddProbe<T>(GrainIdKeyExtensions.CreateIntegerKey(id, keyExtension), classPrefix);
        }

        public static Mock<T> AddProbe<T>(this TestKitSilo silo, Guid id, string keyExtension, string classPrefix = null)
            where T : class, IGrainWithGuidCompoundKey
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            return silo.GrainFactory.AddProbe<T>(GrainIdKeyExtensions.CreateGuidKey(id, keyExtension), classPrefix);
        }

        public static Mock<T> AddProbe<T>(this TestKitSilo silo, string id, string classPrefix = null)
            where T : class, IGrainWithStringKey
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            return silo.GrainFactory.AddProbe<T>(IdSpan.Create(id), classPrefix);
        }

        public static void AddProbe<T>(this TestKitSilo silo, Func<IdSpan, IMock<T>> factory)
            where T : class, IGrain
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            silo.GrainFactory.AddProbe(factory);
        }

        public static void AddProbe<T>(this TestKitSilo silo, Func<IdSpan, T> factory)
            where T : class, IGrain
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            silo.GrainFactory.AddProbe(factory);
        }
    }
}
