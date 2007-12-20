using System;
using System.Xml;

using SharpNeatLib.Evolution;
using SharpNeatLib.Evolution.Xml;

namespace SharpNeatLib.NeatGenome.Xml
{
	public class XmlNeatGenomeReader : IGenomeReader
	{
		public IGenome Read(XmlElement xmlGenome)
		{
			return XmlNeatGenomeReaderStatic.Read(xmlGenome);
		}
	}
}
