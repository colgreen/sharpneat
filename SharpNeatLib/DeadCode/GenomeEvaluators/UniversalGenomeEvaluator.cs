using System;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NetworkEvaluators;

namespace SharpNeatLib.GenomeEvaluators
{
	/// <summary>
	/// This universal evaluator will decode all of the geomes into networks and pass a 
	/// complete list of networks to the INetworkListEvaluator. Therefore this genome 
	/// evaluator can be used for all types of experiments, although it may not be the most
	/// efficient solution, e.g. in deterministic experiments you may wish to avoid
	/// re-evaluating elite genomes at each generation but here we evaluate them at each
	/// generation.
	/// </summary>
	public class UniversalGenomeEvaluator : IGenomeListEvaluator
	{
		INetworkListEvaluator	networkListEvaluator;
		IActivationFunction		activationFn;
		NetworkList				networkList = new NetworkList();
		
		#region Constructor

		public UniversalGenomeEvaluator(INetworkListEvaluator networkListEvaluator, IActivationFunction activationFn)
		{
			this.networkListEvaluator = networkListEvaluator;
			this.activationFn = activationFn;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Evaluate each genome in the list and assign a fitness value to their Fitness property.
		/// </summary>
		/// <param name="genomeList"></param>
		public void EvaluateGenomeList(GenomeList genomeList)
		{
			// TODO: Avoid clear by overwriting existing entries?
			networkList.Clear();

			// Build a list of INetworks by decoding the genomes.
			int genomeListCount = genomeList.Count;
			for(int genomeIdx=0; genomeIdx<genomeListCount; genomeIdx++)
			{	// Note. Decode may return null if genome is not guaranteed to express a valid network.
				// INetworkListEvaluator must therefore handle null networks.
				networkList.Add(genomeList[genomeIdx].Decode(activationFn));
			}

			// Evaluate the networks.
			double[] fitnessValueArray = networkListEvaluator.EvaluateNetworkList(networkList);

			// Copy the fitness values into their respective genome's.
			for(int genomeIdx=0; genomeIdx<genomeListCount; genomeIdx++)
			{	// Avoid zero fitness values because fitness sharing will cause the whole population
				// to be culled if the total pop fitness is zero.
				genomeList[genomeIdx].Fitness = Math.Max(GenomeEvaluatorUtilities.MIN_GENOME_FITNESS, fitnessValueArray[genomeIdx]);
				genomeList[genomeIdx].TotalFitness += genomeList[genomeIdx].Fitness;
				genomeList[genomeIdx].EvaluationCount++;
			}
		}

		/// <summary>
		/// The total number of evaluations performed.
		/// </summary>
		public ulong EvaluationCount
		{
			get
			{
				return networkListEvaluator.EvaluationCount;
			}
		}

		/// <summary>
		/// A human readable message that describes the state of the evaluator. This is useful if the
		/// evaluator has several modes (e.g. difficulty levels in incremenetal evolution) and we want 
		/// to let the user know what mode the evaluator is in.
		/// </summary>
		public string EvaluatorStateMessage
		{
			get
			{
				return networkListEvaluator.EvaluatorStateMessage;
			}
		}

		/// <summary>
		/// Indicates that the current best genome is a champion at the current level of difficulty.
		/// If there is only one difficulty level then the 'SearchCompleted' flag should also be set.
		/// </summary>
		public bool BestIsIntermediateChampion
		{
			get
			{
				return networkListEvaluator.BestIsIntermediateChampion;
			}
		}

		/// <summary>
		/// Indicates that the best solution meets the evaluator's end criteria.
		/// </summary>
		public bool SearchCompleted
		{
			get
			{
				return networkListEvaluator.SearchCompleted;
			}
		}

		#endregion
	}
}
