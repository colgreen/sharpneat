using System;

namespace SharpNeatLib.NeuralNetwork
{
	
	public interface INetwork
	{
		void SingleStep();
		void MultipleSteps(int numberOfSteps);

		/// <summary>
		/// Relax the network. Relaxing refers to activating a network until the amount that signals within 
		/// it are changing within a cetain limit, here defined by maxAllowedSignalDelta. Change is the 
		/// absolute difference between a neuron's output signals between two successive activations.
		/// </summary>
		/// <param name="maxSteps">The number of timesteps to run the network before we give up.</param>
		/// <param name="maxAllowedSignalDelta"></param>
		/// <returns>False if the network did not relax. E.g. due to oscillating signals.</returns>
		bool RelaxNetwork(int maxSteps, double maxAllowedSignalDelta);

		/// <summary>
		/// Assigns a single input signal value.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="signalValue"></param>
		void SetInputSignal(int index, double signalValue);

		/// <summary>
		/// Assigns an array of input signals. IF the array is too long then excess signals are ignored.
		/// If too short then the input neurons with no input keep their pre-existing value.
		/// </summary>
		/// <param name="signalArray"></param>
		void SetInputSignals(double[] signalArray);

		/// <summary>
		/// If index is greater than the number of output neurons then we loop back to the first neuron. 
		/// Therefore we return a value for any given index number >=0.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		double GetOutputSignal(int index);		

		/// <summary>
		/// Reset all inter-neuron signals to zero. This is all neurons except the bias neuron.
		/// Useful when performing successive trials on a network.
		/// </summary>
		void ClearSignals();

		#region Properties

		int InputNeuronCount
		{
			get;
		}

		int OutputNeuronCount
		{
			get;
		}

		#endregion
	}
}
