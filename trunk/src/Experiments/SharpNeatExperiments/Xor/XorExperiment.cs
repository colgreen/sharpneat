using System;
using System.Collections;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class XorExperiment : IExperiment
	{
		IPopulationEvaluator populationEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoid();

		#region Constructor

		public XorExperiment()
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
			populationEvaluator = new SingleFilePopulationEvaluator(new XorNetworkEvaluator(), activationFn);
		}

		public int InputNeuronCount
		{
			get
			{
				return 2;
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
				np.pOffspringAsexual = 0.8;
				np.pOffspringSexual = 0.2;

				np.pMutateConnectionWeights = 0.5;
				np.pMutateAddNode = 0.03;
				np.pMutateAddConnection = 0.5;

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
				return @"Logical XOR. The goal is to reproduce the following logic truth table:
In1 | In2 | Out
-----------------
0    | 0    | 0
0    | 1    | 1
1    | 0    | 1
1    | 1    | 0

Each test case is tested in turn. An output less than 0.5 is interpreted as a 0(false) response, >= is interprested as a 1(true) response.
Evaluation terminates early if a network fails to relax(settle on an output value) within 10 timesteps.
A fitness of 1.0 is assigned for each correct test case, this is on a linear sliding scale that assigns 0 for a completey wrong response, e.g. 1.0 when 0.0 was expected.

An additional fitness of 10 is assigned if all four test cases are passed. Thus the maximum fitness is 14, but any score >=10 indicates
a sucessful network.";
			}
		}

		#endregion
	}
}
