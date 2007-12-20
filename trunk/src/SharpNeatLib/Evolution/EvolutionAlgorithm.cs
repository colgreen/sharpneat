using System;
using System.Collections;
using System.Diagnostics;

//TODO: decouple from NeatGenome.
using SharpNeatLib.NeatGenome;

namespace SharpNeatLib.Evolution
{
	public class EvolutionAlgorithm
	{
		#region Constants

		/// <summary>
		/// Genomes cannot have zero fitness because the fitness sharing logic requires there to be 
		/// a non-zero total fitness in the population. Therefore this figure should be substituted
		/// in where zero fitness occurs.
		/// </summary>
		public const double MIN_GENOME_FITNESS = 0.00001;

		#endregion

		#region Class Variables

		Population pop;
		IPopulationEvaluator populationEvaluator;
		NeatParameters neatParameters;
		NeatParameters neatParameters_Normal;
		NeatParameters neatParameters_PrunePhase;

		bool pruningModeEnabled=false;
        bool pruningModeAlwaysOn=false;
		bool connectionWeightFixingEnabled=false;
		bool pruningMode=false;
		
		/// <summary>
		/// The last generation at which Population.AvgComplexity was reduced. We track this
		/// when simplifications have completed and that therefore the prune phase should end.
		/// </summary>
		long prunePhase_generationAtLastSimplification;
		float prunePhase_MinimumStructuresPerGenome;

		/// <summary>
		/// Population.AvgComplexity when AdjustSpeciationThreshold() was last called. If mean complexity
		/// moves away from this value by a certain amount then it's time to re-apply the speciation threshold
		/// to the whole population by calling pop.RedetermineSpeciation().
		/// </summary>
		double meanComplexityAtLastAdjustSpeciationThreshold;

		// All offspring are temporarily held here before being added to the population proper.
		GenomeList offspringList = new GenomeList();

		// Tables of new connections and neurons created during adiitive mutations. These tables
		// are available during the mutations and can be used to check for matching mutations so
		// that two mutations that create the same structure will be allocated the same ID. 
		// Currently this matching is only performed within the context of a generation, which
		// is how the original C++ NEAT code operated also.
		Hashtable newConnectionGeneTable = new Hashtable();
		Hashtable newNeuronGeneStructTable = new Hashtable();

		// Statistics
		uint generation=0;
		IGenome bestGenome;
		
		#endregion

		#region Constructors

		/// <summary>
		/// Default Constructor.
		/// </summary>
		public EvolutionAlgorithm(Population pop, IPopulationEvaluator populationEvaluator) : this(pop, populationEvaluator, new NeatParameters())
		{}

		/// <summary>
		/// Default Constructor.
		/// </summary>
		public EvolutionAlgorithm(Population pop, IPopulationEvaluator populationEvaluator, NeatParameters neatParameters)
		{
			this.pop = pop;
			this.populationEvaluator = populationEvaluator;
			this.neatParameters = neatParameters;
			neatParameters_Normal = neatParameters;

			neatParameters_PrunePhase = new NeatParameters(neatParameters);
			neatParameters_PrunePhase.pMutateAddConnection = 0.0;
			neatParameters_PrunePhase.pMutateAddNode = 0.0;
			neatParameters_PrunePhase.pMutateConnectionWeights = 0.33;
			neatParameters_PrunePhase.pMutateDeleteConnection = 0.33;
			neatParameters_PrunePhase.pMutateDeleteSimpleNeuron = 0.33;

			// Disable all crossover as this has a tendency to increase complexity, which is precisely what
			// we don't want during a pruning phase.
			neatParameters_PrunePhase.pOffspringAsexual = 1.0;
			neatParameters_PrunePhase.pOffspringSexual = 0.0;

			InitialisePopulation();
		}

		#endregion

		#region Properties

		public Population Population
		{
			get
			{
				return pop;
			}
		}

		public uint NextGenomeId
		{
			get
			{
				return pop.IdGenerator.NextGenomeId;
			}
		}

		public uint NextInnovationId
		{
			get
			{
				return pop.IdGenerator.NextInnovationId;
			}
		}

		public NeatParameters NeatParameters
		{
			get
			{
				return neatParameters;
			}
		}

		public IPopulationEvaluator PopulationEvaluator
		{
			get
			{
				return populationEvaluator;
			}
		}

		public uint Generation
		{
			get
			{
				return generation;
			}
		}

		public IGenome BestGenome
		{
			get
			{
				return bestGenome;
			}
		}

		public Hashtable NewConnectionGeneTable
		{
			get
			{
				return newConnectionGeneTable;
			}
		}

		public Hashtable NewNeuronGeneStructTable
		{
			get
			{
				return newNeuronGeneStructTable;
			}
		}

		public bool IsInPruningMode
		{
			get
			{
                return pruningMode;
			}
		}

		/// <summary>
		/// Get/sets a boolean indicating if the search should use pruning mode.
		/// </summary>
		public bool IsPruningModeEnabled
		{
			get
			{
                return pruningModeEnabled || pruningModeAlwaysOn;
			}
			set
			{
				pruningModeEnabled = value;
				if(value==false)
				{	// Weight fixing cannot (currently) occur with pruning mode disabled.
					connectionWeightFixingEnabled = false;
				}
			}
		}


        /// <summary>
        /// Get/sets a boolean indicating if the search should use pruning mode.
        /// </summary>
        public bool PruningModeAlwaysOn
        {
            get
            {
                return pruningModeAlwaysOn;
            }
            set
            {
                pruningModeAlwaysOn = value;
            }
        }

