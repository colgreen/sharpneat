using SharpNeat.Core;

namespace SharpNeat.EA
{
    public class EAStatistics
    {
        /// <summary>
        /// The current generation number.
        /// </summary>
        uint Generation { get; set; }

        /// <summary>
        /// FitnessInfo associated with the current best genome.
        /// </summary>
        FitnessInfo BestFitness { get; set; }
    }
}
