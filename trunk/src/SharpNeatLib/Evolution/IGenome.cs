using System;
using System.Xml;

using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Evolution
{
	/// <summary>
	/// An interface for describing a generic genome.
	/// IComparable must be implemented in contravention of the docs. So that ArrayList.Sort() will sort into descending order.
	/// This interface may be discarded since the development of SharpNEAT has seen the EvolutionAlgorithm become more
	/// closely coupled with the NeatGenome, thus making this interfaces abstraction unmaintainable.
	/// </summary>
	public interface IGenome : IComparable
	{
		/// <summary>
		/// Some(most) types of network have fixed numbers of input and output nodes and will not work correctly or
		/// throw an exception if we try and use inputs/outputs that do not exist. This method allows us to check
		/// compatibility before we begin.
		/// </summary>
		/// <param name="inputCount"></param>
		/// <param name="outputCount"></param>
		/// <returns></returns>
		bool IsCompatibleWithNetwork(int inputCount, int outputCount);

		/// <summary>
		/// Asexual reproduction with built in mutation.
		/// </summary>
		/// <returns></returns>
		IGenome CreateOffspring_Asexual(EvolutionAlgorithm ea);

		/// <summary>
		/// Sexual reproduction. No mutation performed.
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		IGenome CreateOffspring_Sexual(EvolutionAlgorithm ea, IGenome parent);

		/// <summary>
		/// The globally unique ID for this genome (within the context of a search).
		/// </summary>
		uint GenomeId
		{
			get;
		}

		/// <summary>
		/// The number of generations that this genome has existed. Note that to 
		/// survive a generation a genome must be one of the elite that are preserved
		/// between generations.
		/// </summary>
		long GenomeAge
		{
			get;
			set;
		}

		/// <summary>
		/// This genome's fitness as calculated by the evaluation environment.
		/// </summary>
		double Fitness
		{
			get;
			set;
		}

		/// <summary>
		/// The number of times this genome has been evaluated.
		/// </summary>
		long EvaluationCount
		{
			get;
			set;
		}

		/// <summary>
		/// Returns the total of all fitness scores if this genome has been evaluated more than once.
		/// Average fitness is therefore this figure divided by EvaluationCount.
		/// </summary>
		double TotalFitness
		{
			get;
			set;
		}

		/// <summary>
		/// The species this genome is within.
		/// </summary>
		int SpeciesId
		{
			get;
			set;
		}

		/// <summary>
		/// The ID of this genome's first parent.
		/// </summary>
		int ParentSpeciesId1
		{
			get;
			set;
		}

		/// <summary>
		/// The ID of this genome's second parent. -1 if no second parent.
		/// </summary>
		int ParentSpeciesId2
		{
			get;
			set;
		}

		AbstractNetwork AbstractNetwork
		{
			get;
		}

		/// <summary>
		/// An object reference that can be used by IPopulationEvaluator objects to
		/// store evaluation state information against a genome. E.g. If we have a growing 
		/// list of test cases as evolution progresses then we could store the index of the
		/// last test case to be evaluated against. We can then skip over these test cases
		/// in subsequent evaluations of this genome.
		/// </summary>
		object Tag
		{
			get;
			set;
		}

		/// <summary>
		/// Decode the genome's 'DNA' into a working network.
		/// </summary>
		/// <returns></returns>
		INetwork Decode(IActivationFunction activationFn);

		/// <summary>
		/// Clone this genome.
		/// </summary>
		/// <returns></returns>
		IGenome Clone(EvolutionAlgorithm ea);

		/// <summary>
		/// Compare this IGenome with the provided one. They are compatible (determined to be in 
		/// the same species) if their calculated difference is below the current threshold specified
		/// by NeatParameters.compatibilityThreshold
		/// </summary>
		/// <param name="comparisonGenome"></param>
		/// <param name="neatParameters"></param>
		/// <returns></returns>
		bool IsCompatibleWithGenome(IGenome comparisonGenome, NeatParameters neatParameters);

		/// <summary>
		/// Used primarily to give this IGenome a hook onto the Population it is within.
		/// </summary>
		Population OwningPopulation
		{
			get;
			set;
		}

		/// <summary>
		/// Persist to XML.
		/// </summary>
		/// <param name="parentNode"></param>
		void Write(XmlNode parentNode);


		/// <summary>
		/// For debug purposes only.
		/// </summary>
		/// <returns>Returns true if genome integrity checks out OK.</returns>
		bool PerformIntegrityCheck();
	}
}
