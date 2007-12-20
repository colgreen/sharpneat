using System;
using System.Collections;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class PhysicalTSPControllerExperiment : IExperiment
	{
		IPopulationEvaluator populationEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoid();

		#region Constructor

		public PhysicalTSPControllerExperiment()
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
			populationEvaluator = new SingleFilePopulationEvaluator(new PhysicalTSPControllerNetworkEvaluator(), activationFn);
		}

		public int InputNeuronCount
		{
			get
			{
				return 6;
			}
		}

		public int OutputNeuronCount
		{
			get
			{
				return 2;
			}
		}

		public NeatParameters DefaultNeatParameters
		{
			get
			{
				NeatParameters np = new NeatParameters();
				np.populationSize = 1000;
				np.pOffspringAsexual = 0.8;
				np.pOffspringSexual = 0.2;

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
				return @"Here we try and find an agent that can navigate a physical mass of 1kg through
a series of destinations (in order) by electing to apply 1N of force in one of the following directions 
at each timestep - N,S,E or W.  A fifth option to apply no force also exists.

The evaluation code is setup to evaluate performance against one particular route, the PTSP Competition 
route from GECCO 2005.

The route contains 30 points an 1,000,000 is achieved for each point that is reached. Additional fitness
is achieved for speed, giving good fitness values around to 40,000,000 mark.";
			}
		}

		#endregion
	}
}
