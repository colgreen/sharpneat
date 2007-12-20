using System;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NetworkEvaluators;

namespace SharpNeatLib.GenomeEvaluators
{
	/// <summary>
	/// This genome list evaluator is intended for deterministic evaluation experiments with 
	/// independent (single) evaluation of each genome.
	/// 
	/// For such problem types we can achieve a performance increase by not re-evaluating genomes
	/// that were evaluated in a previous generation, namely elite genomes that persist for more 
	/// then one generation. We can check the EvaluationCount of a genome to determine if it has
	/// already been evaluated.
	/// </summary>
	public class DeterministicSingleGenomeEvaluator : IGenomeListEvaluator
	{
		INetworkListEvaluator	networkListEvaluator;
		IActivationFunction		activationFn;
		NetworkList				networkList = new NetworkList();
		
		#region Constructor

		public DeterministicSingleGenomeEvaluator(INetworkListEvaluator networkListEvaluator, IActivationFunction activationFn)
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

			if(networkListEvaluator.EvaluationSchemeChanged)
			{	// The scheme has changed so we should evaluate all genomes because any old fitness values
				// will relate to the old evaluation scheme.
				for(int genomeIdx=0; genomeIdx<genomeListCount; genomeIdx++)
					networkList.Add(genomeList[genomeIdx].Decode(activationFn));

				// Evaluate the networks.
				double[] fitnessValueArray = networkListEvaluator.EvaluateNetworkList(networkList);

				// Copy the fitness values into their respective genome's.
				for(int genomeIdx=0, fitnessIdx=0; genomeIdx<genomeListCount; genomeIdx++)
				{
					genomeList[genomeIdx].Fitness = Math.Max(GenomeEvaluatorUtilities.MIN_GENOME_FITNESS, fitnessValueArray[fitnessIdx++]);

					// Reset these genome level statistics.
					genomeList[genomeIdx].TotalFitness = genomeList[genomeIdx].Fitness;
					genomeList[genomeIdx].EvaluationCount = 1;
				}
			}
			else
			{
				// Only evaluate new genomes that have not already been evaluated. This is OK because the 
				// evaluation scheme is deterministic and will therefore always give the same fitness for 
				// a given genome.
				for(int genomeIdx=0; genomeIdx<genomeListCount; genomeIdx++)
				{	
					if(genomeList[genomeIdx].EvaluationCount==0)
						networkList.Add(genomeList[genomeIdx].Decode(activationFn));
				}

				// Evaluate the networks.
				double[] fitnessValueArray = networkListEvaluator.EvaluateNetworkList(networkList);

				// Copy the fitness values into their respective genome's.
				for(int genomeIdx=0, fitnessIdx=0; genomeIdx<genomeListCount; genomeIdx++)
				{
					// Only assign fitness to genomes that had their fitness evaluated. See above loop.
					if(genomeList[genomeIdx].EvaluationCount==0)
					{	// Avoid zero fitness values because fitness sharing will cause the whole population
						// to be culled if the total pop fitness is zero.
						genomeList[genomeIdx].Fitness = Math.Max(GenomeEvaluatorUtilities.MIN_GENOME_FITNESS, fitnessValueArray[fitnessIdx++]);
						genomeList[genomeIdx].TotalFitness += genomeList[genomeIdx].Fitness;
						genomeList[genomeIdx].EvaluationCount++;
					}
				}
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
