using System;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class PreyCapturePopulationEvaluator : IPopulationEvaluator
	{
		PreyCaptureNetworkEvaluator networkEvaluator;
		IActivationFunction activationFn;
		ulong evaluationCount=0;

		bool bestIsIntermediateChampion = false;
		
		int currentDifficultyLevel=0;

		#region Constructor

		public PreyCapturePopulationEvaluator(IActivationFunction activationFn)
		{
			this.networkEvaluator = new PreyCaptureNetworkEvaluator(24, 4.5, 0);
			this.activationFn = activationFn;
		}

		#endregion

		#region IPopulationEvaluator Members

		public void EvaluatePopulation(Population pop, EvolutionAlgorithm ea)
		{
			double bestFitness=0.0;

			// Test champion flag from previous evaluation/generation.
			if(bestIsIntermediateChampion)
			{	// Reset the flag and increment difficulty level (if not already at the max difficulty).
				bestIsIntermediateChampion=false;
				
				if(currentDifficultyLevel<11)
				{
					networkEvaluator.SetDifficultyLevel(++currentDifficultyLevel);

					// Reset some key populations stats used by EvolutionAlgorithm to determine pruning phase entry/exit points.
					pop.MaxFitnessEver = 0.0;				// Old fitness values are invalid now we have a new eval scheme.
					pop.GenerationAtLastImprovement = ea.Generation;	// Use the curent generation as a new baseline.
				}
			}

			// Evaluate in single-file each genome within the population. 
			int count = pop.GenomeList.Count;
			for(int i=0; i<count; i++)
			{
				IGenome g = pop.GenomeList[i];
				INetwork network = g.Decode(activationFn);
				if(network==null)
				{	// Future genome encodings may not decode - handle the possibility.
					g.Fitness = EvolutionAlgorithm.MIN_GENOME_FITNESS;
				}
				else
				{
					g.Fitness = Math.Max(networkEvaluator.EvaluateNetwork(network),EvolutionAlgorithm.MIN_GENOME_FITNESS);
				}

				// Keep track of the best fitness seen in this round of evaluations.
				bestFitness = Math.Max(bestFitness, g.Fitness);

				// Reset these genome level statistics.
				g.TotalFitness += g.Fitness;
				g.EvaluationCount++;

				// Update master evaluation counter.
				evaluationCount++;

			}

			// Check if we need to set bestIsIntermediateChampion to signal an 
			// increment in the difficulty level next time around.
			if(bestFitness == PreyCaptureNetworkEvaluator.TRIALS_PER_EVALUATION_DBL)
			{	
				if(currentDifficultyLevel!=11)
				{	// Set champion flag.
					bestIsIntermediateChampion=true;
				}
			}
		}

		public ulong EvaluationCount
		{
			get
			{
				return evaluationCount;
			}
		}

		public string EvaluatorStateMessage
		{
			get
			{
				return "difficulty(" + currentDifficultyLevel.ToString() + ")";
			}
		}

		public bool BestIsIntermediateChampion
		{
			get
			{	
				return bestIsIntermediateChampion;
			}
		}

		public bool SearchCompleted
		{
			get
			{	// This flag is not yet supported in the main search algorithm.
				return false;
			}
		}

		#endregion
	}
}
