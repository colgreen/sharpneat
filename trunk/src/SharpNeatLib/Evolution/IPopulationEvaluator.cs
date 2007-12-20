using System;

namespace SharpNeatLib.Evolution
{
	public interface IPopulationEvaluator
	{
		/// <summary>
		/// Evaluate the genomes within the Population argument. Implementors can choose how to evaluate
		/// the genomes and which ones to evaluate, e.g. only evaluate new genomes (EvaluationCount>0).
		/// </summary>
		/// <param name="pop"></param>
		/// <param name="ea">Some evaluators may wish to interogate the current EvolutionAlgorithm to 
		/// obtain statistical information. Most experiments though do not require this parameter.</param>
		void EvaluatePopulation(Population pop, EvolutionAlgorithm ea);

		/// <summary>
		/// The total number of evaluations performed.
		/// </summary>
		ulong EvaluationCount
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
	}
}
