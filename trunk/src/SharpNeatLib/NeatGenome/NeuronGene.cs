using System;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.NeatGenome
{
	public class NeuronGene
	{
		// Although this id is allocated from the global innovation ID pool, neurons do not participate 
		// in compatibility measurements and so it is not used as an innovation ID. It is used as a unique
		// ID to distinguish between neurons.
		uint innovationId;
		NeuronType neuronType;

		#region Constructor

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="copyFrom"></param>
		public NeuronGene(NeuronGene copyFrom)
		{
			this.innovationId = copyFrom.innovationId;
			this.neuronType = copyFrom.neuronType;
		}

		public NeuronGene(uint innovationId, NeuronType neuronType)
		{
			this.innovationId = innovationId;
			this.neuronType = neuronType;
		}

		#endregion

		#region Properties

		public uint InnovationId
		{
			get
			{
				return innovationId;
			}
			set
			{
				innovationId = value;
			}
		}

		public NeuronType NeuronType
		{
			get
			{
				return neuronType;
			}
		}

		#endregion
	}
}
