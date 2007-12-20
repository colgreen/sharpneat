using System;
using System.Xml;
using SharpNeatLib.Evolution;

namespace SharpNeatLib.Evolution.Xml
{
	public interface IGenomeReader
	{
		IGenome Read(XmlElement xmlGenome);
	}
}
