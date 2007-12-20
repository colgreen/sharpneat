using System;
using SharpNeatLib.NeatGenome;

namespace SharpNeatLib.Evolution
{
	/// <summary>
	/// When mutation creates a new NeuronGene we wish to store the new gene in a list so that we
	/// can amalgamate innovations for a generation. We also need to know the neuron's connections
	/// and so we use this structure to store the new neuron along with it's two connections.
	/// Remember that new neurons are always connected to the network by replacing (splitting)
	/// an existing connection.
	/// </summary>
	public struct NewNeuronGeneStruct
	{
		public NeuronGene NewNeuronGene;

		/// <summary>
		/// The incoming connection.
		/// </summary>
		public ConnectionGene NewConnectionGene_Input;

		/// <summary>
		/// The outgoing connection.
		/// </summary>
		public ConnectionGene NewConnectionGene_Output;


		public NewNeuronGeneStruct(	NeuronGene newNeuronGene,
									ConnectionGene newConnectionGene_Input,
									ConnectionGene newConnectionGene_Output)
		{
				this.NewNeuronGene = newNeuronGene;
				this.NewConnectionGene_Input = newConnectionGene_Input;
				this.NewConnectionGene_Output = newConnectionGene_Output;
		}
	}					
}
