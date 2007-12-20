using System;

using SharpNeatLib;
using SharpNeatLib.Evolution;
using SharpNeatLib.Experiments;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeatGenome;

namespace NeatParameterOptimizer
{
	public class MetaEvolutionAlgorithm
	{
		NeatParametersWrapperList npwList=null;
		MetaNeatParameters mnp=null;
		IActivationFunction activationFunction=null;
		int generation;

		#region Properties

		public NeatParametersWrapperList NeatParametersWrapperList
		{
			get
			{
				return npwList;
			}
		}

		public MetaNeatParameters MetaNeatParameters
		{
			get
			{
				return mnp;
			}
		}

		public int Generation
		{
			get
			{
				return generation;
			}
		}

		public NeatParametersWrapper Best
		{
			get
			{
				return npwList[0];
			}
		}

		public double MeanFitness
		{
			get
			{
				return npwList.MeanFitness;
			}
		}

		#endregion

		#region Public Methods

		public void Initialise(NeatParametersWrapperList npwList, MetaNeatParameters mnp, IActivationFunction activationFunction)
		{
			this.npwList = npwList;
			this.mnp = mnp;
			this.activationFunction = activationFunction;
			generation=0;
		}

		public void RunPerformanceTest(IExperiment experiment, EAStoppingCondition eaStoppingCondition)
		{
			if(mnp==null)
				throw new ApplicationException("MetaNeatParameters opbject is null, call Initialise() before running algorithm.");

			//----- Evaluate each of the npw's
			foreach(NeatParametersWrapper npw in npwList)
				EvaluateNeatParametersWrapper(npw, experiment, eaStoppingCondition);

			// Order npwList best to worst. Randomize order of npw's with equal fitness.
			npwList.Sort();

			npwList.UpdateStats();
			generation++;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="npwList"></param>
		/// <param name="experiment">The IExperiment to use as the basis for evaluations.</param>
		/// <param name="mnp"></param>
		/// <param name="stoppingCondition"></param>
		public void PerformOneGeneration(IExperiment experiment, EAStoppingCondition eaStoppingCondition)
		{
			if(mnp==null)
				throw new ApplicationException("MetaNeatParameters opbject is null, call Initialise() before running algorithm.");

		//----- Evaluate each of the npw's
			foreach(NeatParametersWrapper npw in npwList)
				EvaluateNeatParametersWrapper(npw, experiment, eaStoppingCondition);

			// Order npwList best to worst. Randomize order of npw's with equal fitness.
			npwList.Sort();

		//----- Create some new offspring from the elite.
			int npwListLength = npwList.Count;
			int eliteCount = (int)Math.Max(1.0, npwListLength * 0.2);
			int offspringCount = npwListLength - eliteCount;

			// Calculate total fitness of all elite - ready for the roulette wheel selection function.
			npwList.SelectionTotalFitness = 0; 
			for(int i=0; i<eliteCount; i++)
				npwList.SelectionTotalFitness += npwList[i].Fitness;

			for(int i=0; i<offspringCount; i++)
			{
				// Select one of the elite using roulette wheel selection.
				NeatParametersWrapper parentNpw = RouletteWheelSelect(npwList);

				// Spawn an npw from the elite parent and add it to npwList.
				npwList.Add(parentNpw.CreateOffspring_Asexual(this));
			}

			// Re-sort npwList. Note that parents and offspring are all mixed up at this stage.
			// Also note that npw.OrderRandomizer is 0 for new offspring, giving them priority
			// over older npw's with the same fitness.
			npwList.Sort();

			// Cull the weakest npw's so that npwList.Count is taken back to it's original length.
			npwList.RemoveRange(npwListLength, npwList.Count-npwListLength);

			// 
			npwList.UpdateStats();
			generation++;
		}

		#endregion

		#region Private Methods

		private void EvaluateNeatParametersWrapper(NeatParametersWrapper npw, IExperiment experiment, EAStoppingCondition eaStoppingCondition)
		{
			double fitness=0;

			// Run EvaluationAlgorithm several times and take the average bestFitness achieved.
			for(int i=0; i<mnp.EaRunsPerMetaEvaluation; i++)
			{
				// Create a population.
				IdGenerator idGenerator = new IdGenerator();
			
				GenomeList genomeList =  GenomeFactory.CreateGenomeList(
					npw.NeatParameters,
					idGenerator,
					experiment.NetworkEvaluator.InputNeuronCount,
					experiment.NetworkEvaluator.OutputNeuronCount,
					npw.NeatParameters.pInitialPopulationInterconnections,
					npw.NeatParameters.populationSize);

				Population pop = new Population(idGenerator, genomeList);

				// Create an EvolutionAlgorithm.
				IGenomeEvaluator organismEvaluator = new UniversalGenomeEvaluator(experiment.NetworkEvaluator, activationFunction);
				EvolutionAlgorithm ea = new EvolutionAlgorithm(pop, organismEvaluator, npw.NeatParameters);

				// Now run the algorithm until the stopping condition becomes true.
				long tickStart = System.DateTime.Now.Ticks;
				long durationSecs;
				for(;;)
				{
					ea.PerformOneGeneration();

					// Test for stopping conditions.
					if(ea.BestFitness >= experiment.NetworkEvaluator.MaxFitness)
						break;

					if((eaStoppingCondition.StoppingConditionType == EAStoppingConditionType.MaxDuration) || 
						(eaStoppingCondition.StoppingConditionType == EAStoppingConditionType.Combined))
					{
						durationSecs = (System.DateTime.Now.Ticks - tickStart) / 10000000;
						if(durationSecs >= eaStoppingCondition.MaxEvaluationDurationSecs)
							break;
					}

					if((eaStoppingCondition.StoppingConditionType == EAStoppingConditionType.MaxGenerations) || 
						(eaStoppingCondition.StoppingConditionType == EAStoppingConditionType.Combined))
					{
						if(ea.Generation >= eaStoppingCondition.MaxGenerations)
							break;
					}
				}

				// Fitness is max fitness achieved divided by time taken (in seconds). Limit the max achievable fitness
				// by not allowing durationSecsDouble to go below 0.2, this also solves the problem of a solution in the 
				// initial population or early generations finding a solution in 0 time.
				//double durationSecsDouble = Math.Max(0.2, ((double)(System.DateTime.Now.Ticks - tickStart)) / 10000000.0);

				fitness += (ea.BestFitness / (double)ea.Generation) * (double)eaStoppingCondition.MaxGenerations;
			}

			// Take the average.
			npw.Fitness = fitness / (double)mnp.EaRunsPerMetaEvaluation;
		}

		private NeatParametersWrapper RouletteWheelSelect(NeatParametersWrapperList npwList)
		{
			double selectValue = (Utilities.NextDouble() * npwList.SelectionTotalFitness);
			double accumulator=0.0;

			int bound = npwList.Count;
			for(int idx=0; idx<bound; idx++)
			{
				NeatParametersWrapper npw = npwList[idx];
				accumulator += npw.Fitness;
				if(selectValue <= accumulator)
					return npw;
			}
			// Should never reach here.
			return null;
		}

		#endregion
	}
}
