using System;

using SharpNeatLib.Evolution;
using SharpNeatLib.GenomeEvaluators;
using SharpNeatLib.NetworkEvaluators;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class DynamicPreyCaptureExperiment : IExperiment
	{
		INetworkListEvaluator networkListEvaluator;
		IGenomeListEvaluator genomeListEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoidApproximation();
		
		public DynamicPreyCaptureExperiment()
		{
			ResetEvaluators(activationFunction);
		}

		public AbstractExperimentView CreateExperimentView()
		{
			return null;
		}

		public void ResetEvaluators(IActivationFunction activationFn)
		{
			genomeListEvaluator =  new DynamicPreyCaptureEvaluator(activationFn, 24, 4.5, 0);
			networkListEvaluator = (INetworkListEvaluator)genomeListEvaluator;
		}

		public IGenomeListEvaluator GenomeListEvaluator
		{
			get
			{
				return genomeListEvaluator;
			}
		}

		public INetworkListEvaluator NetworkListEvaluator
		{
			get
			{
				return networkListEvaluator;
			}
		}

		public IActivationFunction SuggestedActivationFunction
		{
			get
			{
				return activationFunction;
			}
		}

		/// <summary>
		/// A description of the evaluator and domain to aid new users.
		/// </summary>
		public string ExplanatoryText
		{
			get
			{
				return "";
			}
		}

		public NeatParameters DefaultNeatParameters
		{
			get
			{
				NeatParameters np = new NeatParameters();

				np.populationSize = 1000;
				np.elitismProportion = 0.1;
				np.selectionProportion = 0.1;
				np.pInitialPopulationInterconnections = 0.01F;
				np.targetSpeciesCountMin = 15;
				np.targetSpeciesCountMax = 30;
				np.pruningPhaseBeginComplexityThreshold = 15;
				np.pruningPhaseEndComplexityStagnationThreshold = 20;
				np.pMutateAddNode = 0.01;
				np.pMutateAddConnection = 0.1;

				np.pOffspringAsexual = 0.9;
				np.pOffspringSexual = 0.1;

				return np;
			}
		}
	}
}
