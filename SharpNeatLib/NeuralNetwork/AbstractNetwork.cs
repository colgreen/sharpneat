using System;

namespace SharpNeatLib.NeuralNetwork
{
	/// <summary>
	/// A base class for neural networks. This class provides the underlying data structures
	/// for neurons and connections but not a technique for 'executing' the network.
	/// </summary>
	public abstract class AbstractNetwork : INetwork
	{
		// The master list of ALL neurons within the network.
		protected NeuronList masterNeuronList;

		// There follows a number of Lists that hold various neuron subsets. Perhaps not 
		// a particularly efficient way of doing things, but at least clear!

		// All input neurons. *Not* including single bias neuron. Used by SetInputSignal().
		NeuronList inputNeuronList;

		// All output neurons. Used by GetOutputSignal().
		NeuronList outputNeuronList;

		#region Constructor
		
		public AbstractNetwork(NeuronList neuronList)
		{
			inputNeuronList = new NeuronList();
			outputNeuronList = new NeuronList();
			LoadNeuronList(neuronList);
		}

		#endregion

		#region Properties

		public int InputNeuronCount
		{
			get	
			{
				return inputNeuronList.Count;	
			}
		}

		public int OutputNeuronCount
		{
			get	
			{
				return outputNeuronList.Count;	
			}
		}

		public NeuronList MasterNeuronList
		{
			get
			{
				return masterNeuronList;
			}
		}

		#endregion

		#region INetwork [Implemented]

		public void SetInputSignal(int index, double signalValue)
		{
			inputNeuronList[index].OutputValue = signalValue;
		}

		public void SetInputSignals(double[] signalArray)
		{
			// For speed we don't bother with bounds checks.
			for(int i=0; i<signalArray.Length; i++)
				inputNeuronList[i].OutputValue = signalArray[i];
		}
		
		public double GetOutputSignal(int index)
		{
			return outputNeuronList[index].OutputValue;
		}

		public void ClearSignals()
		{
			int loopBound = masterNeuronList.Count;
			for(int j=0; j<loopBound; j++)
			{
				Neuron neuron = masterNeuronList[j];
				if(neuron.NeuronType != NeuronType.Bias)
					neuron.OutputValue=0;
			}
		}

		#endregion

		#region INetwork [Abstract]

		abstract public void SingleStep();
		abstract public void MultipleSteps(int numberOfSteps);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="maxSteps">The number of timesteps to run the network before we give up.</param>
		/// <param name="maxAllowedSignalDelta"></param>
		/// <returns>False if the network did not relax. E.g. due to oscillating signals.</returns>
		abstract public bool RelaxNetwork(int maxSteps, double maxAllowedSignalDelta);

		#endregion

		#region Private Methods

		/// <summary>
		/// Accepts a list of interconnected neurons that describe the network and loads them into this class instance
		/// so that the network can be run. This primarily means placing input and output nodes into their own Lists
		/// for use during the run.
		/// </summary>
		/// <param name="neuronList"></param>
		private void LoadNeuronList(NeuronList neuronList)
		{
			masterNeuronList = neuronList;

			int loopBound = masterNeuronList.Count;
			for(int j=0; j<loopBound; j++)
			{
				Neuron neuron = masterNeuronList[j];

				switch(neuron.NeuronType)
				{
					case NeuronType.Input:
						inputNeuronList.Add(neuron);
						break;

					case NeuronType.Output:
						outputNeuronList.Add(neuron);
						break;
				}
			}
		}

		#endregion
	}
}
