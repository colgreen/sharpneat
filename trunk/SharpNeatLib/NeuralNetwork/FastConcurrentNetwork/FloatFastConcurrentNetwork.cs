using System;
using System.Collections;
using System.Collections.Specialized;

using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.NeuralNetwork
{
	/// <summary>
	/// A fast implementation of a network with concurrently activated neurons, that is, each
	/// neuron's output signal is calculated for a given timestep using the output signals
	/// from the previous timestep. This then simulates each neuron activating concurrently.
	/// </summary>
	public class FloatFastConcurrentNetwork : INetwork
	{
		#region Class Variables

		IActivationFunction activationFn;

		// Neurons are ordered with bias and input nodes at the head of the list, then output nodes and
		// hidden nodes on the array's tail.
		float[] neuronSignalArray;
		float[] _neuronSignalArray;
		FloatFastConnection[] connectionArray;

		/// <summary>
		/// The number of input neurons. Also the index 1 after the last input neuron.
		/// </summary>
		int inputNeuronCount;
		int totalInputNeuronCount;
		int outputNeuronCount;

		/// <summary>
		/// This is the index of the first hidden neuron in the array (inputNeuronCount + outputNeuronCount).
		/// </summary>
		int biasNeuronCount;

		#endregion

		#region Constructor

		public FloatFastConcurrentNetwork(	int biasNeuronCount, 
										int inputNeuronCount,
										int outputNeuronCount,
										int totalNeuronCount,
										FloatFastConnection[] connectionArray, 
										IActivationFunction activationFn)
		{
			this.biasNeuronCount = biasNeuronCount;
			this.inputNeuronCount = inputNeuronCount;
			this.totalInputNeuronCount = biasNeuronCount + inputNeuronCount;
			this.outputNeuronCount = outputNeuronCount;

			this.connectionArray = connectionArray;
			this.activationFn = activationFn;
			
			//----- Allocate the arrays that make up the neural network.
			// The neuron signals are initialised to 0 by default. Only bias nodes need setting to 1.
			neuronSignalArray = new float[totalNeuronCount];
			_neuronSignalArray = new float[totalNeuronCount];

			for(int i=0; i<biasNeuronCount; i++)
				neuronSignalArray[i] = 1.0F;
		}

		#endregion

		#region INetwork Members

		public void SingleStep()
		{
			// Loop connections. Calculate each connection's output signal.
			for(int i=0; i<connectionArray.Length; i++)
				connectionArray[i].signal = neuronSignalArray[connectionArray[i].sourceNeuronIdx] * connectionArray[i].weight;

			// Loop the connections again. This time add the signals to the target neurons.
			// This will largely require out of order memory writes. This is the one loop where
			// this will happen.
			for(int i=0; i<connectionArray.Length; i++)
				_neuronSignalArray[connectionArray[i].targetNeuronIdx] += connectionArray[i].signal;

			// Now loop _neuronSignalArray, pass the signals through the activation function 
			// and store the result back to neuronSignalArray. Skip over input neurons - these
			// neurons should be untouched.
			for(int i=totalInputNeuronCount; i<_neuronSignalArray.Length; i++)
			{
				neuronSignalArray[i] = activationFn.Calculate(_neuronSignalArray[i]);
				//neuronSignalArray[i] = 1.0F+(_neuronSignalArray[i]/(0.1F+Math.Abs(_neuronSignalArray[i])));
				
				// Take the opportunity to reset the pre-activation signal array.
				_neuronSignalArray[i]=0.0F;
			}
		}

		public void MultipleSteps(int numberOfSteps)
		{
			for(int i=0; i<numberOfSteps; i++)
				SingleStep();
		}

		/// <summary>
		/// Using RelaxNetwork erodes some of the perofrmance gain of FastConcurrentNetwork because of the slightly 
		/// more complex implemementation of the third loop - whe compared to SingleStep().
		/// </summary>
		/// <param name="maxSteps"></param>
		/// <param name="maxAllowedSignalDelta"></param>
		/// <returns></returns>
		public bool RelaxNetwork(int maxSteps, double maxAllowedSignalDelta)
		{
			bool isRelaxed=false;
			for(int j=0; j<maxSteps && !isRelaxed; j++)
			{	
				isRelaxed=true;	// Assume true.

				// Loop connections. Calculate each connection's output signal.
				for(int i=0; i<connectionArray.Length; i++)
					connectionArray[i].signal = neuronSignalArray[connectionArray[i].sourceNeuronIdx] * connectionArray[i].weight;

				// Loop the connections again. This time add the signals to the target neurons.
				// This will largely require out of order memory writes. This is the one loop where
				// this will happen.
				for(int i=0; i<connectionArray.Length; i++)
					_neuronSignalArray[connectionArray[i].targetNeuronIdx] += connectionArray[i].signal;

				// Now loop _neuronSignalArray, pass the signals through the activation function 
				// and store the result back to neuronSignalArray. Skip over input neurons - these
				// neurons should be untouched.
				for(int i=totalInputNeuronCount; i<_neuronSignalArray.Length; i++)
				{
					float oldSignal = neuronSignalArray[i];
					neuronSignalArray[i] = activationFn.Calculate(_neuronSignalArray[i]);
					//neuronSignalArray[i] = 1.0F+(_neuronSignalArray[i]/(0.1F+Math.Abs(_neuronSignalArray[i])));
			
					if(Math.Abs(neuronSignalArray[i]-oldSignal) > maxAllowedSignalDelta)
						isRelaxed=false;

					// Take the opportunity to reset the pre-activation signal array.
					_neuronSignalArray[i]=0.0F;
				}
			}

			return isRelaxed;
		}

		public void SetInputSignal(int index, double signalValue)
		{
			neuronSignalArray[biasNeuronCount + index] = (float)signalValue;
		}

		public void SetInputSignals(double[] signalArray)
		{
			// For speed we don't bother with bounds checks.
			for(int i=0; i<signalArray.Length; i++)
				neuronSignalArray[i+1] = (float)signalArray[i];
		}

		public double GetOutputSignal(int index)
		{
			return neuronSignalArray[totalInputNeuronCount + index];
		}

		public void ClearSignals()
		{
			// Clear signals for input, hidden and output nodes. Only the bias node is untouched.
			for(int i=biasNeuronCount; i<neuronSignalArray.Length; i++)
				neuronSignalArray[i]=0.0F;
		}

		public int InputNeuronCount
		{
			get
			{
				return inputNeuronCount;
			}
		}

		public int OutputNeuronCount
		{
			get
			{
				return outputNeuronCount;
			}
		}

		#endregion
	}
}