		/// <summary>
		/// Get/sets a boolean indicating if connection weight fixing is enabled. Note that this technique
		/// is currently tied to pruning mode, therefore if pruning mode is disabled then weight fixing
		/// will automatically be disabled.
		/// </summary>
		public bool IsConnectionWeightFixingEnabled
		{
			get
			{
				return connectionWeightFixingEnabled;
			}
			set
			{	// Ensure disabled if pruningMode is disabled.
				connectionWeightFixingEnabled = pruningModeEnabled && value;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Evaluate all genomes in the population, speciate them and then calculate adjusted fitness
		/// and related stats.
		/// </summary>
		/// <param name="p"></param>
		private void InitialisePopulation()
		{
			// The GenomeFactories normally won't bother to ensure that like connections have the same ID 
			// throughout the population (because it's not very easy to do in most cases). Therefore just
			// run this routine to search for like connections and ensure they have the same ID. 
			// Note. This could also be done periodically as part of the search, remember though that like
			// connections occuring within a generation are already fixed - using a more efficient scheme.
			MatchConnectionIds();

			// Evaluate the whole population. 
			populationEvaluator.EvaluatePopulation(pop, this);

			// Speciate the population.
			pop.BuildSpeciesTable(this);

			// Now we have fitness scores and a speciated population we can calculate fitness stats for the
			// population as a whole and per species.
			UpdateFitnessStats();

			// Set new threshold 110% of current level or 10 more if current complexity is very low.
			pop.PrunePhaseAvgComplexityThreshold = pop.AvgComplexity + neatParameters.pruningPhaseBeginComplexityThreshold;

			// Obtain an initial value for this variable that tracks when we should call pp.RedetermineSpeciation().
			meanComplexityAtLastAdjustSpeciationThreshold = pop.AvgComplexity;

			// Now we have stats we can determine the target size of each species as determined by the
			// fitness sharing logic.
			DetermineSpeciesTargetSize();

			// Check integrity.
			Debug.Assert(pop.PerformIntegrityCheck(), "Population integrity check failed.");
		}


		public void PerformOneGeneration()
		{
		//----- Elmininate any poor species before we do anything else. These are species with a zero target
		//		size for this generation and will therefore not have generate any offspring. Here we have to 
		//		explicitly eliminate these species, otherwise the species would persist because of elitism. 
		//		Also, the species object would persist without any genomes within it, so we have to clean it up.
		//		This code could be executed at the end of this method instead of the start, it doesn't really 
		//		matter. Except that If we do it here then the population size will be relatively constant
		//		between generations.
			if(pop.EliminateSpeciesWithZeroTargetSize())
			{	// If species were removed then we should recalculate population stats.
				UpdateFitnessStats();
				DetermineSpeciesTargetSize();
			}

		//----- Stage 1. Create offspring / cull old genomes / add offspring to population.
			CreateOffSpring();
			pop.TrimAllSpeciesBackToElite();

			// Add offspring to the population.
			int genomeBound = offspringList.Count;
			for(int genomeIdx=0; genomeIdx<genomeBound; genomeIdx++)
				pop.AddGenomeToPopulation(this, offspringList[genomeIdx]);

			// Adjust the speciation threshold to try and keep the number of species within defined limits.
			AdjustSpeciationThreshold();

		//----- Stage 2. Evaluate genomes / Update stats.
			populationEvaluator.EvaluatePopulation(pop, this);			
			UpdateFitnessStats();
			DetermineSpeciesTargetSize();
			
			pop.IncrementGenomeAges();
			pop.IncrementSpeciesAges();
			generation++;

		//----- Stage 3. Pruning phase tracking / Pruning phase entry & exit.
			if(pruningModeEnabled)
			{
				if(pruningMode)
				{
					// Track the falling population complexity.
					if(pop.AvgComplexity < prunePhase_MinimumStructuresPerGenome)
					{
						prunePhase_MinimumStructuresPerGenome = pop.AvgComplexity;
						prunePhase_generationAtLastSimplification = generation;
					}

					if(TestForPruningPhaseEnd())
						EndPruningPhase();
				}
				else
				{
					if(TestForPruningPhaseBegin())
						BeginPruningPhase();
				}
			}
		}

		/// <summary>
		/// Indicates that the # of species is outside of the desired bounds and that AdjustSpeciationThreshold()
		/// is attempting to adjust the speciation threshold at each generation to remedy the situation.
		/// </summary>
		private bool speciationThresholdAdjustInProgress=false;

		/// <summary>
		/// If speciationThresholdAdjustInProgress is true then the amount by which we are adjustinf the speciation
		/// threshol dper generation. This value is modified in order to try and find the correct threshold as quickly
		/// as possibly.
		/// </summary>
		private double compatibilityThresholdDelta;

		private const double compatibilityThresholdDeltaAcceleration = 1.05;

		

		private void AdjustSpeciationThreshold()
		{
			bool redetermineSpeciationFlag = false;
			int speciesCount = pop.SpeciesTable.Count;

			if(speciesCount < neatParameters.targetSpeciesCountMin)
			{	
				// Too few species. Reduce the speciation threshold.
				if(speciationThresholdAdjustInProgress)
				{	// Adjustment is already in progress.
					if(compatibilityThresholdDelta<0.0)
					{	// Negative delta. Correct direction, so just increase the delta to try and find the correct value as quickly as possible.
						compatibilityThresholdDelta*=compatibilityThresholdDeltaAcceleration;
					}
					else
					{	// Positive delta. Incorrect direction. This means we have overshot the correct value.
						// Reduce the delta and flip its sign.
						compatibilityThresholdDelta*=-0.5;
					}
				}
				else
				{	// Start new adjustment 'phase'.
					speciationThresholdAdjustInProgress = true;
					compatibilityThresholdDelta = -Math.Max(0.1, neatParameters.compatibilityThreshold * 0.01);
				}

				// Adjust speciation threshold by compatibilityThresholdDelta.
				neatParameters.compatibilityThreshold += compatibilityThresholdDelta;
				neatParameters.compatibilityThreshold = Math.Max(0.01, neatParameters.compatibilityThreshold);

				redetermineSpeciationFlag = true;
			}
			else if(speciesCount > neatParameters.targetSpeciesCountMax)
			{	
				// Too many species. Increase the species threshold.
				if(speciationThresholdAdjustInProgress)
				{	// Adjustment is already in progress.
					if(compatibilityThresholdDelta<0.0)
					{	// Negative delta. Incorrect direction. This means we have overshot the correct value.
						// Reduce the delta and flip its sign.
						compatibilityThresholdDelta*=-0.5;
					}
					else
					{	// Positive delta. Correct direction, so just increase the delta to try and find the correct value as quickly as possible.
						compatibilityThresholdDelta*=compatibilityThresholdDeltaAcceleration;
					}
				}
				else
				{	// Start new adjustment 'phase'.
					speciationThresholdAdjustInProgress = true;
					compatibilityThresholdDelta = Math.Max(0.1, neatParameters.compatibilityThreshold * 0.01);
				}

				// Adjust speciation threshold by compatibilityThresholdDelta.
				neatParameters.compatibilityThreshold += compatibilityThresholdDelta;

				redetermineSpeciationFlag = true;
			}
			else
			{	// Correct # of species. Ensure flag is reset.
				speciationThresholdAdjustInProgress=false;
			}

			if(!redetermineSpeciationFlag)
			{
				double complexityDeltaProportion = Math.Abs(pop.AvgComplexity-meanComplexityAtLastAdjustSpeciationThreshold)/meanComplexityAtLastAdjustSpeciationThreshold; 

				if(complexityDeltaProportion>0.05)
				{	// If the population's complexity has changed by more than some proportion then force a 
					// call to RedetermineSpeciation().
					redetermineSpeciationFlag = true;

					// Update the tracking variable.
					meanComplexityAtLastAdjustSpeciationThreshold = pop.AvgComplexity;
				}
			}

			if(redetermineSpeciationFlag)
			{
				// If the speciation threshold was adjusted then we must disregard all previous speciation 
				// and rebuild the species table.
				pop.RedetermineSpeciation(this);

                // CHANGE: This was often observed to have prolonged the pruning phase by a considerable number of generations. Thus 
                // we don't reset pruning tracking varaibles anymore.
                //// If we are in a pruning phase then we should reset the pruning phase tracking variables.
                //// We are effectively re-starting the pruning phase.
                //prunePhase_generationAtLastSimplification = generation;
                //prunePhase_MinimumStructuresPerGenome = pop.AvgComplexity;

//				Debug.WriteLine("ad hoc RedetermineSpeciation()");
			}
		}

//		/// <summary>
//		/// Returns true if the speciation threshold was adjusted.
//		/// </summary>
//		/// <returns></returns>
//		private bool AdjustSpeciationThreshold()
//		{
//			int speciesCount = pop.SpeciesTable.Count;
//
//			if(speciesCount < neatParameters.targetSpeciesCountMin)
//			{	
//				// Too few species. Reduce the speciation threshold.
//				if(speciationThresholdAdjustInProgress)
//				{	// Adjustment is already in progress.
//					if(compatibilityThresholdDelta<0.0)
//					{	// Negative delta. Correct direction, so just increase the delta to try and find the correct value as quickly as possible.
//						compatibilityThresholdDelta*=compatibilityThresholdDeltaAcceleration;
//					}
//					else
//					{	// Positive delta. Incorrect direction. This means we have overshot the correct value.
//						// Reduce the delta and flip its sign.
//						compatibilityThresholdDelta*=-0.5;
//					}
//				}
//				else
//				{	// Start new adjustment 'phase'.
//					speciationThresholdAdjustInProgress = true;
//					compatibilityThresholdDelta = -Math.Max(0.1, neatParameters.compatibilityThreshold * 0.01);
//				}
//
//				// Adjust speciation threshold by compatibilityThresholdDelta.
//				neatParameters.compatibilityThreshold += compatibilityThresholdDelta;
//				neatParameters.compatibilityThreshold = Math.Max(0.01, neatParameters.compatibilityThreshold);
//
//				Debug.WriteLine("delta=" + compatibilityThresholdDelta);
//
//				return true;
//			}
//			else if(speciesCount > neatParameters.targetSpeciesCountMax)
//			{	
//				// Too many species. Increase the species threshold.
//				if(speciationThresholdAdjustInProgress)
//				{	// Adjustment is already in progress.
//					if(compatibilityThresholdDelta<0.0)
//					{	// Negative delta. Incorrect direction. This means we have overshot the correct value.
//						// Reduce the delta and flip its sign.
//						compatibilityThresholdDelta*=-0.5;
//					}
//					else
//					{	// Positive delta. Correct direction, so just increase the delta to try and find the correct value as quickly as possible.
//						compatibilityThresholdDelta*=compatibilityThresholdDeltaAcceleration;
//					}
//				}
//				else
//				{	// Start new adjustment 'phase'.
//					speciationThresholdAdjustInProgress = true;
//					compatibilityThresholdDelta = Math.Max(0.1, neatParameters.compatibilityThreshold * 0.01);
//				}
//
//				// Adjust speciation threshold by compatibilityThresholdDelta.
//				neatParameters.compatibilityThreshold += compatibilityThresholdDelta;
//
//				Debug.WriteLine("delta=" + compatibilityThresholdDelta);
//
//				return true;
//			}
//			else
//			{	// Correct # of species. Ensure flag is reset.
//				speciationThresholdAdjustInProgress=false;
//				return false;
//			}
//		}

//		private const double compatibilityThresholdDeltaBaseline = 0.1;
//		private const double compatibilityThresholdDeltaAcceleration = 1.5;
//
//		private double compatibilityThresholdDelta = compatibilityThresholdDeltaBaseline;
//		private bool compatibilityThresholdDeltaDirection=true;
//		
//		/// <summary>
//		/// This routine adjusts the speciation threshold so that the number of species remains between the specified upper 
//		/// and lower limits. This routine implements a momentum approach so that the rate of change in the threshold increases
//		/// if the number of species remains incorrect for consecutive invocations.
//		/// </summary>
//		private void AdjustSpeciationThreshold()
//		{
//			double newThreshold;
//
//			if(pop.SpeciesTable.Count < neatParameters.targetSpeciesCountMin)
//			{
//				newThreshold = Math.Max(compatibilityThresholdDeltaBaseline, neatParameters.compatibilityThreshold - compatibilityThresholdDelta);
//
//				// Delta acceleration.
//				if(compatibilityThresholdDeltaDirection)
//				{	// Wrong direction - Direction change. Also reset compatibilityThresholdDelta.
//					compatibilityThresholdDelta = compatibilityThresholdDeltaBaseline;
//					compatibilityThresholdDeltaDirection=false;
//				}
//				else
//				{	// Already going in the right direction. 
//					compatibilityThresholdDelta *= compatibilityThresholdDeltaAcceleration;
//				}				
//			}
//			else if(pop.SpeciesTable.Count > neatParameters.targetSpeciesCountMax)
//			{
//				newThreshold = neatParameters.compatibilityThreshold + compatibilityThresholdDelta;
//
//				// Delta acceleration.
//				if(compatibilityThresholdDeltaDirection)
//				{	// Already going in the right direction. 
//					compatibilityThresholdDelta *= compatibilityThresholdDeltaAcceleration;
//				}
//				else
//				{	// Wrong direction - Direction change. Also reset compatibilityThresholdDelta.
//					compatibilityThresholdDelta = compatibilityThresholdDeltaBaseline;
//					compatibilityThresholdDeltaDirection=true;
//				}
//			}
//			else
//			{	// Current threshold is OK. Reset compatibilityThresholdDelta in case it has 'accelerated' to a large value.
//				// This would be a bad value to start with when the threshold next needs adjustment.
//				compatibilityThresholdDelta = compatibilityThresholdDeltaBaseline;
//				return;
//			}
//
//			neatParameters.compatibilityThreshold = newThreshold;
//
//			// If the speciation threshold was adjusted then we must disregard all previous speciation 
//			// and rebuild the species table.
//			pop.RedetermineSpeciation(this);
//		}

		#endregion

		#region Private Methods

		private void CreateOffSpring()
		{
			offspringList.Clear();
			CreateOffSpring_Asexual();
			CreateOffSpring_Sexual();
		}

		private void CreateOffSpring_Asexual()
		{
			// Create a new lists so that we can track which connections/neurons have been added during this routine.
			newConnectionGeneTable.Clear(); 
			newNeuronGeneStructTable.Clear();

			//----- Repeat the reproduction per species to give each species a fair chance at reproducion.
			//		Note that for this to work for small numbers of genomes in a species we need a reproduction 
			//		rate of 100% or more. This is analagous to the strategy used in NEAT.
			foreach(Species species in pop.SpeciesTable.Values)
			{
				// Determine how many asexual offspring to create. 
				// Minimum of 1. Any species with TargetSize of 0 are eliminated at the top of PerformOneGeneration(). This copes with the 
				// special case where every species may calculate offspringCount to be zero and therefor we loose the entire population!
				// This can happen e.g. when each genome is allocated it's own species with TargetSize of 1.
				int offspringCount = Math.Max(1,(int)Math.Round((species.TargetSize - species.ElitistSize) * neatParameters.pOffspringAsexual));
				for(int i=0; i<offspringCount; i++)
				{	// Add offspring to a seperate genomeList. We will add the offspring later to prevent corruption of the enumeration loop.
					IGenome parent = RouletteWheelSelect(species);
					IGenome offspring = parent.CreateOffspring_Asexual(this);
					offspring.ParentSpeciesId1 = parent.SpeciesId;
					offspringList.Add(offspring);
				}
			}
//			AmalgamateInnovations();
		}

//		/// <summary>
//		/// Mutations can sometime create the same innovation more than once within a population.
//		/// If this occurs then we ensure like innovations are allocated the same innovation ID.
//		/// This is for this generation only - if the innovation occurs in a later generation we
//		/// leave it as it is.
//		/// </summary>
//		private void AmalgamateInnovations()
//		{
//			// TODO: Inefficient routine. Revise.
//			// Indicates that at least one list's order has been invalidated.
//			bool bOrderInvalidated=false;
//
//			// Check through the new NeuronGenes - and their associated connections.
//			int neuronListBound = newNeuronGeneStructList.Count;
//			for(int i=0; i<neuronListBound-1; i++)
//			{
//				for(int j=i+1; j<neuronListBound; j++)
//				{
//					NewNeuronGeneStruct neuronGeneStruct1 = (NewNeuronGeneStruct)newNeuronGeneStructList[i];
//					NewNeuronGeneStruct neuronGeneStruct2 = (NewNeuronGeneStruct)newNeuronGeneStructList[j];
//
//					if(neuronGeneStruct1.NewConnectionGene_Input.SourceNeuronId == neuronGeneStruct2.NewConnectionGene_Input.SourceNeuronId &&
//						neuronGeneStruct1.NewConnectionGene_Output.TargetNeuronId == neuronGeneStruct2.NewConnectionGene_Output.TargetNeuronId)
//					{
//						neuronGeneStruct2.NewNeuronGene.InnovationId = neuronGeneStruct1.NewNeuronGene.InnovationId;
//						neuronGeneStruct2.NewConnectionGene_Input.InnovationId = neuronGeneStruct1.NewConnectionGene_Input.InnovationId;
//						neuronGeneStruct2.NewConnectionGene_Input.TargetNeuronId = neuronGeneStruct2.NewNeuronGene.InnovationId;
//
//						neuronGeneStruct2.NewConnectionGene_Output.InnovationId = neuronGeneStruct1.NewConnectionGene_Output.InnovationId;
//						neuronGeneStruct2.NewConnectionGene_Output.SourceNeuronId = neuronGeneStruct2.NewNeuronGene.InnovationId;
//
//						// Switching innovation numbers over can cause the genes to be out of order with respect
//						// to their innovation id. This order should be maintained at all times, so we set a flag here
//						// and re-order all effected lists at the end of this method.
//						neuronGeneStruct2.OwningGenome.NeuronGeneList.OrderInvalidated = true;
//						neuronGeneStruct2.OwningGenome.ConnectionGeneList.OrderInvalidated = true;
//						bOrderInvalidated = true;
//					}
//				}
//			}
//
//			// Check through the new connections.
//			int connectionListBound = newConnectionGeneStructList.Count;
//			for(int i=0; i<connectionListBound-1; i++)
//			{
//				for(int j=i+1; j<connectionListBound; j++)
//				{
//					NewConnectionGeneStruct connectionGeneStruct1 = (NewConnectionGeneStruct)newConnectionGeneStructList[i];
//					NewConnectionGeneStruct connectionGeneStruct2 = (NewConnectionGeneStruct)newConnectionGeneStructList[j];
//
//					if(connectionGeneStruct1.NewConnectionGene.SourceNeuronId == connectionGeneStruct2.NewConnectionGene.SourceNeuronId && 
//						connectionGeneStruct1.NewConnectionGene.TargetNeuronId == connectionGeneStruct2.NewConnectionGene.TargetNeuronId)
//					{
//						connectionGeneStruct2.NewConnectionGene.InnovationId = connectionGeneStruct1.NewConnectionGene.InnovationId;
//						connectionGeneStruct2.OwningGenome.ConnectionGeneList.OrderInvalidated = true;
//						bOrderInvalidated = true;
//					}
//				}
//			}
//
//			if(bOrderInvalidated)
//			{	// Re-order all invalidated lists within the population.
//				foreach(NeatGenome.NeatGenome genome in offspringList)
//				{
//					if(genome.NeuronGeneList.OrderInvalidated)
//						genome.NeuronGeneList.SortByInnovationId();
//
//					if(genome.ConnectionGeneList.OrderInvalidated)
//						genome.ConnectionGeneList.SortByInnovationId();
//				}
//			}
//		}

		//TODO: review this routine. parent could be null?
		private void CreateOffSpring_Sexual()
		{
			//----- Repeat the reproduction per species to give each species a fair chance at reproducion.
			//		Note that for this to work for small numbers of genomes in a species we need a reproduction 
			//		rate of 100% or more. This is analagous to the strategy used in NEAT.
			foreach(Species species in pop.SpeciesTable.Values)
			{
				bool oneMember=false;
				bool twoMembers=false;

				if(species.Members.Count==1)
				{
					// We can't perform sexual reproduction. To give the species a fair chance we call the asexual routine instead.
					// This keeps the proportions of genomes per species steady.
					oneMember = true;
				} 
				else if(species.Members.Count==2)
					twoMembers = true;			
	
				// Determine how many sexual offspring to create. 
				int matingCount = (int)Math.Round((species.TargetSize - species.ElitistSize) * neatParameters.pOffspringSexual);
				for(int i=0; i<matingCount; i++)
				{
					IGenome parent1;
					IGenome parent2=null;
					IGenome offspring;

					if(Utilities.NextDouble() < neatParameters.pInterspeciesMating)
					{	// Inter-species mating!
						//System.Diagnostics.Debug.WriteLine("Inter-species mating!");
						if(oneMember)
							parent1 = species.Members[0];
						else
							parent1 = RouletteWheelSelect(species); 

						// Select the 2nd parent from the whole popualtion (there is a chance that this will be an genome 
						// from this species, but that's OK).

						int j=0;
						do
						{
							parent2 = RouletteWheelSelect(pop);
						}
						while(parent1==parent2 && j++ < 4);	// Slightly wasteful but not too bad. Limited by j.	
					}
					else
					{	// Mating within the current species.
						//System.Diagnostics.Debug.WriteLine("Mating within the current species.");
						if(oneMember)
						{	// Use asexual reproduction instead.
							offspring = species.Members[0].CreateOffspring_Asexual(this);
							offspring.ParentSpeciesId1 = species.SpeciesId;
							offspringList.Add(offspring);
							continue;
						}

						if(twoMembers)
						{
							offspring = species.Members[0].CreateOffspring_Sexual(this, species.Members[1]);
							offspring.ParentSpeciesId1 = species.SpeciesId;
							offspring.ParentSpeciesId2 = species.SpeciesId;
							offspringList.Add(offspring);
							continue;
						}

						parent1 = RouletteWheelSelect(species);
						
						int j=0;
						do
						{
							parent2 = RouletteWheelSelect(species);
						}
						while(parent1==parent2 && j++ < 4);	// Slightly wasteful but not too bad. Limited by j.						
					}

					if(parent1 != parent2)
					{
						offspring = parent1.CreateOffspring_Sexual(this, parent2);
						offspring.ParentSpeciesId1 = parent1.SpeciesId;
						offspring.ParentSpeciesId2 = parent2.SpeciesId;
						offspringList.Add(offspring);
					}
					else
					{	// No mating pair could be found. Fallback to asexual reproduction to keep the population size constant.
						offspring = parent1.CreateOffspring_Asexual(this);
						offspring.ParentSpeciesId1 = parent1.SpeciesId;
						offspringList.Add(offspring);
					}
				}
			}
		}

		/// <summary>
		/// Biased select.
		/// </summary>
		/// <param name="species">Species to select from.</param>
		/// <returns></returns>
		private IGenome RouletteWheelSelect(Species species)
		{
			double selectValue = (Utilities.NextDouble() * species.SelectionCountTotalFitness);
			double accumulator=0.0;

			int genomeBound = species.Members.Count;
			for(int genomeIdx=0; genomeIdx<genomeBound; genomeIdx++)
			{
				IGenome genome = species.Members[genomeIdx];

				accumulator += genome.Fitness;
				if(selectValue <= accumulator)
					return genome;
			}
			// Should never reach here.
			return null;
		}

//		private IGenome EvenDistributionSelect(Species species)
//		{
//			return species.Members[Utilities.Next(species.SelectionCount)];
//		}


		/// <summary>
		/// Biased select.
		/// </summary>
		/// <param name="species">Species to select from.</param>
		/// <returns></returns>
		private IGenome RouletteWheelSelect(Population p)
		{
			double selectValue = (Utilities.NextDouble() * p.SelectionTotalFitness);
			double accumulator=0.0;

			int genomeBound = p.GenomeList.Count;
			for(int genomeIdx=0; genomeIdx<genomeBound;genomeIdx++)
			{
				IGenome genome = p.GenomeList[genomeIdx];

				accumulator += genome.Fitness;
				if(selectValue <= accumulator)
					return genome;
			}
			// Should never reach here.
			return null;
		}

		private void UpdateFitnessStats()
		{
			/// Indicates if the Candidate CullFlag has been set on any of the species in the first loop.
			bool bCandidateCullFlag=false;
			double bestFitness=double.MinValue;

			//----- Reset the population fitness values
			pop.ResetFitnessValues();
			pop.TotalNeuronCount = 0;
			pop.TotalConnectionCount = 0;

			//----- Loop through the speciesTable so that we can re-calculate fitness totals
			foreach(Species species in pop.SpeciesTable.Values)
			{
				species.ResetFitnessValues();
				species.TotalNeuronCount = 0;
				species.TotalConnectionCount = 0;

				// Members must be sorted so that we can calculate the total fitness of the top few genomes
				// for the selection routines.
				species.Members.Sort();

				// Keep track of the population's best genome and max fitness.
				NeatGenome.NeatGenome genome = (NeatGenome.NeatGenome)(species.Members[0]);
				if(genome.Fitness > bestFitness)
				{
					bestGenome = genome;
					bestFitness = bestGenome.Fitness;
				}

				// Track the generation number when the species improves.
				if(genome.Fitness > species.MaxFitnessEver)
				{
					species.MaxFitnessEver = genome.Fitness;
					species.AgeAtLastImprovement = species.SpeciesAge;
				}
				else if(!pruningMode && (species.SpeciesAge-species.AgeAtLastImprovement > neatParameters.speciesDropoffAge))  
				{	// The species is a candidate for culling. It may be given a pardon (later) if it is a champion species.
					species.CullCandidateFlag=true;
					bCandidateCullFlag=true;
				}

				//----- Update species totals in this first loop.
				// Calculate and store the number of genomes that will be selected from.
				species.SelectionCount = (int)Math.Max(1.0, Math.Round((double)species.Members.Count * neatParameters.selectionProportion));
				species.SelectionCountTotalFitness = 0.0;

				int genomeBound = species.Members.Count;
				for(int genomeIdx=0; genomeIdx<genomeBound;genomeIdx++)
				{
					genome = (NeatGenome.NeatGenome)(species.Members[genomeIdx]);
					Debug.Assert(genome.Fitness>=EvolutionAlgorithm.MIN_GENOME_FITNESS, "Genome fitness must be non-zero. Use EvolutionAlgorithm.MIN_GENOME_FITNESS");
					species.TotalFitness += genome.Fitness;

					if(genomeIdx < species.SelectionCount)
						species.SelectionCountTotalFitness += genome.Fitness;

					species.TotalNeuronCount += genome.NeuronGeneList.Count;
					species.TotalConnectionCount += genome.ConnectionGeneList.Count;
				}

				species.TotalStructureCount = species.TotalNeuronCount + species.TotalConnectionCount;
			}

			// If any species have had their CullCandidateFlag set then we need to execute some extra logic
			// to ensure we don't cull a champion species if it is the only champion species. 
			// If there is more than one champion species and all of them have the CullCandidateFlag set then
			// we unset the flag on one of them. Therefore we always at least one champion species in the 
			// population.
			if(bCandidateCullFlag)
			{
				ArrayList championSpecies = new ArrayList();

				//----- 2nd loop through species. Build list of champion species.
				foreach(Species species in pop.SpeciesTable.Values)
				{
					if(species.Members[0].Fitness == bestFitness)
						championSpecies.Add(species);
				}
				Debug.Assert(championSpecies.Count>0, "No champion species! There should be at least one.");

				if(championSpecies.Count==1)
				{	
					Species species = (Species)championSpecies[0];
					if(species.CullCandidateFlag==true)
					{
						species.CullCandidateFlag = false;

						// Also reset the species AgeAtLastImprovement so that it doesn't become 
						// a cull candidate every generation, which would inefficiently invoke this
						// extra logic on every generation.
						species.AgeAtLastImprovement=species.SpeciesAge;
					}
				}
				else
				{	// There are multiple champion species. Check for special case where all champions
					// are cull candidates.
					bool bAllChampionsAreCullCandidates = true; // default to true.
					foreach(Species species in championSpecies)
					{
						if(species.CullCandidateFlag)
							continue;

						bAllChampionsAreCullCandidates=false;
						break;
					}

					if(bAllChampionsAreCullCandidates)
					{	// Unset the flag on one of the champions at random.
						Species champ = (Species)championSpecies[(int)Math.Floor(Utilities.NextDouble()*championSpecies.Count)];
						champ.CullCandidateFlag = false;

						// Also reset the species AgeAtLastImprovement so that it doesn't become 
						// a cull candidate every generation, which would inefficiently invoke this
						// extra logic on every generation.
						champ.AgeAtLastImprovement=champ.SpeciesAge;
					}
				}
			}

			//----- 3rd loop through species. Update remaining stats.
			foreach(Species species in pop.SpeciesTable.Values)
			{
				const double MEAN_FITNESS_ADJUSTMENT_FACTOR = 0.01;

				if(species.CullCandidateFlag)
					species.MeanFitness = (species.TotalFitness / species.Members.Count) * MEAN_FITNESS_ADJUSTMENT_FACTOR;
				else
					species.MeanFitness = species.TotalFitness / species.Members.Count;

				//----- Update population totals.
				pop.TotalFitness += species.TotalFitness;
				pop.TotalSpeciesMeanFitness += species.MeanFitness;
				pop.SelectionTotalFitness += species.SelectionCountTotalFitness;
				pop.TotalNeuronCount += species.TotalNeuronCount;
				pop.TotalConnectionCount += species.TotalConnectionCount;
			}
			
			//----- Update some population stats /averages.
			if(bestFitness > pop.MaxFitnessEver)
			{
				Debug.WriteLine("UpdateStats() - bestFitness=" + bestGenome.Fitness.ToString() + ", " + bestFitness.ToString());
				pop.MaxFitnessEver = bestGenome.Fitness;
				pop.GenerationAtLastImprovement = this.generation;
			}

			pop.MeanFitness = pop.TotalFitness / pop.GenomeList.Count;
			pop.TotalStructureCount = pop.TotalNeuronCount + pop.TotalConnectionCount;
			pop.AvgComplexity = (float)pop.TotalStructureCount / (float)pop.GenomeList.Count;
		}

		/// <summary>
		/// Determine the target size of each species based upon the current fitness stats. The target size
		/// is stored against each Species object.
		/// </summary>
		/// <param name="p"></param>
		private void DetermineSpeciesTargetSize()
		{
			foreach(Species species in pop.SpeciesTable.Values)
			{
				species.TargetSize = (int)Math.Round((species.MeanFitness / pop.TotalSpeciesMeanFitness) * pop.PopulationSize);

				// Calculate how many elite genomes to keep in the next round. If this is a large number then we can only
				// keep as many genomes as we have!
				species.ElitistSize = Math.Min(species.Members.Count, (int)Math.Floor(species.TargetSize * neatParameters.elitismProportion));
                if((species.TargetSize==0 || species.ElitistSize==0) && species.Members[0]==bestGenome)
                {   // Ensure we don't cull the species that contains the best genome!
                    species.TargetSize = 1;
                    species.ElitistSize = 1;
                }
				else if(species.ElitistSize==0 && species.TargetSize > 1) 
				{	// If ElitistSize is calculated to be zero but the TargetSize non-zero then keep just one genome.
					// If the the TargetSize is 1 then we can't really do this since it would mean that no offspring would be generated.
					// So we throw away the one member and hope that the one offspring generated will be OK.
					species.ElitistSize = 1;
				}
			}
		}

		/// <summary>
		/// Search for connections with the same end-points throughout the whole population and 
		/// ensure that like connections have the same innovation ID.
		/// </summary>
		private void MatchConnectionIds()
		{
			Hashtable connectionIdTable = new Hashtable();

			int genomeBound=pop.GenomeList.Count;
			for(int genomeIdx=0; genomeIdx<genomeBound; genomeIdx++)
			{
				NeatGenome.NeatGenome genome = (NeatGenome.NeatGenome)pop.GenomeList[genomeIdx];

				int connectionGeneBound = genome.ConnectionGeneList.Count;
				for(int connectionGeneIdx=0; connectionGeneIdx<connectionGeneBound; connectionGeneIdx++)
				{
					ConnectionGene connectionGene = genome.ConnectionGeneList[connectionGeneIdx];

					ConnectionEndpointsStruct ces = new ConnectionEndpointsStruct();
					ces.sourceNeuronId = connectionGene.SourceNeuronId;
					ces.targetNeuronId = connectionGene.TargetNeuronId;
					
					Object existingConnectionIdObject = connectionIdTable[ces];
					if(existingConnectionIdObject==null)
					{	// No connection withthe same end-points has been registered yet, so 
						// add it to the table.
						connectionIdTable.Add(ces, connectionGene.InnovationId);
					}
					else
					{	// This connection is already registered. Give our latest connection 
						// the same innovation ID as the one in the table.
						connectionGene.InnovationId = (uint)existingConnectionIdObject;
					}
				}

				// The connection genes in this genome may now be out of order. Therefore we must ensure 
				// they are sorted before we continue.
				genome.ConnectionGeneList.SortByInnovationId();
			}
		}

		#endregion

		#region Private Methods [Pruning Phase]

		private bool TestForPruningPhaseBegin()
		{
			// Enter pruning phase if the complexity has risen beyond the specified threshold AND no gains in fitness have
			// occured for specified number of generations.
            return pruningModeAlwaysOn 
                    ||  ((pop.AvgComplexity > pop.PrunePhaseAvgComplexityThreshold) &&
					    ((generation-pop.GenerationAtLastImprovement) >= neatParameters.pruningPhaseBeginFitnessStagnationThreshold));
		}

		private bool TestForPruningPhaseEnd()
		{
			// Don't expect simplification on every generation. But if nothing has happened for 
			// 'pruningPhaseEndComplexityStagnationThreshold' gens then end the prune phase.            
            return !pruningModeAlwaysOn && (generation-prunePhase_generationAtLastSimplification > neatParameters.pruningPhaseEndComplexityStagnationThreshold);                                            
		}


		private void BeginPruningPhase()
		{
			// Enter pruning phase.
			pruningMode = true;
			prunePhase_generationAtLastSimplification = generation;
			prunePhase_MinimumStructuresPerGenome = pop.AvgComplexity;
			neatParameters = neatParameters_PrunePhase;

			// Copy the speciation threshold as this is dynamically altered during a search and we wish to maintain
			// the tracking during pruning.
			neatParameters.compatibilityThreshold = neatParameters_Normal.compatibilityThreshold;

			System.Diagnostics.Debug.WriteLine(">>Prune Phase<< Complexity=" + pop.AvgComplexity.ToString("0.00"));
		}

		private void EndPruningPhase()
		{
			// Leave pruning phase.
			pruningMode = false;

			// Set new threshold 110% of current level or 10 more if current complexity is very low.
			pop.PrunePhaseAvgComplexityThreshold = pop.AvgComplexity + neatParameters.pruningPhaseBeginComplexityThreshold;
			System.Diagnostics.Debug.WriteLine("complexity=" + pop.AvgComplexity.ToString() + ", threshold=" + pop.PrunePhaseAvgComplexityThreshold.ToString());

			neatParameters = neatParameters_Normal;
			neatParameters.compatibilityThreshold = neatParameters_PrunePhase.compatibilityThreshold;

			// Update species.AgaAtLastimprovement. Originally we reset this age to give all of the species
			// a 'clean slate' following the pruning phase. This though has the effect of giving all of the 
			// species the same AgeAtLastImprovement - which in turn often results in all of the species 
			// reaching the dropoff age simulataneously which results in the species being culled and therefore
			// causes a radical fall in population diversity.
			// Therefore we give the species a new AgeAtLastImprovement which reflects their relative 
			// AgeAtLastImprovement, this gives the species a new chance following pruning but does not allocate
			// them all the same AgeAtLastImprovement.
			NormalizeSpeciesAges();

			if(connectionWeightFixingEnabled)
			{
				// Fix all of the connection weights that remain after pruning (proven to be good values).
				foreach(NeatGenome.NeatGenome genome in pop.GenomeList)
					genome.FixConnectionWeights();
			}
		}

		private void NormalizeSpeciesAges()
		{
			float quarter_of_dropoffage = (float)neatParameters.speciesDropoffAge / 4.0F;

			// Calculate the spread of AgeAtLastImprovement - first find the min and max values.
			long minAgeAtLastImprovement;
			long maxAgeAtLastImprovement;

			minAgeAtLastImprovement = long.MaxValue;
			maxAgeAtLastImprovement = 0;

			foreach(Species species in pop.SpeciesTable.Values)
			{
				minAgeAtLastImprovement = Math.Min(minAgeAtLastImprovement, species.AgeAtLastImprovement);
				maxAgeAtLastImprovement = Math.Max(maxAgeAtLastImprovement, species.AgeAtLastImprovement);
			}

			long spread = maxAgeAtLastImprovement-minAgeAtLastImprovement;

			// Allocate each species a new AgeAtLastImprovement. Scale the ages so that the oldest is
			// only 25% towards the cutoff age.
			foreach(Species species in pop.SpeciesTable.Values)
			{
				long droppOffAge = species.AgeAtLastImprovement-minAgeAtLastImprovement;
				long newDropOffAge = (long)(((float)droppOffAge / (float)spread) * quarter_of_dropoffage);
				species.AgeAtLastImprovement = species.SpeciesAge - newDropOffAge;
			}
		}

		#endregion

		#region Some routines useful for profiling.
//		System.Text.StringBuilder sb = new System.Text.StringBuilder();
//		int tickCountStart;
//		int tickDuration;
//
//		private void StartMonitor()
//		{
//			tickCountStart = System.Environment.TickCount;
//		}
//
//		private void EndMonitor(string msg)
//		{
//			tickDuration =  System.Environment.TickCount - tickCountStart;
//			sb.Append(msg + " : " + tickDuration + " ms\n");
//		}
//
//		private void DumpMessage()
//		{
//			System.Windows.Forms.MessageBox.Show(sb.ToString());
//			sb = new System.Text.StringBuilder();
//		}
		#endregion
	}
}
