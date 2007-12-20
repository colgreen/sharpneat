using System;

using SharpNeatLib.Evolution;
using SharpNeatLib.GenomeEvaluators;
using SharpNeatLib.NetworkEvaluators;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class TicTacToeExperiment : IExperiment
	{
		INetworkListEvaluator networkListEvaluator;
		IGenomeListEvaluator genomeListEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoidApproximation();

		#region Constructor

		public TicTacToeExperiment()
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
			networkListEvaluator = new TicTacToeNetworkEvaluator();
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
				return @"A Tic-Tac-Toe experiment based on the work presented in 'A Comparative Analysis of Simplification and Complexification in the Evolution of Neural Network Topologies', Derek James and Philip Tucker.
In this experiment the players used for evaluating networks are intended to be functionaly identical to those used in Derek and Philips work so that direct comparisons can be made against the experimental results presented in their paper.";

			}
		}

		public NeatParameters DefaultNeatParameters
		{
			get
			{
				NeatParameters np = new NeatParameters();

				np.compatibilityThreshold = 0.2;
				np.compatibilityWeightDeltaCoeff = 0.4;

				return np;
			}
		}

		#endregion
	}
}
