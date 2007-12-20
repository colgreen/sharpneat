using System;

using SharpNeatLib.Evolution;
using SharpNeatLib.ExperimentViews;
using SharpNeatLib.GenomeEvaluators;
using SharpNeatLib.NetworkEvaluators;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class SimpleOcrExperiment : IExperiment
	{
		INetworkListEvaluator networkListEvaluator;
		IGenomeListEvaluator genomeListEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoidApproximation();

		#region Constructor

		public SimpleOcrExperiment()
		{
			ResetEvaluators(activationFunction);
		}

		#endregion

		#region IExperiment Members

		public AbstractExperimentView CreateExperimentView()
		{
			return new SimpleOcrExperimentView(this);
		}

		public void ResetEvaluators(IActivationFunction activationFn)
		{
			networkListEvaluator = new SimpleOcrNetworkEvaluator('a','z');
			genomeListEvaluator =  new DeterministicSingleGenomeEvaluator(networkListEvaluator, activationFn);
		}

		public IGenomeListEvaluator GenomeListEvaluator
		{
			get
			{
				return genomeListEvaluator;
			}
		}

		public SharpNeatLib.NetworkEvaluators.INetworkListEvaluator NetworkListEvaluator
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

		public string ExplanatoryText
		{
			get
			{
				return @"A simple OCR or pattern classification problem. 

There are 26 patterns, each representing one of the 26 lower case letters of the alphabet. 
Each pattern is is a bitmap (1 bit-plane image) of size 7 pixels wide by 9 high. There are therefore 7*9=63 pixels
in each image, each of which is associated with its own input neuron. An output neuron is defined for each character
(so 26 outputs in total) and the task for the evolved networks is to set the output high that corresponds to the character
image being applied to the input nodes.

Because of the large input/output space it is recommended that searches start with either no connections or a very small
proportion of input/output interconnections, e.g. 0.01 seems to work well. Also, the search is largely one of connections 
rather than connection weights, therefore the default search parameters use a higher 'add connection' rate than normal.";			
			}
		}

		public NeatParameters DefaultNeatParameters
		{
			get
			{
				return new NeatParameters();
			}
		}

		#endregion
	}
}
