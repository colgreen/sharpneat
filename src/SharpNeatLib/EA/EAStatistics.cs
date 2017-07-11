using SharpNeat.Core;

namespace SharpNeat.EA
{
    public class EAStatistics
    {
        /// <summary>
        /// The current generation number.
        /// </summary>
        public uint Generation { get; set; }

        /// <summary>
        /// FitnessInfo associated with the current best genome.
        /// </summary>
        public FitnessInfo BestFitness { get; set; }

        /// <summary>
        /// Indicates whether some goal fitness has been achieved and that the evolutionary algorithm search should stop.
        /// This property's value can remain false to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied { get; set; }
    }
}
