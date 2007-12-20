using System;
using System.Collections;

namespace SharpNeatLib.NeatGenome
{
	/// <summary>
	/// Compares the innovation ID of NeuronGenes.
	/// </summary>
	public class NeuronGeneComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			// Test the most likely cases first.
			if(((NeuronGene)x).InnovationId < ((NeuronGene)y).InnovationId)
				return -1;
			else if (((NeuronGene)x).InnovationId > ((NeuronGene)y).InnovationId)
				return 1;
			else
				return 0;
		}
	}
}
