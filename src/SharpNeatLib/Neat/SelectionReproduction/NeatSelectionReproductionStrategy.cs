using Redzen.Numerics;
using Redzen.Random;
using SharpNeat.EA;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Speciation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Neat.SelectionReproduction
{
    public class NeatSelectionReproductionStrategy<T> : ISelectionReproductionStrategy<NeatGenome<T>>
        where T : struct
    {
        #region Instance Fields

        readonly ISpeciationStrategy<NeatGenome<T>,T> _speciationStrategy;
        readonly int _speciesCount;
        readonly IRandomSource _rng;

        #endregion

        #region Constructor

        public NeatSelectionReproductionStrategy(
            ISpeciationStrategy<NeatGenome<T>,T> speciationStrategy,
            int speciesCount,
            IRandomSource rng)
        {
            _speciationStrategy = speciationStrategy;
            _speciesCount = speciesCount;
            _rng = rng;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialise the strategy.
        /// </summary>
        public void Initialise(Population<NeatGenome<T>> population)
        {
            // Check for expected population type.
            var neatPop = population as NeatPopulation<T>;
            if(null == neatPop) {
                throw new ArgumentException("Invalid population type; expected NeatPopulation<T>.", "population");
            }

            // Initialise species.
            var speciesArr = _speciationStrategy.SpeciateAll(population.GenomeList, _speciesCount);
            if(null == speciesArr || speciesArr.Length != _speciesCount) {
                throw new Exception("Species array is null or has incorrect length.");
            }

            neatPop.SpeciesArray = speciesArr;
        }

        /// <summary>
        /// Invoke the strategy.
        /// Accepts a population of genomes, and updates it into a new population.
        /// </summary>
        /// <param name="population">The population to operate upon.</param>
        public void Invoke(Population<NeatGenome<T>> population)
        {
            var neatPop = population as NeatPopulation<T>;
            
            // Calc species target sizes.
            SpeciesAllocationCalcs<T>.CalcSpeciesTargetSizes(neatPop, _rng);







            throw new NotImplementedException();
        }

        #endregion

    }
}
