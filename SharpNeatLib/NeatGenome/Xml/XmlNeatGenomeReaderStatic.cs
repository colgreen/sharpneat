using System;
using System.Xml;

using SharpNeatLib.Evolution;
using SharpNeatLib.Evolution.Xml;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Xml;

namespace SharpNeatLib.NeatGenome.Xml
{
	public class XmlNeatGenomeReaderStatic
	{
		public static NeatGenome Read(XmlDocument doc)
		{
			XmlElement xmlGenome = (XmlElement)doc.SelectSingleNode("genome");
			if(xmlGenome==null)
				throw new Exception("The genome XML is missing the root 'genome' element.");

			return Read(xmlGenome);
		}

		public static NeatGenome Read(XmlElement xmlGenome)
		{
			int inputNeuronCount=0;
			int outputNeuronCount=0;

			uint id = uint.Parse(XmlUtilities.GetAttributeValue(xmlGenome, "id"));

			//--- Read neuron genes into a list.
			NeuronGeneList neuronGeneList = new NeuronGeneList();
			XmlNodeList listNeuronGenes = xmlGenome.SelectNodes("neurons/neuron");
			foreach(XmlElement xmlNeuronGene in listNeuronGenes)
			{
				NeuronGene neuronGene = ReadNeuronGene(xmlNeuronGene);

				// Count the input and output neurons as we go.
				switch(neuronGene.NeuronType)
				{
					case NeuronType.Input:
						inputNeuronCount++;
						break;
					case NeuronType.Output:
						outputNeuronCount++;
						break;
				}

				neuronGeneList.Add(neuronGene);
			}

			//--- Read connection genes into a list.
			ConnectionGeneList connectionGeneList = new ConnectionGeneList();
			XmlNodeList listConnectionGenes = xmlGenome.SelectNodes("connections/connection");
			foreach(XmlElement xmlConnectionGene in listConnectionGenes)
				connectionGeneList.Add(ReadConnectionGene(xmlConnectionGene));
			
			return new NeatGenome(id, neuronGeneList, connectionGeneList, inputNeuronCount, outputNeuronCount);
		}

		private static NeuronGene ReadNeuronGene(XmlElement xmlNeuronGene)
		{
			uint id = uint.Parse(XmlUtilities.GetAttributeValue(xmlNeuronGene, "id"));
			NeuronType neuronType = XmlUtilities.GetNeuronType(XmlUtilities.GetAttributeValue(xmlNeuronGene, "type"));
			return new NeuronGene(id, neuronType);	
		}

		private static ConnectionGene ReadConnectionGene(XmlElement xmlConnectionGene)
		{
			uint innovationId = uint.Parse(XmlUtilities.GetAttributeValue(xmlConnectionGene, "innov-id"));
			uint sourceNeuronId = uint.Parse(XmlUtilities.GetAttributeValue(xmlConnectionGene, "src-id"));
			uint targetNeuronId = uint.Parse(XmlUtilities.GetAttributeValue(xmlConnectionGene, "tgt-id"));
			double weight = double.Parse(XmlUtilities.GetAttributeValue(xmlConnectionGene, "weight"));
	
			return new ConnectionGene(innovationId, sourceNeuronId, targetNeuronId, weight);
		}
	}
}
