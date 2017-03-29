using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Core
{
    public interface IGenomeListEvaluator<TGenome>
    {
        /// <summary>
        /// Evaluates a list of genomes and returns a list of fitness scores.
        /// Each genome in genomeList has its fitness score returned in the corresponding index/position in the returned list.
        /// The returned list may be re-used to avoid unnecessary memory allocations, therefore classes implementing this interface
        /// should be assumed to be not reentrant.
        /// </summary>
        IList<FitnessInfo> Evaluate(IList<TGenome> genomeList);  
    }
}
