using System;
using Orleans.Core;
using Orleans.Runtime;

namespace Orleans.TestKit {
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
    }
}
