using System;
using System.Collections;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class PreyCaptureExperiment : IExperiment
	{
		IPopulationEvaluator populationEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoid();

		#region Constructor

		public PreyCaptureExperiment()
		{
		}

		#endregion

		#region IExperiment Members
		
		/// <summary>
		/// This method is called immediately following instantiation of an experiment. It is used
		/// to pass in a hashtable of string key-value pairs from the 'experimentParameters' 
		/// block of the experiment configuration block within the application config file.
		/// 
		/// If no parameters where specified then an empty Hashtable is used.
		/// </summary>
		/// <param name="parameterTable"></param>
		public void LoadExperimentParameters(Hashtable parameterTable)
		{
		}

		public IPopulationEvaluator PopulationEvaluator
		{
			get
			{
				if(populationEvaluator==null)
					ResetEvaluator(activationFunction);

				return populationEvaluator;
			}
		}

		public void ResetEvaluator(IActivationFunction activationFn)
		{
			populationEvaluator = new PreyCapturePopulationEvaluator(activationFn);
		}

		public int InputNeuronCount
		{
			get
			{
				return 13;
			}
		}

		public int OutputNeuronCount
		{
			get
			{
				return 4;
			}
		}

		public NeatParameters DefaultNeatParameters
		{
			get
			{
				NeatParameters np = new NeatParameters();

				np.populationSize = 1000;
				np.pInitialPopulationInterconnections = 0.01F;
				np.targetSpeciesCountMin = 40;
				np.targetSpeciesCountMax = 50;
				//np.pruningPhaseBeginComplexityThreshold = 2.0F;
				np.pruningPhaseEndComplexityStagnationThreshold = 20;

				np.pOffspringAsexual = 0.8;
				np.pOffspringSexual = 0.2;

				return np;
			}
		}

		public IActivationFunction SuggestedActivationFunction
		{
			get
			{
				return activationFunction;
			}
		}

		public AbstractExperimentView CreateExperimentView()
		{
			return null;
		}

		public string ExplanatoryText
		{
			get
			{
				return @"Prey capture within a 24x24 grid. As described for the experiments within Faustino Gomez and Risto Miikkulainen (1997). Incremental Evolution Of Complex General Behavior, Adaptive Behavior, 5:317-342, 1997..
The paper can be found online at http://nn.cs.utexas.edu/pub-view.php?RECORD_KEY(Pubs)=PubID&PubID(Pubs)=35

Each genome is tested exactly 100 times and the number of captures are counted and returned as a fitness value.
Incremental evolution is achieved by increasing the task difficulty when the best genome scores 100 at the current level.
Difficulty is increased by giving the prey a head start of between 0 to 4 moves and then by increasing its speed. 

Difficulty levels are (speed, head-start moves), note that speed is actually the probability of prey moving per timestep:
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

		#endregion
	}
}
