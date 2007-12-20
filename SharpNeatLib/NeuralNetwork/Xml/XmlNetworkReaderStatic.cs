using System;
using System.Collections;
using System.Xml;

using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Xml;


namespace SharpNeatLib.NeuralNetwork.Xml
{
	public class XmlNetworkReaderStatic
	{
		public static ConcurrentNetwork Read(XmlDocument doc)
		{
			XmlElement network = (XmlElement)doc.SelectSingleNode("network");
			if(network==null)
				throw new Exception("The network XML is missing the root 'network' element.");

			return Read(network);
		}

		public static ConcurrentNetwork Read(XmlElement xmlNetwork)
		{
			return ReadNetwork(xmlNetwork);
		}

		private static ConcurrentNetwork ReadNetwork(XmlElement xmlNetwork)
		{
			//--- Read the activation function id.
			string activationFnId = XmlUtilities.GetAttributeValue(xmlNetwork, "activation-fn-id");
			IActivationFunction activationFn = ActivationFunctionFactory.GetActivationFunction(activationFnId);

			// Read the neurons into a list and also into a table keyed on id.
			Hashtable neuronTable = new Hashtable();

			NeuronList biasNeuronList = new NeuronList();
			NeuronList inputNeuronList = new NeuronList();
			NeuronList hiddenNeuronList = new NeuronList();
			NeuronList outputNeuronList = new NeuronList();
			NeuronList masterNeuronList = new NeuronList();

			XmlNodeList listNeurons = xmlNetwork.SelectNodes("neurons/neuron");
			foreach(XmlElement xmlNeuron in listNeurons)
			{
				Neuron neuron = ReadNeuron(xmlNeuron, activationFn);
				neuronTable.Add(neuron.Id, neuron);

				switch(neuron.NeuronType)
				{
					case NeuronType.Bias:
						biasNeuronList.Add(neuron);
						break;
					case NeuronType.Input:
						inputNeuronList.Add(neuron);
						break;
					case NeuronType.Hidden:
						hiddenNeuronList.Add(neuron);
						break;
					case NeuronType.Output:
						outputNeuronList.Add(neuron);
						break;
				}
			}

			//----- Build a master list of neurons. Neurons must be ordered by type - bias,input,hidden,output.
			if(biasNeuronList.Count != 1)
				throw new SharpNeatLib.Xml.XmlException("Neural Network XML must contain exactly 1 bias node.");

			foreach(Neuron neuron in biasNeuronList)
				masterNeuronList.Add(neuron);

			foreach(Neuron neuron in inputNeuronList)
				masterNeuronList.Add(neuron);

			foreach(Neuron neuron in hiddenNeuronList)
				masterNeuronList.Add(neuron);

			foreach(Neuron neuron in outputNeuronList)
				masterNeuronList.Add(neuron);

			//----- Read Connections and store against target neurons.
			XmlNodeList listConnections = xmlNetwork.SelectNodes("connections/connection");
			foreach(XmlElement xmlConnection in listConnections)
			{
				Connection connection = ReadConnection(xmlConnection);

				// Store the connection with it's target neuron.
				((Neuron)neuronTable[connection.TargetNeuronId]).ConnectionList.Add(connection);
				
				// Bind the connection to it's source neuron.
				connection.SetSourceNeuron((Neuron)neuronTable[connection.SourceNeuronId]);
			}

			return new ConcurrentNetwork(masterNeuronList);
		}

		private static Neuron ReadNeuron(XmlElement xmlNeuron, IActivationFunction activationFn)
		{
			uint id = uint.Parse(XmlUtilities.GetAttributeValue(xmlNeuron, "id"));
			NeuronType neuronType = XmlUtilities.GetNeuronType(XmlUtilities.GetAttributeValue(xmlNeuron, "type"));
			return new Neuron(activationFn, neuronType, id);	
		}

		private static Connection ReadConnection(XmlElement xmlConnection)
		{
			uint sourceNeuronId = uint.Parse(XmlUtilities.GetAttributeValue(xmlConnection, "src-id"));
			uint targetNeuronId = uint.Parse(XmlUtilities.GetAttributeValue(xmlConnection, "tgt-id"));
			double weight = double.Parse(XmlUtilities.GetAttributeValue(xmlConnection, "weight"));
	
			return new Connection(sourceNeuronId, targetNeuronId, weight);
		}
	}
}
