using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Orleans.Core;
using Orleans.GrainDirectory;
using Orleans.Runtime;

namespace Orleans.TestKit
{
    public sealed class TestGrainActivationContext : IGrainActivationContext
    {
        public IServiceProvider ActivationServices { get; set; }

        public IGrainIdentity GrainIdentity { get; set; }

        public Type GrainType { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public Grain GrainInstance => throw new NotImplementedException();

        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public IDictionary<object, object> Items => throw new NotImplementedException();

        public IGrainLifecycle ObservableLifecycle { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public IMultiClusterRegistrationStrategy RegistrationStrategy => throw new NotImplementedException();
    }
}
