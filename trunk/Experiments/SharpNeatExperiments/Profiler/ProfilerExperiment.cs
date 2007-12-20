using System;
using System.Collections;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class ProfilerExperiment : IExperiment
	{
		IPopulationEvaluator populationEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoid();

		#region Constructor

		public ProfilerExperiment()
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
			populationEvaluator = new ProfilerPopulationEvaluator();
		}

		public int InputNeuronCount
		{
			get
			{
				return 9;
			}
		}

		public int OutputNeuronCount
		{
			get
			{
				return 9;
			}
		}

		public NeatParameters DefaultNeatParameters
		{
			get
			{
				return new NeatParameters();
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
				return @"This experiment just assigns each genome a fitness of MIN_GENOME_FITNESS. 
Therefore there is no evaluation overhead and this experimetn can be used to profile how fast the
core NEAT code executes.";
			}
		}

		#endregion
	}
}
