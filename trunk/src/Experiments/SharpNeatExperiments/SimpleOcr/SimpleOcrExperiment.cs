using System;
using System.Collections;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Experiments.Views;

namespace SharpNeatLib.Experiments
{
	public class SimpleOcrExperiment : IExperiment
	{
		IPopulationEvaluator populationEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoid();
		SimpleOcrNetworkEvaluator simpleOcrNetworkEvaluator;

		#region Constructor

		public SimpleOcrExperiment()
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
			simpleOcrNetworkEvaluator = new SimpleOcrNetworkEvaluator('a','z');
			populationEvaluator = new SingleFilePopulationEvaluator(simpleOcrNetworkEvaluator, activationFn);
		}

		public int InputNeuronCount
		{
			get
			{
                // Ensure simpleOcrNetworkEvaluator has been created.
                IPopulationEvaluator oPopulationEvaluator = this.PopulationEvaluator;
				return simpleOcrNetworkEvaluator.InputNeuronCount;
			}
		}

		public int OutputNeuronCount
		{
			get
			{
                IPopulationEvaluator oPopulationEvaluator = this.PopulationEvaluator;
				return simpleOcrNetworkEvaluator.OutputNeuronCount;
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

				np.pMutateAddConnection = 0.05;

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
			//return null;
			return new SimpleOcrExperimentView(this);
		}

		public string ExplanatoryText
		{
			get
			{
				return @"A simple OCR or pattern classification problem. 

There are 26 patterns, each representing one of the 26 lower case letters of the english alphabet. 
Each pattern is is a bitmap (1 bit-plane image) of size 7 pixels wide by 9 high. There are therefore 7*9=63 pixels
in each image, each of which is associated with its own input neuron. An output neuron is defined for each character
(so 26 outputs in total) and the task for the evolved networks is to set the output high that corresponds to the character
image being applied to the input nodes.

Because of the large input/output space it is recommended that searches start with either no connections or a very small
proportion of input/output interconnections, e.g. 0.01 seems to work well. Also, the search is largely one of connections 
rather than connection weights, therefore the default search parameters use a higher 'add connection' rate than normal.";
			}
		}

		#endregion

        
	}
}
