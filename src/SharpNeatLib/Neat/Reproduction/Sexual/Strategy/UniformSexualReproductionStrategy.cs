using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Reproduction.Sexual.Strategy
{
    /// <summary>
    /// Uniform crossover.
    /// 
    /// The genes of the two parent genomes are aligned by innovation ID. The new child genome
    /// takes genes from each of the parents with a given probability (e.g. 50%).
    /// </summary>
    public class UniformCrossoverReproductionStrategy<T> : ISexualReproductionStrategy<T>
        where T : struct
    {
        public NeatGenome<T> CreateGenome(NeatGenome<double> parent1, NeatGenome<double> parent2)
        {
            throw new NotImplementedException();
        }
    }
}
