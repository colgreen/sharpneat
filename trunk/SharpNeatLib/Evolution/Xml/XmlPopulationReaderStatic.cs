using System;
using System.Xml;

using SharpNeatLib.Xml;

namespace SharpNeatLib.Evolution.Xml
{
	public class XmlPopulationReader
	{
		public static Population Read(XmlDocument doc, IGenomeReader genomeReader, IIdGeneratorFactory idGeneratorFactory)
		{
			XmlElement xmlPopulation = (XmlElement)doc.SelectSingleNode("population");
			return Read(xmlPopulation, genomeReader, idGeneratorFactory);
		}

		public static Population Read(XmlElement xmlPopulation, IGenomeReader genomeReader, IIdGeneratorFactory idGeneratorFactory)
		{
			GenomeList genomeList = new GenomeList();
			XmlNodeList listGenomes = xmlPopulation.SelectNodes("genome");
			foreach(XmlElement xmlGenome in listGenomes)
				genomeList.Add(genomeReader.Read(xmlGenome));

			IdGenerator idGenerator = idGeneratorFactory.CreateIdGenerator(genomeList);
			return new Population(idGenerator, genomeList);
		}
	}
}
