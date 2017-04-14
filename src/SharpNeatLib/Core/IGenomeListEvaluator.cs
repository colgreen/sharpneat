using System.Collections.Generic;

namespace SharpNeat.Core
{
    public interface IGenomeListEvaluator<TGenome> 
        where TGenome : IGenome
    {
        /// <summary>
        /// Evaluates a list of genomes and assigns fitness info to each.
        /// </summary>
        void Evaluate(IList<TGenome> genomeList);  
    }
}
