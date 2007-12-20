using System;
using System.Collections;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	/// <summary>
	/// The Max experiment searches for an ANN that accepts a variable number of input signals (defined within the 
	/// application configuration file) and that indicates which signal is the largest value with a single output value.
	/// E.g. if there are two inputs then the output will be between 0 and 0.5 if input 0 (the first) is highest.
	/// 
	/// This experiment was submitted by Ricardo J. Mendez (RJM from here on) and has been updated/modified by Colin Green. 
	/// This experiment was the first to use data from the application configuration system built into
	/// .Net and extended/customised for SharpNEAT by RJM. Thus, this experiment has a configurable number of inputs
	/// and also reads test data from a data file that has a configurable path.
	/// 
	/// A score of 1.0 is assigned for every correct response and 0.0 for an incorrect response. Thus the maximum fitness
	/// is equal to the number of rows in the test data.
	/// 
	/// An example configuration XML block (with anngled brackets replaced by square brackets so we can include this 
	/// within a code comment XML block!) is as follows:
	/// 
	///	[experiment]
	///		[title]Maximizer[/title]
	///		[description]Sample maximizer experiment[/description]
	///		[assemblyUrl]MaxExperiment.dll[/assemblyUrl]
	///		[typeName]SharpNeatLib.Experiments.MaxExperiment[/typeName]
	///		[experimentParameters]
	///			[inputNeuronCount]3[/inputNeuronCount]
	///			[dataFileUrl]c:\projects\SharpNEAT\src\Experiments\MaxExperiment\testdata_3cols_10values.txt[/dataFileUrl]
	///		[/experimentParameters]
	///	[/experiment]
	/// 
	/// Note. The inputNeuronCount value must correspond to the number of data columns within the data file. 
	/// The data format for a row is as follows:
	/// 
	/// 0.1234 0.5678\r\n
	/// 
	/// Values are seperated by a single space, there is no space at the end of a line, lines are terminated by a 
	/// \r\n and there should be no blank lines beyond the last (terminating) line, except a \r\n is allowed on 
	/// the very last row of data. 
	/// 
	/// Value ranges are not strictly defined but should be within the range acceptable by the neurons in use, therefore
	/// the test data is within the range 0.0 to 1.0 inclusive.
	/// 
	/// At first this may seem to be a trivially simple problem domain, however, although this experiment finds a solution
	/// very quickly for two inputs, beyond this the problem becomes more difficult. This is mainly because values that are
	/// very close are hard to distinguish using only a standard activation function. This experiment is therefore a further 
	/// example of how extending the types of neuron activation function in use should improve the ability of NEAT to solve
	/// problems in general. E.g. in this experiment a comparator function would be useful.
	/// </summary>
	public class MaxExperiment : IExperiment
	{
		IPopulationEvaluator populationEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoid();

		int inputNeuronCount;
		string dataFileUrl;
		
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
			inputNeuronCount = int.Parse((string)parameterTable["inputNeuronCount"]);
			dataFileUrl = (string)parameterTable["dataFileUrl"];
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
			populationEvaluator = new SingleFilePopulationEvaluator(new MaxNetworkEvaluator(inputNeuronCount, dataFileUrl), activationFn);
		}

		public int InputNeuronCount
		{
			get
			{
				return inputNeuronCount;
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
				return @"The Max experiment searches for an ANN that accepts a variable number of input signals (defined within the 
application configuration file) and that indicates which signal is the largest value with a single output value.
E.g. if there are two inputs then the output will be between 0 and 0.5 if input 0 (the first) is highest.

This experiment was submitted by Ricardo J. Mendez (RJM from here on) and has been updated/modified by Colin Green. 
This experiment was the first to use data from the application configuration system built into
.Net and extended/customised for SharpNEAT by RJM. Thus, this experiment has a configurable number of inputs
and also reads test data from a data file that has a configurable path.

A score of 1.0 is assigned for every correct response and 0.0 for an incorrect response. Thus the maximum fitness
is equal to the number of rows in the test data.

An example configuration XML block (with anngled brackets replaced by square brackets so we can include this 
within a code comment XML block!) is as follows:

[experiment]
	[title]Maximizer[/title]
	[description]Sample maximizer experiment[/description]
	[assemblyUrl]MaxExperiment.dll[/assemblyUrl]
	[typeName]SharpNeatLib.Experiments.MaxExperiment[/typeName]
	[experimentParameters]
		[inputNeuronCount]3[/inputNeuronCount]
		[dataFileUrl]c:\projects\SharpNEAT\src\Experiments\MaxExperiment\testdata_3cols_10values.txt[/dataFileUrl]
	[/experimentParameters]
[/experiment]

Note. The inputNeuronCount value must correspond to the number of data columns within the data file. 
The data format for a row is as follows:

0.1234 0.5678\r\n

Values are seperated by a single space, there is no space at the end of a line, lines are terminated by a 
\r\n and there should be no blank lines beyond the last (terminating) line, except a \r\n is allowed on 
the very last row of data. 

Value ranges are not strictly defined but should be within the range acceptable by the neurons in use, therefore
the test data is within the range 0.0 to 1.0 inclusive.

At first this may seem to be a trivially simple problem domain, however, although this experiment finds a solution
very quickly for two inputs, beyond this the problem becomes more difficult. This is mainly because values that are
very close are hard to distinguish using only a standard activation function. This experiment is therefore a further 
example of how extending the types of neuron activation function in use should improve the ability of NEAT to solve
problems in general. E.g. in this experiment a comparator function would be useful.
";
			}
		}

		#endregion
	}
}
