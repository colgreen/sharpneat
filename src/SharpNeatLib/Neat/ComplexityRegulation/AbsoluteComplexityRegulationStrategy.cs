using System;
using System.Collections.Generic;
using System.Text;
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Neat.ComplexityRegulation
{
    public class AbsoluteCeilingComplexityRegulationStrategy : IComplexityRegulationStrategy
    {
        /// <summary>
        /// Determine the complexity regulation mode that the evolution algorithm search should be in given the 
        /// provided evolution algorithm statistics object.
        /// </summary>
        /// <param name="eaStats">An object that conveys a set of statistics related to the current state of the
        /// evolution algorithm.</param>
        public ComplexityRegulationMode DetermineMode(EvolutionAlgorithmStatistics eaStats)
        {
            // TODO: Implement!
            throw new NotImplementedException();
        }
    }
}
