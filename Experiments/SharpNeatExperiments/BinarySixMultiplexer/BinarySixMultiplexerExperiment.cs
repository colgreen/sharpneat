using System;
using System.Collections;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Experiments.Views;

namespace SharpNeatLib.Experiments
{
	public class BinarySixMultiplexerExperiment : IExperiment
	{
		IPopulationEvaluator populationEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoid();

		#region Constructor

		public BinarySixMultiplexerExperiment()
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
			populationEvaluator = new SingleFilePopulationEvaluator(new BinarySixMultiplexerNetworkEvaluator(), activationFn);
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
				return 1;
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
			return new BinarySixMultiplexerExperimentView();
		}

		public string ExplanatoryText
		{
			get
			{
				return @"6-Multiplexer experiment with binary value inputs/outputs.
Evaluation consists of testing a network against each of the 64 (=2^6) possible input combinations.
An output less than 0.5 is considered a binary 0 answer, and >=0.5 a binary 1. However, fitness is scored
based on how close a given network's response is to each correct answer, therefore the maximum fitness 
is 64 if exact answers are given, and the minimum possible fitness while still being correct overall is 
approx. 32.

To distinguish between correct low scorers and incorrect low scorers an additional value of 1000 is added 
to the score of networks that give a correct response to each of the 64 test cases.

Max fitness is therefore 1064.";
			}
		}

		#endregion
	}
}
