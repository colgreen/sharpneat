using SharpNeat.Evaluation;

namespace SharpNeat.EvolutionAlgorithm
{
    /// <summary>
    /// Conveys statistics related to an <see cref="IEvolutionAlgorithm"/>.
    /// </summary>
    public class EvolutionAlgorithmStatistics
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
