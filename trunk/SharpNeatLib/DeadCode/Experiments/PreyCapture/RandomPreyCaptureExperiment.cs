using System;

using SharpNeatLib.Evolution;
using SharpNeatLib.GenomeEvaluators;
using SharpNeatLib.NetworkEvaluators;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class RandomPreyCaptureExperiment : IExperiment
	{
		INetworkListEvaluator networkListEvaluator;
		IGenomeListEvaluator genomeListEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoidApproximation();
		
		public RandomPreyCaptureExperiment()
		{
			ResetEvaluators(activationFunction);
		}

		public AbstractExperimentView CreateExperimentView()
		{
			return null;
		}

		public void ResetEvaluators(IActivationFunction activationFn)
		{
			networkListEvaluator = new RandomPreyCaptureNetworkEvaluator(24, 4.5, 0);
			genomeListEvaluator =  new DeterministicSingleGenomeEvaluator(networkListEvaluator, activationFn);
		}

		public IGenomeListEvaluator GenomeListEvaluator
		{
			get
			{
				return genomeListEvaluator;
			}
		}

		public INetworkListEvaluator NetworkListEvaluator
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

		/// <summary>
		/// A description of the evaluator and domain to aid new users.
		/// </summary>
		public string ExplanatoryText
		{
			get
			{
				return @"Prey capture within a 24x24 grid. As described for the experiments within Faustino Gomez and Risto Miikkulainen (1997). Incremental Evolution Of Complex General Behavior, Adaptive Behavior, 5:317-342, 1997..
The paper can be found online at http://nn.cs.utexas.edu/pub-view.php?RECORD_KEY(Pubs)=PubID&PubID(Pubs)=35

Each genome is tested exactly 100 times and the number of captures are counted and returned as a fitness value.
Incremental evolution is achieved by increasing the task difficulty when the best genome scores 90 or above.
Difficulty is increased by giving the prey a head start of 0 to 4 moves and then by increasing its speed. 

Difficulty levels are:
Level 0 -> (0.0 , 0)
Level 1 -> (0.0 , 1)
Level 2 -> (0.0 , 2)
Level 3 -> (0.0 , 3)
Level 4 -> (0.0 , 4)
Level 5 -> (0.1 , 4)
Level 6 -> (0.2 , 4)
Level 7 -> (0.3 , 4)
Level 8 -> (0.4 , 4)
Level 9 -> (0.6 , 4)
Level 10 -> (0.8 , 4)
Level 11 -> (1.0 , 4)";
			}
		}

		public NeatParameters DefaultNeatParameters
		{
			get
			{
				NeatParameters np = new NeatParameters();

				np.populationSize = 1000;
				np.elitismProportion = 0.1;
				np.selectionProportion = 0.1;
				np.pInitialPopulationInterconnections = 0.01F;
				np.targetSpeciesCountMin = 15;
				np.targetSpeciesCountMax = 30;
				np.pruningPhaseBeginComplexityThreshold = 15;
				np.pruningPhaseEndComplexityStagnationThreshold = 20;
				np.pMutateAddNode = 0.01;
				np.pMutateAddConnection = 0.1;

				np.pOffspringAsexual = 0.9;
				np.pOffspringSexual = 0.1;

				return np;
			}
		}
	}
}
