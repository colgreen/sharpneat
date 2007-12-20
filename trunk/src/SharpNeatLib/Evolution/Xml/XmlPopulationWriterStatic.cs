using System;
using System.Xml;

using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Xml;

namespace SharpNeatLib.Evolution.Xml
{
	public class XmlPopulationWriter
	{
		public static void Write(XmlNode parentNode, Population p, IActivationFunction activationFn)
		{
			XmlElement xmlPopulation = XmlUtilities.AddElement(parentNode, "population");
			XmlUtilities.AddAttribute(xmlPopulation, "activation-fn-id", activationFn.FunctionId);

			foreach(IGenome genome in p.GenomeList)
			{
				genome.Write(xmlPopulation);
			}
		}
	}
}
