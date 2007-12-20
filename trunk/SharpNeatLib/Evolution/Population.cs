using System;
using System.Collections;

namespace SharpNeatLib.Evolution
{
	public class Population
	{
		IdGenerator idGenerator;
		GenomeList genomeList;		// The master list of genomes in the population.
		Hashtable speciesTable;			// Asecondary structure containing all of the genomes partitioned into their respective species. A Hashtable of GenomeList structures.

		int populationSize;		// The base-line number for the population size. The actual size may vary slightly from this figure as offspring are generated and culled.
		double totalFitness;	// totalled fitness values of all genomes in the population.
		double meanFitness;
		double totalSpeciesMeanFitness;

		// The totalled fitness of the genomes that will be selected from.
		double selectionTotalFitness;

		int totalNeuronCount;
		int totalConnectionCount;
		int totalStructureCount;
		float avgComplexity;

		int nextSpeciesId=0;

		// Some statistics.
		long generationAtLastImprovement=0;
		double maxFitnessEver = 0.0;
//		double fitnessAtLastPrunePhaseEnd=0.0;

		float prunePhaseAvgComplexityThreshold=-1;

		#region Constructor

		public Population(IdGenerator idGenerator, GenomeList genomeList)
		{
			this.idGenerator = idGenerator;
			this.genomeList = genomeList;
			this.populationSize = genomeList.Count;
		}

		#endregion

		#region Properties

		public IdGenerator IdGenerator
		{
			get
			{
				return idGenerator;
			}
		}

		/// <summary>
		/// The base-line number for the population size. The actual size may vary slightly from this figure as offspring are generated and culled.
		/// </summary>
		public int PopulationSize
		{
			get
			{
				return populationSize;
			}
		}

		public GenomeList GenomeList
		{
			get
			{
				return genomeList;
			}
		}

		public Hashtable SpeciesTable
		{
			get
			{
				return speciesTable;
			}
		}

		public double TotalFitness
		{
			get
			{
				return totalFitness;
			}
			set
			{
				totalFitness = value;
			}
		}

		public double MeanFitness
		{
			get
			{
				return meanFitness;
			}
			set
			{
				meanFitness = value;
			}
		}

		/// <summary>
		/// The total of all of the Species.MeanFitness
		/// </summary>
		public double TotalSpeciesMeanFitness
		{
			get
			{
				return totalSpeciesMeanFitness;
			}
			set
			{
				totalSpeciesMeanFitness = value;
			}
		}

		/// <summary>
		/// The total of all of the Species.MeanFitness
		/// </summary>
		public double SelectionTotalFitness
		{
			get
			{
				return selectionTotalFitness;
			}
			set
			{
				selectionTotalFitness = value;
			}
		}

		public int TotalNeuronCount
		{
			get
			{
				return totalNeuronCount;
			}
			set
			{
				totalNeuronCount = value;
			}
		}

		public int TotalConnectionCount
		{
			get
			{
				return totalConnectionCount;
			}
			set
			{
				totalConnectionCount = value;
			}
		}

		/// <summary>
		/// TotalNeuronCount + TotalConnectionCount
		/// </summary>
		public int TotalStructureCount
		{
			get
			{
				return totalStructureCount;
			}
			set
			{
				totalStructureCount = value;
			}
		}

		/// <summary>
		/// Avg Structures Per Genome.
		/// </summary>
		public float AvgComplexity
		{
			get
			{
				return avgComplexity;
			}
			set
			{
				avgComplexity = value;
			}
		}

		public long GenerationAtLastImprovement
		{
			get
			{
				return generationAtLastImprovement;
			}
			set
			{
				generationAtLastImprovement = value;
			}
		}

//		public long GenerationAtLastPrunePhaseEnd
//		{
//			get
//			{
//				return generationAtLastPrunePhaseEnd;
//			}
//			set
//			{
//				generationAtLastPrunePhaseEnd = value;
//			}
//		}

