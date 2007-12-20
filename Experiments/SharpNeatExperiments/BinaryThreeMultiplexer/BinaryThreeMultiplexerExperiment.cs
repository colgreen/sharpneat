using System;
using System.Collections;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class BinaryThreeMultiplexerExperiment : IExperiment
	{
		IPopulationEvaluator populationEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoid();

		#region Constructor

		public BinaryThreeMultiplexerExperiment()
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
			populationEvaluator = new SingleFilePopulationEvaluator(new BinaryThreeMultiplexerNetworkEvaluator(), activationFn);
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
				return @"3-Multiplexer experiment with binary value inputs/outputs.
Evaluation consists of testing a network against each of the 8 (=2^3) possible input combinations.
An output less than 0.5 is considered a binary 0 answer, and >=0.5 a binary 1. However, fitness is scored
based on how close a given network's response is to each correct answer, therefore the maximum fitness 
is 8 if exact answers are given, and the minimum possible fitness while still being correct overall is 
approx. 4.

To distinguish between correct low scorers and incorrect low scorers an additional value of 100 is added 
to the score for networks that give a correct response to each of the 8 test cases.

Max fitness is therefore 108.";
			}
		}

		#endregion
	}
}
