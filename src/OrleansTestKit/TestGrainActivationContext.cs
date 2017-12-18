using System;
using System.Collections.Generic;
using Orleans.Core;
using Orleans.GrainDirectory;
using Orleans.Runtime;
using Microsoft.Extensions.Logging;

namespace Orleans.TestKit
{
    public class TestGrainActivationContext : IGrainActivationContext
    {
        public IServiceProvider ActivationServices
        {
            get;
            set;
        }

        public IGrainIdentity GrainIdentity
        {
            get;
            set;
        }

        public Type GrainType
        {
            get;
            set;
        }

        public Grain GrainInstance => throw new NotImplementedException();

        public IDictionary<object, object> Items => throw new NotImplementedException();

        public IGrainLifecycle ObservableLifecycle { get; set; }

        public IMultiClusterRegistrationStrategy RegistrationStrategy => throw new NotImplementedException();
    }
}