		public double MaxFitnessEver
		{
			get
			{
				return maxFitnessEver;
			}
			set
			{
				maxFitnessEver = value;
			}
		}
	
//		public double FitnessAtLastPrunePhaseEnd
//		{
//			get
//			{
//				return fitnessAtLastPrunePhaseEnd;
//			}
//			set
//			{
//				fitnessAtLastPrunePhaseEnd = value;
//			}
//		}

		public float PrunePhaseAvgComplexityThreshold
		{
			get
			{
				return prunePhaseAvgComplexityThreshold;
			}
			set
			{
				prunePhaseAvgComplexityThreshold = value;
			}
		}

		#endregion

		#region Public Methods

		public void ResetFitnessValues()
		{
			totalFitness = 0.0;
			meanFitness = 0.0;
			totalSpeciesMeanFitness = 0.0;
			selectionTotalFitness = 0.0;
		}

		public void AddGenomeToPopulation(EvolutionAlgorithm ea, IGenome genome)
		{
			//----- Add genome to the master list of genomes.
			genomeList.Add(genome);

			//----- Determine it's species and insert into the speciestable.
			AddGenomeToSpeciesTable(ea, genome);
		}

		/// <summary>
		/// Determine the species of each genome in genomeList and build the 'species' Hashtable.
		/// </summary>
		public void BuildSpeciesTable(EvolutionAlgorithm ea)
		{
			//----- Build the table.
			speciesTable = new Hashtable();

			// First pass. Genomes that already have an assigned species.

			//foreach(IGenome genome in genomeList)
			int genomeIdx;
			int genomeBound = genomeList.Count;
			for(genomeIdx=0; genomeIdx<genomeBound; genomeIdx++)
			{
				IGenome genome = genomeList[genomeIdx];
				if(genome.SpeciesId!=-1)
					AddGenomeToSpeciesTable(ea, genome);
			}
			
			// Second pass. New genomes. Performing two passes ensures we preserve the species IDs.
			for(genomeIdx=0; genomeIdx<genomeBound; genomeIdx++)
			{
				IGenome genome = genomeList[genomeIdx];
				if(genome.SpeciesId==-1)
					AddGenomeToSpeciesTable(ea, genome);
			}
		}

		public void RedetermineSpeciation(EvolutionAlgorithm ea)
		{
			// Keep a reference to the old species table.
			Hashtable oldSpeciesTable = speciesTable;

			// Remove the gnomes from the old species objects. Note that the genome's can still be 
			// accessed via 'genomeList' and that they still contain the old speciesId.
			foreach(Species species in oldSpeciesTable.Values)
				species.Members.Clear();

			// Create a new species table.
			speciesTable = new Hashtable();

			// Loop through all of the genomes and place them into the new species table.
			// Use the overload for AddGenomeToSpeciesTable() that re-uses the old species
			// objects instead of creating new species.
			int genomeBound = genomeList.Count;
			for(int genomeIdx=0; genomeIdx<genomeBound; genomeIdx++)
			{
				IGenome genome = genomeList[genomeIdx];
				Species oldSpecies = (Species)oldSpeciesTable[genome.SpeciesId];
				AddGenomeToSpeciesTable(ea, genome, oldSpecies);
			}

            




//			speciesTable.Clear();
//			
//			int genomeBound = genomeList.Count;
//			for(int genomeIdx=0; genomeIdx<genomeBound; genomeIdx++)
//			{
//				IGenome genome = genomeList[genomeIdx];
//
//				genome.SpeciesId = -1;
//				genome.ParentSpeciesId1=-1;
//				genome.ParentSpeciesId2=-1;
//				AddGenomeToSpeciesTable(ea, genome);
//			}
		}


//		public void RedetermineSpeciation(EvolutionAlgorithm ea)
//		{
//			speciesTable.Clear();
//			
//			int genomeBound = genomeList.Count;
//			for(int genomeIdx=0; genomeIdx<genomeBound; genomeIdx++)
//			{
//				IGenome genome = genomeList[genomeIdx];
//
//				genome.SpeciesId = -1;
//				genome.ParentSpeciesId1=-1;
//				genome.ParentSpeciesId2=-1;
//				AddGenomeToSpeciesTable(ea, genome);
//			}
//		}

