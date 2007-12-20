using System;
using System.Collections;
using System.Configuration;
using System.Xml;

namespace SharpNeatLib.AppConfig
{
	// An example experiment block: 
	//	<experiment>
	//		<title>Max Experiment</title>
	//		<description>Sample maximizer experiment</description>
	//		<assemblyUrl>MaxExperiment.dll</assemblyUrl>
	//		<typeName>SharpNeatLib.Experiments.MaxExperiment</typeName>
	//		<experimentParameters>
	//			<inputNeuronCount>5</inputNeuronCount>
	//			<outputNeuronCount>1</outputNeuronCount>
	//			<fileName>c:\projects\SharpNEAT\src\Experiments\MaxExperiment\testdata.txt</fileName>
	//		</experimentParameters>
	//	</experiment>

	/// <summary>
	/// An IConfigurationSectionHandler that handle the 'experimentCatalog' block within the
	/// application configuration XML.
	/// 
	/// Reads each experiment is read into an ExperimentConfigInfo object.
	/// </summary>
	public class ExperimentConfigurationSectionHandler : IConfigurationSectionHandler
	{
		#region Public Methods

		/// <summary>
		/// Returns an ArrayList of ExperimentConfigInfo objects. One for each experiment block
		/// within the experimentCatalog section.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="configContext"></param>
		/// <param name="section"></param>
		/// <returns></returns>
		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			// Loop over the experiment nodes within the experimentCatalog section.
			ArrayList oExperimentConfigInfoList = new ArrayList();
			foreach(XmlNode oExperimentNode in  section.ChildNodes)
			{
				ExperimentConfigInfo eci = ReadExperimentConfigInfo(oExperimentNode);
				if(eci!=null) oExperimentConfigInfoList.Add(eci);
			}

			return oExperimentConfigInfoList.ToArray(typeof(ExperimentConfigInfo));
		}

		#endregion

		#region Private Methods

		private ExperimentConfigInfo ReadExperimentConfigInfo(XmlNode experimentNode)
		{
			// Create an ExperimentConfigInfo to read data into.
			ExperimentConfigInfo eci = new ExperimentConfigInfo();

			// Loop over the child nodes of the experiment node.
			foreach(XmlNode oChildNode in experimentNode.ChildNodes)
			{
				// Skip non-element nodes.
				if(oChildNode.NodeType!=XmlNodeType.Element)
					continue;

				switch(oChildNode.LocalName)
				{
					case "title":
						eci.Title = oChildNode.InnerText;
						break;
					case "description":
						eci.Description = oChildNode.InnerText;
						break;
					case "assemblyUrl":
						eci.AssemblyUrl = oChildNode.InnerText;
						break;
					case "typeName":
						eci.TypeName = oChildNode.InnerText;
						break;
					case "experimentParameters":
						eci.ParameterTable = ReadExperimentParameters(oChildNode);
						break;
				}
			}

			// Return whatever data we have read so long as some key pieces of data are present.
			// Otherwise return null.
			if(eci.Title==null || eci.AssemblyUrl==null || eci.TypeName==null)
				return null;

			if(eci.ParameterTable==null)
			{	// Create an empty hashtable for consistency.
				eci.ParameterTable = new Hashtable();
			}

			return eci;
		}

		/// <summary>
		/// Reads key-value pairs from teh parameters node into a Hashtable.
		/// </summary>
		/// <param name="parametersNode"></param>
		/// <returns></returns>
		private Hashtable ReadExperimentParameters(XmlNode parametersNode)
		{
			Hashtable oParamTable = new Hashtable();

			foreach(XmlNode oChildNode in parametersNode)
			{	
				// Skip non-element nodes.
				if(oChildNode.NodeType!=XmlNodeType.Element)
					continue;
				
				oParamTable[oChildNode.LocalName] = oChildNode.InnerText;
			}

			return oParamTable;
		}

		#endregion
	}
}
