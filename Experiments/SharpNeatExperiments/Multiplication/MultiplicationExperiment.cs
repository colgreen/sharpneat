using System;
using System.Collections;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class MultiplicationExperiment : IExperiment
	{
		IPopulationEvaluator populationEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoid();

		#region Constructor

		public MultiplicationExperiment()
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
			populationEvaluator = new SingleFilePopulationEvaluator(new MultiplicationNetworkEvaluator(), activationFn);
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
				return @"Multiplication of fractional operands between -1.0 and 1.0.
Multiplication over such a range is a sub-task of the vector cross product task.
This experiment demonstrates that evolving a network to perform multiplication 
is a non-trivial task, at least when using a sigmoid activation function.

Fitness is evaluated by multiplying every combination of the values in the set
-1.0, -0.9 ... 0.9, 1.0

The set contains 21 values and there are therefore 21^2=441 multiplications performed
per fitness evaluation. The correct answer for each mult is >=-1.0 and <=1.0, therefore
the standard sigmoid output range of 0.0 .. 1.0 is mapped to the range -1.0 .. 1.0 to 
obtain an answer.

Each network response can therefore be wrong by a maximum of 2.0. Fitness for each mult 
is calculated as follows. Take the absolute difference between the network's answer and
the correct answer and raise to the power 0.3. Raising to a fractional power introduces a
steeper incline in the fitness landscape around correct answers. This value is then capped
at 1.0 and subtracted from 1.0. Thus the max fitness per mult is 1.0 and the overall max 
fitness per evaluation is 441.0.
";
			}
		}

		#endregion
	}
}
