using System;
using System.Collections;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	public class ParetoDominanceCoEvTicTacToeExperiment : IExperiment
	{
		IPopulationEvaluator populationEvaluator;
		IActivationFunction activationFunction = new SteepenedSigmoid();

		#region Constructor

		public ParetoDominanceCoEvTicTacToeExperiment()
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
			populationEvaluator = new ParetoDominanceCoEvTicTacToePopulationEvaluator(activationFn);
		}

		public int InputNeuronCount
		{
			get
			{
				return 9;
			}
		}

		public int OutputNeuronCount
		{
			get
			{
				return 9;
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
				return @"Evolution of Tic-Tac-Toe players using a form of co-evolution whereby 
evaluation is performed against a 'chain' of dominant players. Pure co-evolution is performed in 
the first generation (every genome plays every other genome), the generation champ (or one of them)
becomes the first entry on the chain (we call this the dominace 'front').

In subseqent generations evaluation is againt each genome/player on the chain. If a new genome 
scores higher against all of the players on teh chain then it becomes a new entry onto the chain.
There is a maximum of 1 new entry per generation onto the chain, so if multiple genomes are candidates
then one is picked arbitrarily (this could be improved).";
			}
		}

		#endregion
	}
}
