using System;

namespace SharpNeatLib.GenomeEvaluators
{
	public class GenomeEvaluatorUtilities
	{
		/// <summary>
		/// Genomes cannot have zero fitness because the fitness sharing logic requires there to be 
		/// a non-zero total fitness in the population. Therefore this figure is substituted in where
		/// zero fitness occurs.
		/// </summary>
		public const double MIN_GENOME_FITNESS = 0.00001;
	}
}
