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
    }
}