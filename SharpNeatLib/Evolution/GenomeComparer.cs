using System;
using System.Collections;
using SharpNeatLib.Evolution;

namespace SharpNeatLib.Evolution
{
	/// <summary>
	/// Sort by Fitness(Descending). Genomes with like fitness are then sorted by genome size(Ascending).
	/// This means the selection routines are more liley to select the fit AND the smallest first.
	/// </summary>
	public class GenomeComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			IGenome X = (IGenome)x;
			IGenome Y = (IGenome)y;

			double fitnessDelta = Y.Fitness - X.Fitness;
			if(fitnessDelta<0.0D)
				return -1;
			else if(fitnessDelta>0.0D)
				return 1;

			long ageDelta = X.GenomeAge - Y.GenomeAge;

			// Convert result to an int.
			if(ageDelta <0)
				return -1;
			else if(ageDelta>0)
				return 1;
			
			return 0;
		}
	}
}
