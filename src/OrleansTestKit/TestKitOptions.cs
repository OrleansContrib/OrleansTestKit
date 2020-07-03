using System;

namespace Orleans.TestKit
{
    public sealed class TestKitOptions
    {
        /// <summary>
        /// Flag indicating if strict grain probes are enabled.
        /// True = All probes must be added in order to be used.
        /// False = Probes do not need to be defined in order to be used.
        /// </summary>
        public bool StrictGrainProbes { get; set; }

        /// <summary>
        /// Flag indicating if strict stream probes are enabled.
        /// True = All probes must be added in order to be used.
        /// False = Probes do not need to be defined in order to be used.
        /// </summary>
        public bool StrictStreamProbes { get; set; }

        /// <summary>
        /// Flag indicating if strict service probes are enabled.
        /// True = All probes must be added in order to be used.
        /// False = Probes do not need to be defined in order to be used.
        /// </summary>
        public bool StrictServiceProbes { get; set; }

        /// <summary>
        /// Factory that will be used by StorageManager to create an instance of IStorage&lt;TState&gt;.
        /// Leave null to use default TestStorage.
        /// Implement IStorageStats in your storage class to support storage statistics.
        /// </summary>
        public Func<Type, object> StorageFactory { get; set; }
    }
}
