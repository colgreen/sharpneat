using System;
using System.Xml;

using SharpNeatLib.Evolution;
using SharpNeatLib.Xml;

namespace NeatParameterOptimizer.Xml
{
	public class NeatParametersWrapperWriter
	{
		public static void Write(XmlNode parentNode, NeatParametersWrapperList npwList)
		{
			XmlElement xmlNeatParametersList = XmlUtilities.AddElement(parentNode, "NeatParametersList");

			foreach(NeatParametersWrapper npw in npwList)
				Write(xmlNeatParametersList, npw);
		}

		public static void Write(XmlNode parentNode, NeatParametersWrapper npw)
		{
			NeatParameters np = npw.NeatParameters;

			//-----
			XmlElement xmlNeatParameters = XmlUtilities.AddElement(parentNode, "NeatParameters");

			XmlElement xmlFitness = XmlUtilities.AddElement(xmlNeatParameters, "Fitness");
			xmlFitness.InnerText = npw.Fitness.ToString();

			XmlElement xmlPopulationSize = XmlUtilities.AddElement(xmlNeatParameters, "populationSize");
			xmlPopulationSize.InnerText = np.populationSize.ToString();

			XmlElement xmlPInitialPopulationInterconnections = XmlUtilities.AddElement(xmlNeatParameters, "pInitialPopulationInterconnections");
			xmlPInitialPopulationInterconnections.InnerText = np.pInitialPopulationInterconnections.ToString();

			XmlElement xmlPOffspringAsexual = XmlUtilities.AddElement(xmlNeatParameters, "pOffspringAsexual");
			xmlPOffspringAsexual.InnerText = np.pOffspringAsexual.ToString();

			XmlElement xmlPOffspringSexual = XmlUtilities.AddElement(xmlNeatParameters, "pOffspringSexual");
			xmlPOffspringSexual.InnerText = np.pOffspringSexual.ToString();

			XmlElement xmlPInterspeciesMating = XmlUtilities.AddElement(xmlNeatParameters, "pInterspeciesMating");
			xmlPInterspeciesMating.InnerText = np.pInterspeciesMating.ToString();

			XmlElement xmlPDisjointExcessGenesRecombined = XmlUtilities.AddElement(xmlNeatParameters, "pDisjointExcessGenesRecombined");
			xmlPDisjointExcessGenesRecombined.InnerText = np.pDisjointExcessGenesRecombined.ToString();

			XmlElement xmlPMutateConnectionWeights = XmlUtilities.AddElement(xmlNeatParameters, "pMutateConnectionWeights");
			xmlPMutateConnectionWeights.InnerText = np.pMutateConnectionWeights.ToString();

			XmlElement xmlPMutateAddNode = XmlUtilities.AddElement(xmlNeatParameters, "pMutateAddNode");
			xmlPMutateAddNode.InnerText = np.pMutateAddNode.ToString();

			XmlElement xmlPMutateAddConnection = XmlUtilities.AddElement(xmlNeatParameters, "pMutateAddConnection");
			xmlPMutateAddConnection.InnerText = np.pMutateAddConnection.ToString();

			XmlElement xmlPMutateDeleteConnection = XmlUtilities.AddElement(xmlNeatParameters, "pMutateDeleteConnection");
			xmlPMutateDeleteConnection.InnerText = np.pMutateDeleteConnection.ToString();

			XmlElement xmlPMutateDeleteSimpleNeuron = XmlUtilities.AddElement(xmlNeatParameters, "pMutateDeleteSimpleNeuron");
			xmlPMutateDeleteSimpleNeuron.InnerText = np.pMutateDeleteSimpleNeuron.ToString();

			XmlElement xmlConnectionMutationParameterGroupList = XmlUtilities.AddElement(xmlNeatParameters, "ConnectionMutationParameterGroupList");
			foreach(ConnectionMutationParameterGroup cmpg in np.ConnectionMutationParameterGroupList)
			{
				XmlElement xmlConnectionMutationParameterGroup = XmlUtilities.AddElement(xmlNeatParameters, "ConnectionMutationParameterGroup");
				Write(xmlConnectionMutationParameterGroup, cmpg);
			}
	
			XmlElement xmlCompatibilityThreshold = XmlUtilities.AddElement(xmlNeatParameters, "compatibilityThreshold");
			xmlCompatibilityThreshold.InnerText = np.compatibilityThreshold.ToString();

			XmlElement xmlCompatibilityDisjointCoeff = XmlUtilities.AddElement(xmlNeatParameters, "compatibilityDisjointCoeff");
			xmlCompatibilityDisjointCoeff.InnerText = np.compatibilityDisjointCoeff.ToString();

			XmlElement xmlCompatibilityExcessCoeff = XmlUtilities.AddElement(xmlNeatParameters, "compatibilityExcessCoeff");
			xmlCompatibilityExcessCoeff.InnerText = np.compatibilityExcessCoeff.ToString();

			XmlElement xmlCompatibilityWeightDeltaCoeff = XmlUtilities.AddElement(xmlNeatParameters, "compatibilityWeightDeltaCoeff");
			xmlCompatibilityWeightDeltaCoeff.InnerText = np.compatibilityWeightDeltaCoeff.ToString();

			XmlElement xmlElitismProportion = XmlUtilities.AddElement(xmlNeatParameters, "elitismProportion");
			xmlElitismProportion.InnerText = np.elitismProportion.ToString();

			XmlElement xmlSelectionProportion = XmlUtilities.AddElement(xmlNeatParameters, "selectionProportion");
			xmlSelectionProportion.InnerText = np.selectionProportion.ToString();

			XmlElement xmlTargetSpeciesCountMin = XmlUtilities.AddElement(xmlNeatParameters, "targetSpeciesCountMin");
			xmlTargetSpeciesCountMin.InnerText = np.targetSpeciesCountMin.ToString();

			XmlElement xmlTargetSpeciesCountMax = XmlUtilities.AddElement(xmlNeatParameters, "targetSpeciesCountMax");
			xmlTargetSpeciesCountMax.InnerText = np.targetSpeciesCountMax.ToString();

			XmlElement xmlSpeciesDropoffAge = XmlUtilities.AddElement(xmlNeatParameters, "speciesDropoffAge");
			xmlSpeciesDropoffAge.InnerText = np.speciesDropoffAge.ToString();

			XmlElement xmlPruningPhaseBeginComplexityThreshold = XmlUtilities.AddElement(xmlNeatParameters, "pruningPhaseBeginComplexityThreshold");
			xmlPruningPhaseBeginComplexityThreshold.InnerText = np.pruningPhaseBeginComplexityThreshold.ToString();

			XmlElement xmlPruningPhaseBeginFitnessStagnationThreshold = XmlUtilities.AddElement(xmlNeatParameters, "pruningPhaseBeginFitnessStagnationThreshold");
			xmlPruningPhaseBeginFitnessStagnationThreshold.InnerText = np.pruningPhaseBeginFitnessStagnationThreshold.ToString();

			XmlElement xmlPruningPhaseEndComplexityStagnationThreshold = XmlUtilities.AddElement(xmlNeatParameters, "pruningPhaseEndComplexityStagnationThreshold");
			xmlPruningPhaseEndComplexityStagnationThreshold.InnerText = np.pruningPhaseEndComplexityStagnationThreshold.ToString();

			XmlElement xmlConnectionWeightRange = XmlUtilities.AddElement(xmlNeatParameters, "connectionWeightRange");
			xmlConnectionWeightRange.InnerText = np.connectionWeightRange.ToString();
		}


