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
	public class IntegerFastConcurrentNetwork : INetwork
	{
		#region Class Variables

//		IActivationFunction activationFn;

		// Neurons are ordered with bias and input nodes at the head of the list, then output nodes and
		// hidden nodes on the array's tail.
		int[] neuronSignalArray;
		int[] _neuronSignalArray;
		IntegerFastConnection[] connectionArray;

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

		public IntegerFastConcurrentNetwork(int biasNeuronCount, 
											int inputNeuronCount,
											int outputNeuronCount,
											int totalNeuronCount,
											IntegerFastConnection[] connectionArray)
		{
			this.biasNeuronCount = biasNeuronCount;
			this.inputNeuronCount = inputNeuronCount;
			this.totalInputNeuronCount = biasNeuronCount + inputNeuronCount;
			this.outputNeuronCount = outputNeuronCount;

			this.connectionArray = connectionArray;
			
			//----- Allocate the arrays that make up the neural network.
			// The neuron signals are initialised to 0 by default. Only bias nodes need setting to 1.
			neuronSignalArray = new int[totalNeuronCount];
			_neuronSignalArray = new int[totalNeuronCount];

			for(int i=0; i<biasNeuronCount; i++)
				neuronSignalArray[i] = 0x1000;
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
				//neuronSignalArray[i] = activationFn.Calculate(_neuronSignalArray[i]);
				
				// Customised integer maths activation function.
				int x = _neuronSignalArray[i];
				int result;

				if( x < -0x800000 )
				{
					result = 0x0;
				}
				else if( x < 0x0 )
				{
					// Scale x down to a max of 2^15 so it won't overflow when we square it.
					// Within this condition part, x has a max of 2^23, 23-15=8, so divide by
					// 2^8.  Then translate the value up into the +ve.
					int tmp = (x>>8) + 0x8000;

					// Square tmp to generate the curve. max result is 2^30. Expected max output 
					// for this half of the curve is 2^11. 30-11=19, so...
					result = ((tmp*tmp)>>19);
				}
				else if( x < 0x800000 )
				{
					// Same thing again except we flip the curve and translate it at the same time 
					// by subtracting the result from 2^12.
					int tmp = (x>>8) - 0x8000;
					result = 0x1000 - ((tmp*tmp)>>19);
				}
				else
				{
					result = 0x1000;
				}

				neuronSignalArray[i] = result;
				
				// Take the opportunity to reset the pre-activation signal array.
				_neuronSignalArray[i]=0;
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
			int intMaxAllowedSignalDelta = (int)(maxAllowedSignalDelta * 0x1000D);

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
					int oldSignal = neuronSignalArray[i];
					//neuronSignalArray[i] = activationFn.Calculate(_neuronSignalArray[i]);

					// Customised integer maths activation function.
					int x = _neuronSignalArray[i];
					int result;

					if( x < -0x800000 )
					{
						result = 0x0;
					}
					else if( x < 0x0 )
					{
						// Scale x down to a max of 2^15 so it won't overflow when we square it.
						// Within this condition part, x has a max of 2^23, 23-15=8, so divide by
						// 2^8.  Then translate the value up into the +ve.
						int tmp = (x>>8) + 0x8000;

						// Square tmp to generate the curve. max result is 2^30. Expected max output 
						// for this half of the curve is 2^11. 30-11=19, so...
						result = ((tmp*tmp)>>19);
					}
					else if( x < 0x800000 )
					{
						// Same thing again except we flip the curve and translate it at the same time 
						// by subtracting the result from 2^12.
						int tmp = (x>>8) - 0x8000;
						result = 0x1000 - ((tmp*tmp)>>19);
					}
					else
					{
						result = 0x1000;
					}
					neuronSignalArray[i] = result;

			
					if(Math.Abs(neuronSignalArray[i]-oldSignal) > intMaxAllowedSignalDelta)
						isRelaxed=false;

					// Take the opportunity to reset the pre-activation signal array.
					_neuronSignalArray[i]=0;
				}
			}

			return isRelaxed;
		}

		public void SetInputSignal(int index, double signalValue)
		{
			// Scale the signal into our expected range for the integer network.
			// +-5 -> +-2^31
			neuronSignalArray[biasNeuronCount + index] = (int)(signalValue * 0x19999999D);

			//neuronSignalArray[biasNeuronCount + index] = (float)signalValue;
		}

		public void SetInputSignals(double[] signalArray)
		{
			// Scale the signal into our expected range for the integer network.
			// +-5 -> +-2^31
			// For speed we don't bother with bounds checks.
			for(int i=0; i<signalArray.Length; i++)
				neuronSignalArray[i+1] = (int)(signalArray[i] * 0x19999999D);
		}

		public double GetOutputSignal(int index)
		{
			// Scale the integer signal back into the flaoting point world.
			// 0 to 0x1000 -> 0 to 1.0
			return (double)neuronSignalArray[totalInputNeuronCount + index] / 0x1000D;
		}

		public void ClearSignals()
		{
			// Clear signals for input, hidden and output nodes. Only the bias node is untouched.
			for(int i=biasNeuronCount; i<neuronSignalArray.Length; i++)
				neuronSignalArray[i]=0;
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
