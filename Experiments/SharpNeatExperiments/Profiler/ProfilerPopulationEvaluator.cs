using System;
using System.Collections;
using System.IO;
using System.Xml;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeatGenome.Xml;
using SharpNeatLib.Experiments.TicTacToe;

namespace SharpNeatLib.Experiments
{
	public class ProfilerPopulationEvaluator : IPopulationEvaluator
	{
		ulong evaluationCount;

		#region IPopulationEvaluator Members

		public void EvaluatePopulation(Population pop, EvolutionAlgorithm ea)
		{
			int genomeCount = pop.GenomeList.Count;
			for(int i=0; i<genomeCount; i++)
			{
				pop.GenomeList[i].Fitness = EvolutionAlgorithm.MIN_GENOME_FITNESS;
			}
			evaluationCount += (ulong)genomeCount;
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
				return "";
			}
		}

		public bool BestIsIntermediateChampion
		{
			get
			{	
				return false;
			}
		}

		public bool SearchCompleted
		{
			get
			{
				return false;
			}
		}

		#endregion
	}
}
