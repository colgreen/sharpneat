using System;
using System.Collections;
using System.Collections.Specialized;

using SharpNeatLib.Genome;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.NeuralNetwork
{

	public class FastRealtimeNetwork : INetwork
	{
		#region Structures

		struct FastConnection
		{
			public int sourceNeuronIdx;
			public int targetNeuronIdx;
			public double weight;
			public double signal;
		}

		#endregion

		#region Classes

		internal class FastConnectionComparer : IComparer
		{
			#region IComparer Members

			public int Compare(object x, object y)
			{
				FastConnection a = (FastConnection)x;
				FastConnection b = (FastConnection)y;


				int diff = a.sourceNeuronIdx - b.sourceNeuronIdx;
				if(diff==0)
				{
					// Secondary sort on targetNeuronIdx.
					return a.targetNeuronIdx - b.targetNeuronIdx;
				}
				else
				{
					return diff;
				}
			}

			#endregion
		}


		#endregion

		static FastConnectionComparer fastConnectionComparer = new FastConnectionComparer();

		#region Class Variables

		IActivationFunction activationFn;

		// Neurons are ordered with bias and input nodes at the head of the list, then output nodes and
		// hidden nodes on the array's tail.
		double[] neuronSignalArray;
		double[] _neuronSignalArray;
		FastConnection[] connectionArray;

		/// <summary>
		/// The number of input neurons. ALso the index 1 after the last input neuron.
		/// </summary>
		int inputNeuronCount;
		int totalInputNeuronCount;
		int outputNeuronCount;

		/// <summary>
		/// This is the index of the first hidden neuron in the array (inputNeuronCount + outputNeuronCount).
		/// </summary>
//		int firstOutputIdx;
		int biasNodeCount;

		#endregion

		#region Constructor

		public FastRealtimeNetwork(Genome.Genome g, IActivationFunction activationFn)
		{
			this.activationFn = activationFn;

		//----- Store/calculate some useful values.
			outputNeuronCount = g.OutputNeuronCount;

			int neuronGeneCount = g.NeuronGeneList.Count;
//			firstOutputIdx = neuronGeneCount - outputNeuronCount;

			// Slightly inefficient - determine the number of bias nodes. Fortunately there is not actually
			// any reason to ever have more than one bias node - although there may be 0.
			int neuronGeneIdx=0;
			for(; neuronGeneIdx<neuronGeneCount; neuronGeneIdx++)
			{
				if(g.NeuronGeneList[neuronGeneIdx].NeuronType != NeuronType.Bias)
					break;
			}
			biasNodeCount = neuronGeneIdx;
			inputNeuronCount = g.InputNeuronCount;
			totalInputNeuronCount = inputNeuronCount+biasNodeCount;

		//----- Allocate the arrays that make up the neural network.
			// The neurons signals are initialised to 0 by default. Only bias nodes need setting to 1.
			neuronSignalArray = new double[neuronGeneCount];
			_neuronSignalArray = new double[neuronGeneCount];

			for(int i=0; i<biasNodeCount; i++)
				neuronSignalArray[i] = 1.0;

			// ConnectionGenes point to a neuron ID. We need to map this ID to a 0 based index for
			// efficiency. To do this we build a table of indexes (ints) keyed on neuron ID.
			// TODO: An alternative here would be to forgo the building of a table and do a binary 
			// search directly on the NeuronGeneList - probaly a good idea to use a heuristic based upon
			// neuroncount*connectioncount that decides on which technique to use. Small networks will
			// likely be faster to decode using the binary search.

			// Actually we can partly achieve the above optimzation by using HybridDictionary instead of Hashtable.
			// Although creating a table is a bit expensive.
			HybridDictionary neuronIndexTable = new HybridDictionary(neuronGeneCount);
			for(int i=0; i<neuronGeneCount; i++)
				neuronIndexTable.Add(g.NeuronGeneList[i].InnovationId, i);

			// Now we can build the connection array(s).
			int connectionCount = g.ConnectionGeneList.Count;
			connectionArray = new FastConnection[connectionCount];
			for(int connectionIdx=0; connectionIdx<connectionCount; connectionIdx++)
			{
				ConnectionGene connectionGene = g.ConnectionGeneList[connectionIdx];
				
				connectionArray[connectionIdx].sourceNeuronIdx = (int)neuronIndexTable[connectionGene.SourceNeuronId];
				connectionArray[connectionIdx].targetNeuronIdx = (int)neuronIndexTable[connectionGene.TargetNeuronId];
				connectionArray[connectionIdx].weight = connectionGene.Weight;
			}

			// Now sort the connection array on sourceNeuronIdx, secondary sort on targetNeuronIdx.
			// TODO: custom sort routine to prevent boxing/unboxing required by Array.Sort(ValueType[])
			Array.Sort(connectionArray, fastConnectionComparer);
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

				// Take the opportunity to reset the pre-activation signal array.
				_neuronSignalArray[i]=0.0;
			}
		}

		public void MultipleSteps(int numberOfSteps)
		{
			for(int i=0; i<numberOfSteps; i++)
				SingleStep();
		}

		public bool RelaxNetwork(int maxSteps, double maxAllowedSignalDelta)
		{
			// TODO:  Add FastRealtimeNetwork.RelaxNetwork implementation
			return false;
		}

		public void SetInputSignal(int index, double signalValue)
		{
			neuronSignalArray[biasNodeCount + index] = signalValue;
		}

		public double GetOutputSignal(int index)
		{
			return neuronSignalArray[totalInputNeuronCount + index];
		}

		public void ClearSignals()
		{
			for(int i=totalInputNeuronCount; i<neuronSignalArray.Length; i++)
				neuronSignalArray[i]=0.0;
		}

		public int InputNeuronsCount
		{
			get
			{
				return inputNeuronCount;
			}
		}

		public int OutputNeuronsCount
		{
			get
			{
				return outputNeuronCount;
			}
		}

		#endregion
	}
}
