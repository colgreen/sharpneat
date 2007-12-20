using System;
using System.Collections;

using SharpNeatLib.Evolution;

namespace SharpNeatLib.NeatGenome
{
	public class IdGeneratorFactory : IIdGeneratorFactory
	{
		/// <summary>
		/// Create an IdGeneratoy by interrogating the provided population of Genomes.
		/// This routine also fixes any duplicate IDs that are found in the
		/// population.
		/// </summary>
		/// <param name="pop"></param>
		/// <returns></returns>
		public IdGenerator CreateIdGenerator(GenomeList genomeList)
		{
			uint maxGenomeId=0;
			uint maxInnovationId=0;

			// First pass: Determine the current maximum genomeId and innovationId.
			foreach(NeatGenome genome in genomeList)
			{
				if(genome.GenomeId > maxGenomeId)
					maxGenomeId = genome.GenomeId;

				// Neuron IDs actualy come from the innovation IDs generator, so although they 
				// aren't used as historical markers we should count them as innovation IDs here.
				foreach(NeuronGene neuronGene in genome.NeuronGeneList)
				{
					if(neuronGene.InnovationId > maxInnovationId)
						maxInnovationId = neuronGene.InnovationId;
				}

				foreach(ConnectionGene connectionGene in genome.ConnectionGeneList)
				{
					if(connectionGene.InnovationId > maxInnovationId)
						maxInnovationId = connectionGene.InnovationId;
				}
			}

			if(maxGenomeId==uint.MaxValue)
			{	 //reset to zero.
				maxGenomeId=0;
			}
			else
			{	// Increment to next available ID.
				maxGenomeId++;
			}

			if(maxInnovationId==uint.MaxValue)
			{	 //reset to zero.
				maxInnovationId=0;
			}
			else
			{	// Increment to next available ID.
				maxInnovationId++;
			}

			// Create an IdGenerator using the discovered maximum IDs.
			IdGenerator idGenerator = new IdGenerator(maxGenomeId, maxInnovationId);

			// Second pass: Check for duplicate genome IDs.
			Hashtable genomeIdTable = new Hashtable();
			Hashtable innovationIdTable = new Hashtable();
			foreach(NeatGenome genome in genomeList)
			{
				if(genomeIdTable.Contains(genome.GenomeId))
				{	// Assign this genome a new Id.
					genome.GenomeId = idGenerator.NextGenomeId;
				}
				//Register the ID.
				genomeIdTable.Add(genome.GenomeId, null);
			}
						
			return idGenerator;
		}


		/// <summary>
		/// Create an IdGeneratoy by interrogating the provided Genome.
		/// </summary>
		/// <param name="pop"></param>
		/// <returns></returns>
		public IdGenerator CreateIdGenerator(NeatGenome genome)
		{
			uint maxGenomeId=0;
			uint maxInnovationId=0;

			// First pass: Determine the current maximum genomeId and innovationId.
			if(genome.GenomeId > maxGenomeId)
				maxGenomeId = genome.GenomeId;

			// Neuron IDs actualy come from the innovation IDs generator, so although they 
			// aren't used as historical markers we should count them as innovation IDs here.
			foreach(NeuronGene neuronGene in genome.NeuronGeneList)
			{
				if(neuronGene.InnovationId > maxInnovationId)
					maxInnovationId = neuronGene.InnovationId;
			}

			foreach(ConnectionGene connectionGene in genome.ConnectionGeneList)
			{
				if(connectionGene.InnovationId > maxInnovationId)
					maxInnovationId = connectionGene.InnovationId;
			}

			if(maxGenomeId==uint.MaxValue)
			{	 //reset to zero.
				maxGenomeId=0;
			}
			else
			{	// Increment to next available ID.
				maxGenomeId++;
			}

			if(maxInnovationId==uint.MaxValue)
			{	 //reset to zero.
				maxInnovationId=0;
			}
			else
			{	// Increment to next available ID.
				maxInnovationId++;
			}

			// Create an IdGenerator using the discovered maximum IDs.						
			return new IdGenerator(maxGenomeId, maxInnovationId);
		}
	}
}
