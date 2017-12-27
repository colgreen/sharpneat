using System.Collections.Generic;

namespace SharpNeat.Evaluation
{
    public interface IGenomeCollectionEvaluator<TGenome>
    {
        /// <summary>
        /// Evaluates a collection of genomes and assigns fitness info to each.
        /// </summary>
        void Evaluate(ICollection<TGenome> genomeList);  

        /// <summary>
        /// Gets a fitness comparer. 
        /// </summary>
        /// <remarks>
        /// Typically there is a single fitness score whereby a higher score is better, however if there are multiple fitness scores
        /// per genome then we need a more general purpose comparer to determine an ordering on FitnessInfo(s), i.e. to be able to 
        /// determine which is the better FitenssInfo between any two.
        /// </remarks>
        IComparer<FitnessInfo> FitnessComparer { get; }
    }
}