		ArrayList speciesToRemove = new ArrayList();
		public bool EliminateSpeciesWithZeroTargetSize()
		{
			// Ensure helper table is empty before we start.
			speciesToRemove.Clear();

			// Store a reference to all species that we need to remove. We cannot remove right 
			// away as we would be modifying the structrue we are looping through.
			foreach(Species species in speciesTable.Values)
			{
				if(species.TargetSize==0)
					speciesToRemove.Add(species.SpeciesId);
			}

			// Remove the poor species.
			int bound=speciesToRemove.Count;
			//foreach(int speciesId in speciesToRemove)
			for(int i=0; i<bound; i++)
				speciesTable.Remove((int)speciesToRemove[i]);
			
			// If species were removed then rebuild the master GenomeList.
			bool bSpeciesRemoved;
			if(speciesToRemove.Count>0)
			{
				bSpeciesRemoved = true;
				RebuildGenomeList();
			}
			else
			{
				bSpeciesRemoved = false;
			}

			if(bSpeciesRemoved)
				speciesToRemove.Clear();

			return bSpeciesRemoved;
		}

		public void TrimAllSpeciesBackToElite()
		{
			speciesToRemove.Clear();
			foreach(Species species in speciesTable.Values)
			{
				if(species.ElitistSize==0)
				{	// Remove the entire species.
					speciesToRemove.Add(species.SpeciesId);
				}
				else
				{	// Remove genomes from the species.
					int delta = species.Members.Count - species.ElitistSize;
					species.Members.RemoveRange(species.ElitistSize, delta);
				}
			}
			//foreach(int speciesId in speciesToRemove)
			int speciesBound=speciesToRemove.Count;
			for(int speciesIdx=0; speciesIdx<speciesBound; speciesIdx++)
				speciesTable.Remove(speciesToRemove[speciesIdx]);

			RebuildGenomeList();
		}

		/// <summary>
		/// Rebuild GenomeList from the genomes held in the speciesTable.
		/// Quite useful to keep the list up-to-date after a species has been deleted.
		/// </summary>
		public void RebuildGenomeList()
		{
			genomeList.Clear();
			foreach(Species species in speciesTable.Values)
			{
				//foreach(IGenome genome in species.Members)
				int genomeBound = species.Members.Count;
				for(int genomeIdx=0; genomeIdx<genomeBound; genomeIdx++)
					genomeList.Add(species.Members[genomeIdx]);
			}
		}

		/// <summary>
		/// Some(most) types of network have fixed numbers of input and output nodes and will not work correctly or
		/// throw an exception if we try and use inputs/outputs that do not exist. This method allows us to check
		/// compatibility of the current populations genomes before we try to use them.
		/// </summary>
		/// <param name="inputCount"></param>
		/// <param name="outputCount"></param>
		/// <returns></returns>
		public bool IsCompatibleWithNetwork(int inputCount, int outputCount)
		{
			foreach(IGenome genome in genomeList)
			{
				if(!genome.IsCompatibleWithNetwork(inputCount, outputCount))
					return false;
			}
			return true;
		}

		public void IncrementGenomeAges()
		{
			int genomeBound = genomeList.Count;
			for(int genomeIdx=0; genomeIdx<genomeBound; genomeIdx++)
				genomeList[genomeIdx].GenomeAge++;
		}

		public void IncrementSpeciesAges()
		{
			foreach(Species species in speciesTable.Values)
				species.SpeciesAge++;
		}

		/// <summary>
		/// For debug purposes only.
		/// </summary>
		/// <returns>Returns true if population integrity checks out OK.</returns>
		public bool PerformIntegrityCheck()
		{
			foreach(IGenome genome in genomeList)
			{
				if(!genome.PerformIntegrityCheck())
					return false;
			}
			return true;
		}

		#endregion

		#region Private Methods

		private void AddGenomeToSpeciesTable(EvolutionAlgorithm ea, IGenome genome)
		{
			Species species = DetermineSpecies(ea, genome);
			if(species==null)	
			{					
				species = new Species();

				// Completely new species. Generate a speciesID.
				species.SpeciesId = nextSpeciesId++;
				speciesTable.Add(species.SpeciesId, species);
			}

			//----- The genome is a member of an existing species.
			genome.SpeciesId = species.SpeciesId;
			species.Members.Add(genome);
		}

