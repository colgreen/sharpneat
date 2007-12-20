using System;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.NetworkEvaluators
{
	public interface INetworkListEvaluator
	{
		/// <summary>
		/// Evaluate the networks with NetworkList and assign each if them a fitness score.
		/// </summary>
		/// <param name="network"></param>
		/// <returns>An array of doubles that represnt the fitness values of the networks in the list.</returns>
		double[] EvaluateNetworkList(NetworkList networkList);

		/// <summary>
		/// The total number of evaluations performed.
		/// </summary>
		ulong EvaluationCount
		{
			get;
		}

		/// <summary>
		/// Indicates that the evaluation scheme has changed. This flag will generally be set immediately
		/// whereas the EvaluatorStateMessage will not reflect the change until after the following round
		/// of evaluation. This means that we can use EvaluatorStateMessage to determine which scheme was
		/// used to obtain the current set of fitness values, while at the same time providing a flag to 
		/// notify us of a change in teh evaluation scheme.
		/// </summary>
		bool EvaluationSchemeChanged
		{
			get;
		}

		/// <summary>
		/// A human readable message that describes the state of the evaluator. This is useful if the
		/// evaluator has several modes (e.g. difficulty levels in incremenetal evolution) and we want 
		/// to let the user know what mode the evaluator is in.
		/// </summary>
		string EvaluatorStateMessage
		{
			get;
		}

		/// <summary>
		/// Indicates that the current best genome is a champion at the current level of difficulty.
		/// If there is only one difficulty level then the 'SearchCompleted' flag should also be set.
		/// </summary>
		bool BestIsIntermediateChampion
		{
			get;
		}

		/// <summary>
		/// Indicates that the best solution meets the evaluator's end criteria.
		/// </summary>
		bool SearchCompleted
		{
			get;
		}

		/// <summary>
		/// The number of input neurons required for the domain.
		/// </summary>
		int InputNeuronCount
		{
			get;
		}

		/// <summary>
		/// The number of output neurons required for the domain.
		/// </summary>
		int OutputNeuronCount
		{
			get;
		}
	}
}
