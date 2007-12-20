using System;
using System.Collections;
using System.Drawing;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.NetworkVisualization
{

	/// <summary>
	/// A class that represnets a model of a neural network that can be used primarily for
	/// viewing the network. Can also be used for manipulating the network - moving neurons.
	/// </summary>
	public class NetworkModel
	{
		Size bounds;

		ModelNeuronList masterNeuronList;// = new ModelNeuronList();
		// includes bias neurons.
		ModelNeuronList inputNeuronList = new ModelNeuronList();	
		ModelNeuronList hiddenNeuronList = new ModelNeuronList();
		ModelNeuronList outputNeuronList = new ModelNeuronList();

		#region Constructors

		public NetworkModel(ModelNeuronList masterNeuronList)
		{
			bounds = new Size(0,0);
			
			this.masterNeuronList = masterNeuronList;
			foreach(ModelNeuron modelNeuron in masterNeuronList)
			{
				// Build the various lists.
				switch(modelNeuron.NeuronType)
				{
					case NeuronType.Bias:
					case NeuronType.Input:
						inputNeuronList.Add(modelNeuron);
						break;

					case NeuronType.Hidden:
						hiddenNeuronList.Add(modelNeuron);
						break;

					case NeuronType.Output:
						outputNeuronList.Add(modelNeuron);
						break;
				}
			}
		}

//		public NetworkModel(ConcurrentNetwork network)
//		{
//			bounds = new Size(0,0);
//			Hashtable neuronTable = new Hashtable(network.MasterNeuronList.Count);
//
//			// loop all neurons and build a table keyed on id.
//			foreach(Neuron neuron in network.MasterNeuronList)
//			{
//				ModelNeuron modelNeuron = new ModelNeuron(neuron);
//				neuronTable.Add(modelNeuron.Id, modelNeuron);
//
//				// Build the various lists.
//				switch(modelNeuron.NeuronType)
//				{
//					case NeuronType.Bias:
//					case NeuronType.Input:
//						inputNeuronList.Add(modelNeuron);
//						break;
//
//					case NeuronType.Hidden:
//						hiddenNeuronList.Add(modelNeuron);
//						break;
//
//					case NeuronType.Output:
//						outputNeuronList.Add(modelNeuron);
//						break;
//				}
//				masterNeuronList.Add(modelNeuron);
//			}
//			// Loop through all of the connections (within the neurons)
//			// Now we have a neuron table keyed on id we can attach the connections
//			// to their source and target neurons.
//			foreach(Neuron neuron in network.MasterNeuronList)
//			{
//				foreach(Connection connection in neuron.ConnectionList)
//				{
//					ModelConnection modelConnection = new ModelConnection();
//					modelConnection.Weight = connection.Weight;
//					modelConnection.SourceNeuron = (ModelNeuron)neuronTable[connection.SourceNeuronId];
//					modelConnection.TargetNeuron = (ModelNeuron)neuronTable[connection.TargetNeuronId];
//
//					modelConnection.SourceNeuron.OutConnectionList.Add(modelConnection);
//					modelConnection.TargetNeuron.InConnectionList.Add(modelConnection);
//				}
//			}
//		}

		#endregion

		#region Properties

		public bool HasPositionInfo
		{
			get
			{
				foreach(ModelNeuron modelNeuron in masterNeuronList)
				{
					// If any one of the VNeurons has no position info then
					// return false. The whole network will likely be re-layed out.
					if(!modelNeuron.HasPositionInfo)
						return false;
				}
				//Got this far so must be ok.
				return true;
			}
		}

		public Size Bounds
		{
			get
			{
				return bounds;
			}
			set
			{
				bounds = value;
			}
		}

		public ModelNeuronList MasterNeuronList
		{
			get
			{
				return masterNeuronList;
			}
		}

		public ModelNeuronList InputNeuronList
		{
			get
			{
				return inputNeuronList;
			}
		}

		public ModelNeuronList HiddenNeuronList
		{
			get
			{
				return hiddenNeuronList;
			}
		}

		public ModelNeuronList OutputNeuronList
		{
			get
			{
				return outputNeuronList;
			}
		}

		#endregion
	}
}
