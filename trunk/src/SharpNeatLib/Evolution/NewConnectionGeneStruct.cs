using System;
using SharpNeatLib.NeatGenome;

namespace SharpNeatLib.Evolution
{
	/// <summary>
	/// When mutation creates a new ConnectionGene we wish to store the new gene in a list so that we
	/// can amalgamate innovations for a generation. 
	/// </summary>
	public struct NewConnectionGeneStruct
	{
		public NeatGenome.NeatGenome OwningGenome;
		public ConnectionGene NewConnectionGene;

		public NewConnectionGeneStruct(NeatGenome.NeatGenome owningGenome, ConnectionGene newConnectionGene)
		{
			this.OwningGenome = owningGenome;
			this.NewConnectionGene = newConnectionGene;
		}
	}
}