		private static void Write(XmlNode parentNode, ConnectionMutationParameterGroup cmpg)
		{
			XmlElement xmlActivationProportion = XmlUtilities.AddElement(parentNode, "ActivationProportion");
			xmlActivationProportion.InnerText = cmpg.ActivationProportion.ToString();

			XmlElement xmlPerturbationType = XmlUtilities.AddElement(parentNode, "PerturbationType");
			xmlPerturbationType.InnerText = cmpg.PerturbationType.ToString();
			
			XmlElement xmlSelectionType = XmlUtilities.AddElement(parentNode, "SelectionType");
			xmlSelectionType.InnerText =  cmpg.SelectionType.ToString();

			XmlElement xmlProportion = XmlUtilities.AddElement(parentNode, "Proportion");
			xmlProportion.InnerText = cmpg.Proportion.ToString();

			XmlElement xmlQuantity = XmlUtilities.AddElement(parentNode, "Quantity");
			xmlQuantity.InnerText = cmpg.Quantity.ToString();

			XmlElement xmlPerturbationFactor = XmlUtilities.AddElement(parentNode, "PerturbationFactor");
			xmlPerturbationFactor.InnerText = cmpg.PerturbationFactor.ToString();

			XmlElement xmlSigma = XmlUtilities.AddElement(parentNode, "Sigma");
			xmlSigma.InnerText = cmpg.Sigma.ToString();
		}
	}
}
