using System;
using System.Collections;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class VectorCrossProductExperiment : IExperiment
	{
		IPopulationEvaluator populationEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoid();

		#region Constructor

		public VectorCrossProductExperiment()
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
			populationEvaluator = new SingleFilePopulationEvaluator(new VectorCrossProductNetworkEvaluator(), activationFn);
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
				return 3;
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
				return @"Vector Cross Product experiment.";
			}
		}

		#endregion
	}
}
