using System;

namespace SharpNeat.EvolutionAlgorithms.PhasedSearch
{
    /// <summary>
    /// Statistics realting to genome complexity regulation. Also known as phased search.
    /// </summary>
    public class ComplexityRegulationStats
    {
        /// <summary>
        /// The maximum fitness observed in the current phase.
        /// </summary>
        public double _currentPhaseFitnessMax;
        /// <summary>
        /// The generation at which the maximum fitness for the current phase was observed.
        /// We use this to detect fitness stalling during complexification.
        /// </summary>
        public uint _currentPhaseFitnessMaxGeneration;
        /// <summary>
        /// The clock time at which the maximum fitness for the current phase was observed.
        /// We use this to detect fitness stalling during complexification.
        /// </summary>
        public DateTime _currentPhaseFitnessMaxClockTime;


        /// <summary>
        /// The minimum complexity (population mean) observed in the current phase.
        /// </summary>
        public double _currentPhaseComplexityMin;
        /// <summary>
        /// The generation at which the minimum complexity (population mean) for the current phased was observed.
        /// We use this to detect stalling during simplification.
        /// </summary>
        public uint _currentPhaseComplexityMinGeneration;
        /// <summary>
        /// The clock time at which the minimum complexity (population mean) for the current phased was observed.
        /// We use this to detect stalling during simplification.
        /// </summary>
        public DateTime _currentPhaseComplexityMinClockTime;


        /// <summary>
        /// The complexity low point from the most recent simplification phase.
        /// </summary>
        public double _lastComplexityBase;
        /// <summary>
        /// The complexity peak from teh most recent complexification phase.
        /// </summary>
        public double _lastComplexityPeak;
        /// <summary>
        /// The generation of the last phase transition.
        /// </summary>
        public uint _lastTransitionGeneration;
        /// <summary>
        /// The clock time of the last phase transition.
        /// </summary>
        public DateTime _lastTransitionClockTime;
    }
}
