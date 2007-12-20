using System;
using System.Xml;

using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Xml;

namespace SharpNeatLib.NeuralNetwork.Xml
{
	public class XmlNetworkWriterStatic
	{
		public static void Write(XmlNode parentNode, NeatGenome.NeatGenome genome, IActivationFunction activationFn)
		{
		//----- Start writing. Create document root node.
			XmlElement xmlNetwork = XmlUtilities.AddElement(parentNode, "network");
			XmlUtilities.AddAttribute(xmlNetwork, "activation-fn-id", activationFn.FunctionId);

		//----- Write neurons.
			XmlElement xmlNeurons = XmlUtilities.AddElement(xmlNetwork, "neurons");
			foreach(NeuronGene neuronGene in genome.NeuronGeneList)
				WriteNeuron(xmlNeurons, neuronGene);

		//----- Write Connections.
			XmlElement xmlConnections = XmlUtilities.AddElement(xmlNetwork, "connections");
			foreach(ConnectionGene connectionGene in genome.ConnectionGeneList)
				WriteConnection(xmlConnections, connectionGene);
		}

		#region Private Methods

		private static void WriteNeuron(XmlElement xmlNeurons, NeuronGene neuronGene)
		{
			XmlElement xmlNeuron = XmlUtilities.AddElement(xmlNeurons, "neuron");

			XmlUtilities.AddAttribute(xmlNeuron, "id", neuronGene.InnovationId.ToString());
			XmlUtilities.AddAttribute(xmlNeuron, "type", XmlUtilities.GetNeuronTypeString(neuronGene.NeuronType));
		}

		private static void WriteConnection(XmlElement xmlConnections, ConnectionGene connectionGene)
		{
			XmlElement xmlConnection = XmlUtilities.AddElement(xmlConnections, "connection");

			XmlUtilities.AddAttribute(xmlConnection, "src-id", connectionGene.SourceNeuronId.ToString() );
			XmlUtilities.AddAttribute(xmlConnection, "tgt-id", connectionGene.TargetNeuronId.ToString());
			XmlUtilities.AddAttribute(xmlConnection, "weight", connectionGene.Weight.ToString());
		}

		#endregion
	}
}
