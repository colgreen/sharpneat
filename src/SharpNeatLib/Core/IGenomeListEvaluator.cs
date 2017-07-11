using System.Collections.Generic;

namespace SharpNeat.Core
{
    public interface IGenomeListEvaluator<TGenome>
    {
        /// <summary>
        /// Evaluates a list of genomes and assigns fitness info to each.
        /// </summary>
        void Evaluate(IList<TGenome> genomeList);  

        /// <summary>
        /// Indicates whether some goal fitness has been achieved and that the evolutionary algorithm search should stop.
        /// This property's value can remain false to allow the algorithm to run indefinitely.
        /// </summary>
        bool StopConditionSatisfied { get; }
    }
}
