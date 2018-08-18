using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redzen.Numerics;
using Redzen.Random;
using Redzen.Sorting;
using SharpNeat.EA;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Speciation;

namespace SharpNeat.Neat.SelectionReproduction
{
    public class NeatSelectionReproductionStrategy<T> : ISelectionReproductionStrategy<NeatGenome<T>>
        where T : struct
    {
        #region Instance Fields

        readonly ISpeciationStrategy<NeatGenome<T>,T> _speciationStrategy;
        readonly EvolutionAlgorithmSettings _eaSettings;
        readonly IRandomSource _rng;

        #endregion

        #region Constructor

        public NeatSelectionReproductionStrategy(
            ISpeciationStrategy<NeatGenome<T>,T> speciationStrategy,
            EvolutionAlgorithmSettings eaSettings)
            : this(speciationStrategy, eaSettings, RandomDefaults.CreateRandomSource())
        {}

        public NeatSelectionReproductionStrategy(
            ISpeciationStrategy<NeatGenome<T>,T> speciationStrategy,
            EvolutionAlgorithmSettings eaSettings,
            IRandomSource rng)
        {
            _speciationStrategy = speciationStrategy ?? throw new ArgumentNullException(nameof(speciationStrategy));
            _eaSettings = eaSettings ?? throw new ArgumentNullException(nameof(eaSettings));
            _rng = rng ?? throw new ArgumentNullException(nameof(rng));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialise the strategy.
        /// </summary>
        public void Initialise(Population<NeatGenome<T>> population)
        {
            // Check for expected population type.
            if (!(population is NeatPopulation<T> neatPop)) {
                throw new ArgumentException("Invalid population type; expected NeatPopulation<T>.", "population");
            }

            // Initialise species.
            InitialiseSpecies(neatPop);

            // Calculate and store stats on the population as a whole, and for each species.
            PopulationStatsCalcs<T>.CalcAndStorePopulationStats(neatPop);
            SpeciesStatsCalcs<T>.CalcAndStoreSpeciesStats(neatPop, _eaSettings, _rng);
        }

        /// <summary>
        /// Invoke the strategy.
        /// Accepts a population of genomes, and updates it into a new population.
        /// </summary>
        /// <param name="population">The population to operate upon.</param>
        public void Invoke(Population<NeatGenome<T>> population)
        {
            var neatPop = population as NeatPopulation<T>;
            

            // Select genomes.


            // Create offspring genomes.
            //List<NeatGenome<T>> offspringList = CreateOffspring(specieStatsArr, offspringCount);


            // Trim species back to their elite genomes.


            // Add offspring into genomeList.
        

            // Evaluate genomes.


            // Add/merge offspring into species.


            // Calc species target sizes.
            //SpeciesAllocationCalcs<T>.CalcAndStoreSpeciesTargetSizes(neatPop, _rng);

            // TODO: Implement.
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        private void InitialiseSpecies(NeatPopulation<T> pop)
        {
            // Allocate the genomes to species.
            Species<T>[] speciesArr = _speciationStrategy.SpeciateAll(pop.GenomeList, _eaSettings.SpeciesCount);
            if(null == speciesArr || speciesArr.Length != _eaSettings.SpeciesCount) {
                throw new Exception("Species array is null or has incorrect length.");
            }
            pop.SpeciesArray = speciesArr;

            // Sort the genomes in each species. Highest fitness first, then secondary sorted by youngest genomes first.
            foreach(Species<T> species in speciesArr) {
                SortUtils.SortUnstable(species.GenomeList, GenomeFitnessAndAgeComparer<T>.Singleton, _rng);
            }
        }

        #endregion
    }
}
