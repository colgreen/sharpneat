using System;
using System.Collections;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class OneInputFunctionRegressionExperiment : IExperiment
	{
		IPopulationEvaluator populationEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoid();
		OneInputFunctionRegressionNetworkEvaluator networkEvaluator;

		#region Constructor

		public OneInputFunctionRegressionExperiment()
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
			networkEvaluator = new OneInputFunctionRegressionNetworkEvaluator();
			populationEvaluator = new SingleFilePopulationEvaluator(networkEvaluator, activationFn);
		}

		public int InputNeuronCount
		{
			get
			{
				return 1;
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
			return new OneInputFunctionRegressionExperimentView(networkEvaluator);
		}

		public string ExplanatoryText
		{
			get
			{
				return @"Function Regression. Here we evaluate a network's ability to find an approximation for 
a function with a single input parameter. The default function is a function built by joining various lines and curves
to form a function complex enough to challenge SharpNEAT's ability to find a solution.

See the OneInputFunctionRegressionNetworkEvalutor.TargetFunction() method for some other pre-defined functions, it
will be necessary to un-comment the function you want and recompile.";
			}
		}

		#endregion
	}
}
