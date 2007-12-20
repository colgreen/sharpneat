using System;

using SharpNeatLib.Evolution;
using SharpNeatLib.GenomeEvaluators;
using SharpNeatLib.NetworkEvaluators;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.ExperimentViews;

namespace SharpNeatLib.Experiments
{
	public class BinaryElevenMultiplexerExperiment : IExperiment
	{
		INetworkListEvaluator networkListEvaluator;
		IGenomeListEvaluator genomeListEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoidApproximation();

		#region Constructor

		public BinaryElevenMultiplexerExperiment()
		{
			ResetEvaluators(activationFunction);
		}

		#endregion

		#region IExperiment Members

		public AbstractExperimentView CreateExperimentView()
		{
			//return new BinaryElevenMultiplexerExperimentView();
			return null;
		}

		public void ResetEvaluators(IActivationFunction activationFn)
		{
			networkListEvaluator = new BinaryElevenMultiplexerNetworkEvaluator();
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
				return "11-Multiplexer experiment with binary only inputs/outputs.";
			}
		}

		public NeatParameters DefaultNeatParameters
		{
			get
			{
				NeatParameters np = new NeatParameters();
				np.pInitialPopulationInterconnections = 0.01F;
				np.pruningPhaseEndComplexityStagnationThreshold = 50;
				np.pMutateAddNode = 0.01;
				np.pMutateAddConnection = 0.1;
				return np;
			}
		}

		#endregion
	}
}