		/// <summary>
		/// This version of AddGenomeToSpeciesTable is used by RedetermineSpeciation(). It allows us to
		/// pass in the genome's original species object, which we can then re-use if the genome does not
		/// match any of our existing species and needs to be placed into a new species of it's own.
		/// The old species object can be used directly because it should already have already had all of 
		/// its genome sremoved by RedetermineSpeciation() before being passed in here.
		/// </summary>
		/// <param name="ea"></param>
		/// <param name="genome"></param>
		/// <param name="originalSpecies"></param>
		private void AddGenomeToSpeciesTable(EvolutionAlgorithm ea, IGenome genome, Species originalSpecies)
		{
			Species species = DetermineSpecies(ea, genome);
			if(species==null)	
			{	
				// The genome is not in one of the existing (new) species. Is this genome's old
				// species already in the new table?
				species = (Species)speciesTable[genome.SpeciesId];
				if(species!=null)
				{
					// The genomes old species is already in the table but the genome no longer fits into that 
					// species. Therefore we need to create an entirely new species.
					species = new Species();
					species.SpeciesId = nextSpeciesId++;		
				}
				else
				{
					// We can re-use the oldSpecies object.
					species = originalSpecies;
				}
				speciesTable.Add(species.SpeciesId, species);
			}

			//----- The genome is a member of an existing species.
			genome.SpeciesId = species.SpeciesId;
			species.Members.Add(genome);
		}



		/// <summary>
		/// Determine the given genome's species and return that species. If the genome does not 
		/// match one of the existing species then we return null to indicate a new species.
		/// </summary>
		/// <param name="genome"></param>
		/// <returns></returns>
		private Species DetermineSpecies(EvolutionAlgorithm ea, IGenome genome)
		{
			//----- Performance optimization. Check against parent species IDs first.
			Species parentSpecies1 = null;
			Species parentSpecies2 = null;

			// Parent1. Not set in initial population.
			if(genome.ParentSpeciesId1!=-1)
			{
				parentSpecies1 = (Species)speciesTable[genome.ParentSpeciesId1];
				if(parentSpecies1!=null)
				{
					if(IsGenomeInSpecies(genome, parentSpecies1, ea))
						return parentSpecies1;
				}
			}
				
			// Parent2. Not set if result of asexual reproduction.
			if(genome.ParentSpeciesId2!=-1)
			{
				parentSpecies2 = (Species)speciesTable[genome.ParentSpeciesId2];
				if(parentSpecies2!=null)
				{
					if(IsGenomeInSpecies(genome, parentSpecies2, ea))
						return parentSpecies2;
				}
			}

			//----- Not in parent species. Systematically compare against all species.
			foreach(Species compareWithSpecies in speciesTable.Values)
			{
				// Don't compare against the parent species again.
				if(compareWithSpecies==parentSpecies1 || compareWithSpecies == parentSpecies2)
					continue;

				if(IsGenomeInSpecies(genome, compareWithSpecies, ea))
				{	// We have found matching species.
					return compareWithSpecies;		
				}
			}

			//----- The genome is not a member of any existing species.
			return null;
		}

		private bool IsGenomeInSpecies(IGenome genome, Species compareWithSpecies, EvolutionAlgorithm ea)
		{
//			// Pick a member of the species at random.
//			IGenome compareWithGenome = compareWithSpecies.Members[(int)Math.Floor(compareWithSpecies.Members.Count * Utilities.NextDouble())];
//			return (genome.CalculateCompatibility(compareWithGenome, ea.NeatParameters) < ea.NeatParameters.compatibilityThreshold);

			// Compare against the species champ. The species champ is the exemplar that represents the species.
			IGenome compareWithGenome = compareWithSpecies.Members[0];
			//IGenome compareWithGenome = compareWithSpecies.Members[(int)Math.Floor(compareWithSpecies.Members.Count * Utilities.NextDouble())];
			return genome.IsCompatibleWithGenome(compareWithGenome, ea.NeatParameters);
		}

		#endregion
	}
}
