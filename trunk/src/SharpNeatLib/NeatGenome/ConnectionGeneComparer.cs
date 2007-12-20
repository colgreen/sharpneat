using System;
using System.Collections;

namespace SharpNeatLib.NeatGenome
{
	/// <summary>
	/// Compares the innovation ID of ConnectionGenes.
	/// </summary>
	public class ConnectionGeneComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			// Test the most likely cases first.
			if(((ConnectionGene)x).InnovationId < ((ConnectionGene)y).InnovationId)
				return -1;
			else if (((ConnectionGene)x).InnovationId > ((ConnectionGene)y).InnovationId)
				return 1;
			else
				return 0;
		}
	}
}
