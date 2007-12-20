using System;
using System.Collections;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeatGenome;

namespace SharpNeatLib.Evolution
{
	/// <summary>
	/// Sort by Fitness(Descending). Genomes with like fitness are then sorted by genome age(Ascending).
	/// This means the selection routines are more liley to select the fit AND the youngest fit.
	/// </summary>
	public class PruningModeGenomeComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			NeatGenome.NeatGenome X = (NeatGenome.NeatGenome)x;
			NeatGenome.NeatGenome Y = (NeatGenome.NeatGenome)y;

			double fitnessDelta = Y.Fitness - X.Fitness;
			if(fitnessDelta<0.0D)
				return -1;
			else if(fitnessDelta>0.0D)
				return 1;

			long sizeDelta = (X.NeuronGeneList.Count+X.ConnectionGeneList.Count) - (Y.NeuronGeneList.Count+Y.ConnectionGeneList.Count);

			// Convert result to an int.
			if(sizeDelta <0)
				return -1;
			else if(sizeDelta==0)
				return 0;
			else
				return 1;
		}
	}
}
