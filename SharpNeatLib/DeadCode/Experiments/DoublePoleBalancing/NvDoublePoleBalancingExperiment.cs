using System;

using SharpNeatLib.Evolution;
using SharpNeatLib.GenomeEvaluators;
using SharpNeatLib.NetworkEvaluators;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class NvDoublePoleBalancingExperiment : IExperiment
	{
		INetworkListEvaluator networkListEvaluator;
		IGenomeListEvaluator genomeListEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoidApproximation();

		#region Constructor

		public NvDoublePoleBalancingExperiment()
		{
			ResetEvaluators(activationFunction);
		}

		#endregion

		#region IExperiment Members

		public AbstractExperimentView CreateExperimentView()
		{
			return null;
		}

		public void ResetEvaluators(IActivationFunction activationFn)
		{
			networkListEvaluator = new NvDoublePoleBalancingNetworkEvaluator();
			genomeListEvaluator =  new DeterministicSingleGenomeEvaluator(networkListEvaluator, activationFn);
		}

		public IGenomeListEvaluator GenomeListEvaluator
		{
			get
			{
				return genomeListEvaluator;
			}
		}

		public SharpNeatLib.NetworkEvaluators.INetworkListEvaluator NetworkListEvaluator
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

		public string ExplanatoryText
		{
			get
			{
				return @"Double Pole with no velocity inputs.";
			}
		}

		public NeatParameters DefaultNeatParameters
		{
			get
			{
				NeatParameters np = new NeatParameters();
				np.populationSize = 500;
				return np;
			}
		}

		#endregion
	}
}
