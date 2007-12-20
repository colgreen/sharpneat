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
	public class FastConcurrentMultiplicativeNetwork : INetwork
	{
		#region Class Variables

		IActivationFunction activationFn;

		// Neurons are ordered with bias and input nodes at the head of the list, then output nodes and
		// hidden nodes on the array's tail.
		float[] neuronSignalArray;
		float[] _neuronSignalArray;
		BitArray neuronSignalFlagArray;
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

		public FastConcurrentMultiplicativeNetwork(	int biasNeuronCount, 
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
			// The neurons signals are initialised to 0 by default. Only bias nodes need setting to 1.
			neuronSignalArray = new float[totalNeuronCount];
			_neuronSignalArray = new float[totalNeuronCount];
			neuronSignalFlagArray = new BitArray(totalNeuronCount);

			for(int i=0; i<biasNeuronCount; i++)
				neuronSignalArray[i] = 1.0F;
		}

//		public FastConcurrentNetwork(NeatGenome.NeatGenome g, IActivationFunction activationFn)
//		{
//			this.activationFn = activationFn;
//
//		//----- Store/calculate some useful values.
//			outputNeuronCount = g.OutputNeuronCount;
//
//			int neuronGeneCount = g.NeuronGeneList.Count;
//
//			// Slightly inefficient - determine the number of bias nodes. Fortunately there is not actually
//			// any reason to ever have more than one bias node - although there may be 0.
//			int neuronGeneIdx=0;
//			for(; neuronGeneIdx<neuronGeneCount; neuronGeneIdx++)
//			{
//				if(g.NeuronGeneList[neuronGeneIdx].NeuronType != NeuronType.Bias)
//					break;
//			}
//			biasNeuronCount = neuronGeneIdx;
//			inputNeuronCount = g.InputNeuronCount;
//			totalInputNeuronCount = inputNeuronCount+biasNeuronCount;
//
//		//----- Allocate the arrays that make up the neural network.
//			// The neurons signals are initialised to 0 by default. Only bias nodes need setting to 1.
//			neuronSignalArray = new float[neuronGeneCount];
//			_neuronSignalArray = new float[neuronGeneCount];
//
//			for(int i=0; i<biasNeuronCount; i++)
//				neuronSignalArray[i] = 1.0F;
//
//			// ConnectionGenes point to a neuron ID. We need to map this ID to a 0 based index for
//			// efficiency. To do this we build a table of indexes (ints) keyed on neuron ID.
//			// TODO: An alternative here would be to forgo the building of a table and do a binary 
//			// search directly on the NeuronGeneList - probaly a good idea to use a heuristic based upon
//			// neuroncount*connectioncount that decides on which technique to use. Small networks will
//			// likely be faster to decode using the binary search.
//
//			// Actually we can partly achieve the above optimzation by using HybridDictionary instead of Hashtable.
//			// Although creating a table is a bit expensive.
//			HybridDictionary neuronIndexTable = new HybridDictionary(neuronGeneCount);
//			for(int i=0; i<neuronGeneCount; i++)
//				neuronIndexTable.Add(g.NeuronGeneList[i].InnovationId, i);
//
//			// Now we can build the connection array(s).
//			int connectionCount = g.ConnectionGeneList.Count;
//			connectionArray = new FastConnection[connectionCount];
//			for(int connectionIdx=0; connectionIdx<connectionCount; connectionIdx++)
//			{
//				ConnectionGene connectionGene = g.ConnectionGeneList[connectionIdx];
//				
//				connectionArray[connectionIdx].sourceNeuronIdx = (int)neuronIndexTable[connectionGene.SourceNeuronId];
//				connectionArray[connectionIdx].targetNeuronIdx = (int)neuronIndexTable[connectionGene.TargetNeuronId];
//				connectionArray[connectionIdx].weight = (float)connectionGene.Weight;
//			}
//
//			// Now sort the connection array on sourceNeuronIdx, secondary sort on targetNeuronIdx.
//			// TODO: custom sort routine to prevent boxing/unboxing required by Array.Sort(ValueType[])
//			Array.Sort(connectionArray, fastConnectionComparer);
//		}

		#endregion

		#region INetwork Members

		public void SingleStep()
		{
			// Loop connections. Calculate each connection's output signal.
			for(int i=0; i<connectionArray.Length; i++)
				connectionArray[i].signal = neuronSignalArray[connectionArray[i].sourceNeuronIdx] * connectionArray[i].weight;

			for(int i=totalInputNeuronCount; i<_neuronSignalArray.Length; i++)
				_neuronSignalArray[i]=1.0F;

			// Loop the connections again. This time add the signals to the target neurons.
			// This will largely require out of order memory writes. This is the one loop where
			// this will happen.
			for(int i=0; i<connectionArray.Length; i++)
			{
				_neuronSignalArray[connectionArray[i].targetNeuronIdx] *= connectionArray[i].signal;
				// Set flag to indicate this neuron has some inputs (required for multiplicative network).
				neuronSignalFlagArray[connectionArray[i].targetNeuronIdx] = true;
			}

			// Now loop _neuronSignalArray, pass the signals through the activation function 
			// and store the result back to neuronSignalArray. Skip over input neurons - these
			// neurons should be untouched.
			for(int i=totalInputNeuronCount; i<_neuronSignalArray.Length; i++)
			{
				if(neuronSignalFlagArray[i])
					neuronSignalArray[i] = activationFn.Calculate(_neuronSignalArray[i]);
				else
					neuronSignalArray[i] = activationFn.Calculate(0.0F);

				// Take the opportunity to reset the pre-activation signal array.
				// Reset to 1.0 for multiplicative network.
				//_neuronSignalArray[i]=1.0F;
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

				for(int i=totalInputNeuronCount; i<_neuronSignalArray.Length; i++)
					_neuronSignalArray[i]=1.0F;

				// Loop the connections again. This time add the signals to the target neurons.
				// This will largely require out of order memory writes. This is the one loop where
				// this will happen.
				for(int i=0; i<connectionArray.Length; i++)
				{
					_neuronSignalArray[connectionArray[i].targetNeuronIdx] *= connectionArray[i].signal;
					// Set flag to indicate this neuron has some inputs (required for multiplicative network).
					neuronSignalFlagArray[connectionArray[i].targetNeuronIdx] = true;
				}

				// Now loop _neuronSignalArray, pass the signals through the activation function 
				// and store the result back to neuronSignalArray. Skip over input neurons - these
				// neurons should be untouched.
				for(int i=totalInputNeuronCount; i<_neuronSignalArray.Length; i++)
				{
					float oldSignal = neuronSignalArray[i];

					if(neuronSignalFlagArray[i])
						neuronSignalArray[i] = activationFn.Calculate(_neuronSignalArray[i]);
					else
						neuronSignalArray[i] = activationFn.Calculate(0.0F);
			
					if(Math.Abs(neuronSignalArray[i]-oldSignal) > maxAllowedSignalDelta)
						isRelaxed=false;

					// Take the opportunity to reset the pre-activation signal array.
					// Reset to 1.0 for multiplicative network.
					//_neuronSignalArray[i]=1.0F;
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
