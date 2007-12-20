using System;
using System.Collections;

using SharpNeatLib;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Maths;

namespace SharpNeatLib.NeatGenome
{
	public class GenomeFactory
	{
		/// <summary>
		/// Create a default minimal genome that describes a NN with the given number of inputs and outputs.
		/// </summary>
		/// <returns></returns>
		public static IGenome CreateGenome(NeatParameters neatParameters, IdGenerator idGenerator, int inputNeuronCount, int outputNeuronCount, float connectionProportion)
		{
			NeuronGene neuronGene; // temp variable.
			NeuronGeneList inputNeuronGeneList = new NeuronGeneList(); // includes bias neuron.
			NeuronGeneList outputNeuronGeneList = new NeuronGeneList();
			NeuronGeneList neuronGeneList = new NeuronGeneList();
			ConnectionGeneList connectionGeneList = new ConnectionGeneList();

			// IMPORTANT NOTE: The neurons must all be created prior to any connections. That way all of the genomes
			// will obtain the same innovation ID's for the bias,input and output nodes in the initial population.
			// Create a single bias neuron.
			neuronGene = new NeuronGene(idGenerator.NextInnovationId, NeuronType.Bias);
			inputNeuronGeneList.Add(neuronGene);
			neuronGeneList.Add(neuronGene);

			// Create input neuron genes.
			for(int i=0; i<inputNeuronCount; i++)
			{
				neuronGene = new NeuronGene(idGenerator.NextInnovationId, NeuronType.Input);
				inputNeuronGeneList.Add(neuronGene);
				neuronGeneList.Add(neuronGene);
			}

			// Create output neuron genes. 
			for(int i=0; i<outputNeuronCount; i++)
			{
				neuronGene = new NeuronGene(idGenerator.NextInnovationId, NeuronType.Output);
				outputNeuronGeneList.Add(neuronGene);
				neuronGeneList.Add(neuronGene);
			}

			// Loop over all possible connections from input to output nodes and create a number of connections based upon
			// connectionProportion.
			foreach(NeuronGene targetNeuronGene in outputNeuronGeneList)
			{
				foreach(NeuronGene sourceNeuronGene in inputNeuronGeneList)
				{
					// Always generate an ID even if we aren't going to use it. This is necessary to ensure connections
					// between the same neurons always have the same ID throughout the generated population.
					uint connectionInnovationId = idGenerator.NextInnovationId;

					if(Utilities.NextDouble() < connectionProportion)
					{	// Ok lets create a connection.
						connectionGeneList.Add(	new ConnectionGene(connectionInnovationId, 
							sourceNeuronGene.InnovationId,
							targetNeuronGene.InnovationId,
							(Utilities.NextDouble() * neatParameters.connectionWeightRange ) - neatParameters.connectionWeightRange/2.0));  // Weight 0 +-5
					}
				}
			}

			// Don't create any hidden nodes at this point. Fundamental to the NEAT way is to start minimally!
			return new NeatGenome(idGenerator.NextGenomeId, neuronGeneList, connectionGeneList, inputNeuronCount, outputNeuronCount);
		}

		/// <summary>
		/// Construct a GenomeList. This can be used to construct a new Population object.
		/// </summary>
		/// <param name="evolutionAlgorithm"></param>
		/// <param name="inputNeuronCount"></param>
		/// <param name="outputNeuronCount"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static GenomeList CreateGenomeList(NeatParameters neatParameters, IdGenerator idGenerator, int inputNeuronCount, int outputNeuronCount, float connectionProportion, int length)
		{
			GenomeList genomeList = new GenomeList();
			
			for(int i=0; i<length; i++)
			{
				idGenerator.ResetNextInnovationNumber();
				genomeList.Add(CreateGenome(neatParameters, idGenerator, inputNeuronCount, outputNeuronCount, connectionProportion));
			}

			return genomeList;
		}

		public static GenomeList CreateGenomeList(NeatGenome seedGenome, int length, NeatParameters neatParameters, IdGenerator idGenerator)
		{
			//Build the list.
			GenomeList genomeList = new GenomeList();
			
			// Use the seed directly just once.
			NeatGenome newGenome = new NeatGenome(seedGenome, idGenerator.NextGenomeId);
			genomeList.Add(newGenome);

			// For the remainder we alter the weights.
			for(int i=1; i<length; i++)
			{
				newGenome = new NeatGenome(seedGenome, idGenerator.NextGenomeId);
				
				// Reset the connection weights
				foreach(ConnectionGene connectionGene in newGenome.ConnectionGeneList)
					connectionGene.Weight = (Utilities.NextDouble() * neatParameters.connectionWeightRange) - neatParameters.connectionWeightRange/2.0;

				genomeList.Add(newGenome);
			}

			return genomeList;
		}


		public static GenomeList CreateGenomeList(Population seedPopulation, int length, NeatParameters neatParameters, IdGenerator idGenerator)
		{
			//Build the list.
			GenomeList genomeList = new GenomeList();
			int seedIdx=0;
			
			for(int i=0; i<length; i++)
			{
				NeatGenome newGenome = new NeatGenome((NeatGenome)seedPopulation.GenomeList[seedIdx], idGenerator.NextGenomeId);

				// Reset the connection weights
				foreach(ConnectionGene connectionGene in newGenome.ConnectionGeneList)
					connectionGene.Weight = (Utilities.NextDouble() * neatParameters.connectionWeightRange) - neatParameters.connectionWeightRange/2.0;

				genomeList.Add(newGenome);

				if(++seedIdx >= seedPopulation.GenomeList.Count)
				{	// Back to first genome.
					seedIdx=0;
				}
			}
			return genomeList;
		}
	}
}
