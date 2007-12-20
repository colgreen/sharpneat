using System;

namespace SharpNeatLib.Evolution
{
	public class Species
	{
		public long SpeciesAge=0;
		public long AgeAtLastImprovement=0;
		public double MaxFitnessEver=0.0;

		public int SpeciesId=-1;
		public GenomeList Members = new GenomeList();
		public double TotalFitness;
		public double MeanFitness;

		/// <summary>
		/// The target size for this species, as determined by the fitness sharing technique.
		/// </summary>
		public int TargetSize;

		/// <summary>
		/// The number of orgainisms that are elite and should not be culled. 
		/// </summary>
		public int ElitistSize;

		/// <summary>
		/// The number of top scoring genomes we can should select from.
		/// </summary>
		public int SelectionCount;

		/// <summary>
		/// The total fitness of all of the genomes that can be selected from.
		/// </summary>
		public double SelectionCountTotalFitness;

		public int TotalNeuronCount;
		public int TotalConnectionCount;

		/// <summary>
		/// TotalNeuronCount + TotalConnectionCount.
		/// </summary>
		public int TotalStructureCount;

		/// <summary>
		/// Indicates that this species is a candidate for species culling. This will normally occur when the
		/// species has not improved for a number of generations.
		/// </summary>
		public bool CullCandidateFlag=false;

		public void ResetFitnessValues()
		{
			TotalFitness = 0;
			MeanFitness = 0;
		}
	}
}
