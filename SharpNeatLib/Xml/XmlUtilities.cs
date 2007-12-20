using System;
using System.Xml;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Xml
{
	public class XmlUtilities
	{
		#region Public Static Methods [General Xml Reader/Writer Support]

		public static void AddAttribute(XmlElement parent, string name, string attrValue)
		{
			XmlDocument doc = parent.OwnerDocument;

			XmlAttribute attr = doc.CreateAttribute(name);
			attr.Value = attrValue;
			parent.Attributes.Append(attr);
		}

		public static XmlElement AddElement(XmlNode parentNode, string name)
		{
			XmlDocument doc;
			if(parentNode is XmlDocument)
				doc = (XmlDocument)parentNode;
			else
				doc = parentNode.OwnerDocument;

			XmlElement elem = doc.CreateElement(name);
			parentNode.AppendChild(elem);

			return elem;
		}

		static public string GetAttributeValue(XmlNode xmlNode, string attributeName)
		{
			return GetAttributeValue(xmlNode, attributeName, true);
		}

		static public string GetAttributeValue(XmlNode xmlNode, string attributeName, bool mandatory)
		{
			XmlAttribute attr = GetAttribute(xmlNode, attributeName, mandatory);

			if(attr==null)
				return "";
			else
				return attr.Value;
		}


		static public XmlAttribute GetAttribute(XmlNode xmlNode, string attributeName)
		{
			return GetAttribute(xmlNode, attributeName, true);
		}

		static public XmlAttribute GetAttribute(XmlNode xmlNode, string attributeName, bool mandatory)
		{
			XmlAttribute attr = (XmlAttribute)xmlNode.Attributes.GetNamedItem(attributeName);

			if(attr==null)
			{
				if(mandatory)
					throw new Exception("Missing mandatory '" + attributeName + "' attribute on '" + xmlNode.LocalName + "' element"); //TODO: tidy up exception.
				else
					return null;
			}
			
			return attr;
		}

		#endregion

		#region Public Static Methods [Type Conversion]

		public static string GetNeuronTypeString(NeuronType type)
		{
			switch(type)
			{
				case NeuronType.Bias:
					return "bias";
				case NeuronType.Hidden:
					return "hid";
				case NeuronType.Input:
					return "in";
				case NeuronType.Output:
					return "out";
				default:
					return string.Empty;
			}
		}

		public static NeuronType GetNeuronType(string typeIdentifier)
		{
			if(typeIdentifier=="bias")
			{
				return NeuronType.Bias;
			}
			else if(typeIdentifier=="hid")
			{
				return NeuronType.Hidden;
			}
			else if(typeIdentifier=="in")
			{
				return NeuronType.Input;
			}
			else if(typeIdentifier=="out")
			{
				return NeuronType.Output;
			}
			else
			{
				throw new XmlException("Unrecognised neuron type identifier - '" + typeIdentifier + "'");
			}
		}

		#endregion

	}
}
