using System;

namespace SharpNeatLib.NeuralNetwork
{

	public class Connection
	{
		uint sourceNeuronId; // These are redundant in normal operation (we have a reference to the neurons)
		uint targetNeuronId;	// but is useful when creating/loading a network.

		Neuron sourceNeuron;								
		double weight;

		#region Constructor

		public Connection(uint sourceNeuronId, uint targetNeuronId, double weight)
		{
			this.sourceNeuronId = sourceNeuronId;
			this.targetNeuronId = targetNeuronId;
			this.weight = weight;
		}

		#endregion

		#region Public methods

		public void SetSourceNeuron(Neuron neuron)
		{
			sourceNeuron = neuron;
		}

		#endregion

		#region Properties

		public uint SourceNeuronId
		{
			get	
			{
				return sourceNeuronId;	
			}
			set
			{
				sourceNeuronId = value;	
			}
		}

		public uint TargetNeuronId
		{
			get	
			{
				return targetNeuronId;	
			}
			set
			{
				targetNeuronId = value;	
			}
		}

		public double Weight
		{
			get
			{
				return weight;	
			}
		}

		public Neuron SourceNeuron
		{
			get
			{
				return sourceNeuron;	
			}
		}

		#endregion
	}
}
