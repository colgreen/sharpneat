using System;
using System.Collections;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class NvAntiWiggleDoublePoleBalancingExperiment : IExperiment
	{
		IPopulationEvaluator populationEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoid();

		#region Constructor

		public NvAntiWiggleDoublePoleBalancingExperiment()
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
			populationEvaluator = new SingleFilePopulationEvaluator(new NvAntiWiggleDoublePoleBalancingNetworkEvaluator(), activationFn);
		}

		public int InputNeuronCount
		{
			get
			{
				return 3;
			}
		}

		public int OutputNeuronCount
		{
			get
			{
				return 1;
			}
		}

		public NeatParameters DefaultNeatParameters
		{
			get
			{
				NeatParameters np = new NeatParameters();

				np.populationSize = 1000;

				// Experimentally determined value. This gives some speciation from the outset.
				np.compatibilityThreshold = 1.6F; 

				np.targetSpeciesCountMin = 40;
				np.targetSpeciesCountMax = 50;

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
				return @"Double pole balancing with no velocity inputs and customised fitness function to punish pole wiggling.

See Antiwiggle_Explanation_Email.txt for an explanation of the anti-wiggle evluation scheme.

Theoretical max fitness is 50,010. That represents a balanced system for the maximum of 50,000 timesteps and the maximum antiwiggle
factor of 10.0. In practice the max anti-wiggle factor will be more like 1.0 since some movement is necessary to balance the poles!
";
			}
		}

		#endregion
	}
}
