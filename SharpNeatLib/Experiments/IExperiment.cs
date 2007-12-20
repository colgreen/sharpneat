using System;
using System.Collections;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public interface IExperiment
	{
		/// <summary>
		/// This method is called immediately following instantiation of an experiment. It is used
		/// to pass in a hashtable of string key-value pairs from the 'experimentParameters' 
		/// block of the experiment configuration block within the application config file.
		/// 
		/// If no parameters where specified then an empty Hashtable is used.
		/// </summary>
		/// <param name="parameterTable"></param>
		void LoadExperimentParameters(Hashtable parameterTable);

		/// <summary>
		/// The IPopulationEvaluator to use for the experiment. This is passed to the 
		/// constructor of EvolutionAlgorithm.
		/// </summary>
		IPopulationEvaluator PopulationEvaluator
		{
			get;
		}

		/// <summary>
		/// This is called prior to constructing a new EvolutionAlgorithm to ensure we have a 
		/// fresh evaluator - some evaluators have state.
		/// </summary>
		/// <param name="activationFn"></param>
		void ResetEvaluator(IActivationFunction activationFn);

		/// <summary>
		/// The number of input neurons required for an experiment. This figure is used
		/// to generate a population of genomes with the correct number of inputs.
		/// </summary>
		int InputNeuronCount
		{
			get;
		}

		/// <summary>
		/// The number of output neurons required for an experiment. This figure is used
		/// to generate a population of genomes with the correct number of outputs.
		/// </summary>
		int OutputNeuronCount
		{
			get;
		}

		/// <summary>
		/// The default NeatParameters object to use for the experiment.
		/// </summary>
		NeatParameters DefaultNeatParameters
		{
			get;
		}

		/// <summary>
		/// This is the suggested netowkr activation function for an experiment. The default
		/// activation function is shown within SharpNEAT's GUI and can be overriden by 
		/// selecting an alternative function in the drop-down combobox.
		/// </summary>
		IActivationFunction SuggestedActivationFunction
		{
			get;
		}

		/// <summary>
		/// Returns a Form based view of the experiment. It is accetable to return null to
		/// indicate that no view is available.
		/// </summary>
		/// <returns></returns>
		AbstractExperimentView CreateExperimentView();

		/// <summary>
		/// A description of the evaluator and domain to aid new users.
		/// </summary>
		string ExplanatoryText
		{
			get;
		}
	}
}
