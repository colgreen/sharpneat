using System;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public interface ISimulator
	{
		void Initialise_Random();
		bool PerformSingleStep(INetwork network);

		#region Properties

		/// <summary>
		/// The number of input signals used in the simulator.
		/// </summary>
		int InputNeuronCount
		{
			get;
		}

		/// <summary>
		/// The number of output signals used in the simulator.
		/// </summary>
		int OutputNeuronCount
		{
			get;
		}

		#endregion
	}
}
