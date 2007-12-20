using System;
using System.Xml;

using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Xml;

namespace SharpNeatLib.NeatGenome.Xml
{
	public class XmlGenomeWriterStatic
	{
		public static void Write(XmlNode parentNode, NeatGenome genome)
		{
			//----- Start writing. Create document root node.
			XmlElement xmlGenome = XmlUtilities.AddElement(parentNode, "genome");
			XmlUtilities.AddAttribute(xmlGenome, "id", genome.GenomeId.ToString());
			XmlUtilities.AddAttribute(xmlGenome, "species-id", genome.SpeciesId.ToString());
			XmlUtilities.AddAttribute(xmlGenome, "age", genome.GenomeAge.ToString());
			XmlUtilities.AddAttribute(xmlGenome, "fitness", genome.Fitness.ToString("0.00"));

			//----- Write neurons.
			XmlElement xmlNeurons = XmlUtilities.AddElement(xmlGenome, "neurons");
			foreach(NeuronGene neuronGene in genome.NeuronGeneList)
				WriteNeuron(xmlNeurons, neuronGene);

			//----- Write Connections.
			XmlElement xmlConnections = XmlUtilities.AddElement(xmlGenome, "connections");
			foreach(ConnectionGene connectionGene in genome.ConnectionGeneList)
				WriteConnectionGene(xmlConnections, connectionGene);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="genome"></param>
		/// <param name="activationFn">Not strictly part of a genome. But it is useful to document which function
		/// the genome is supposed to run against when decoded into a network.</param>
		public static void Write(XmlNode parentNode, NeatGenome genome, IActivationFunction activationFn)
		{
			//----- Start writing. Create document root node.
			XmlElement xmlGenome = XmlUtilities.AddElement(parentNode, "genome");
			XmlUtilities.AddAttribute(xmlGenome, "id", genome.GenomeId.ToString());
			XmlUtilities.AddAttribute(xmlGenome, "species-id", genome.SpeciesId.ToString());
			XmlUtilities.AddAttribute(xmlGenome, "age", genome.GenomeAge.ToString());
			XmlUtilities.AddAttribute(xmlGenome, "fitness", genome.Fitness.ToString("0.00"));
			XmlUtilities.AddAttribute(xmlGenome, "activation-fn-id", activationFn.FunctionId);

			//----- Write neurons.
			XmlElement xmlNeurons = XmlUtilities.AddElement(xmlGenome, "neurons");
			foreach(NeuronGene neuronGene in genome.NeuronGeneList)
				WriteNeuron(xmlNeurons, neuronGene);

			//----- Write Connections.
			XmlElement xmlConnections = XmlUtilities.AddElement(xmlGenome, "connections");
			foreach(ConnectionGene connectionGene in genome.ConnectionGeneList)
				WriteConnectionGene(xmlConnections, connectionGene);
		}

		#region Private Static Methods

		private static void WriteNeuron(XmlElement xmlNeurons, NeuronGene neuronGene)
		{
			XmlElement xmlNeuron = XmlUtilities.AddElement(xmlNeurons, "neuron");

			XmlUtilities.AddAttribute(xmlNeuron, "id", neuronGene.InnovationId.ToString());
			XmlUtilities.AddAttribute(xmlNeuron, "type", XmlUtilities.GetNeuronTypeString(neuronGene.NeuronType));
		}

		private static void WriteConnectionGene(XmlElement xmlConnections, ConnectionGene connectionGene)
		{
			XmlElement xmlConnectionGene = XmlUtilities.AddElement(xmlConnections, "connection");

			XmlUtilities.AddAttribute(xmlConnectionGene, "innov-id", connectionGene.InnovationId.ToString());
			XmlUtilities.AddAttribute(xmlConnectionGene, "src-id", connectionGene.SourceNeuronId.ToString());
			XmlUtilities.AddAttribute(xmlConnectionGene, "tgt-id", connectionGene.TargetNeuronId.ToString());
			XmlUtilities.AddAttribute(xmlConnectionGene, "weight", connectionGene.Weight.ToString("R"));
		}

		#endregion
	}
}
