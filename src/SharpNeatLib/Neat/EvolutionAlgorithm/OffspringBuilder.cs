using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redzen.Numerics;
using Redzen.Random;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Asexual;
using SharpNeat.Neat.Reproduction.Sexual;
using SharpNeat.Neat.Speciation;

namespace SharpNeat.Neat.EvolutionAlgorithm
{
    // TODO: Unit tests.

    /// <summary>
    /// For creating new offspring genomes, from one or two parents selected stochastically from a population.
    /// </summary>
    /// <typeparam name="T">Neural net numeric data type.</typeparam>
    internal class OffspringBuilder<T> where T : struct
    {
        #region Instance Fields

        readonly NeatReproductionAsexual<T> _reproductionAsexual;
        readonly NeatReproductionSexual<T> _reproductionSexual;
        readonly IRandomSource _rng;
        readonly double _interspeciesMatingProportion;

        #endregion

        #region Constructor

        public OffspringBuilder(
            NeatReproductionAsexual<T> reproductionAsexual,
            NeatReproductionSexual<T> reproductionSexual,
            IRandomSource rng,
            double interspeciesMatingProportion)
        {
            _reproductionAsexual = reproductionAsexual;
            _reproductionSexual = reproductionSexual;
            _rng = rng;
            _interspeciesMatingProportion = interspeciesMatingProportion;
        }

        #endregion

        #region Public Methods

        public List<NeatGenome<T>> CreateOffspring(Species<T>[] speciesArr)
        {
            // Create selection distributions.
            // Notes.
            // speciesDist is for selecting species when performing inter-species sexual reproduction, i.e. selecting parent genomes 
            // from two separate species. 
            // genomeDistArr is an array of distributions, one per species; this is for selecting genomes for intra-species reproduction.
            CreateSelectionDistributionUtils<T>.CreateSelectionDistributions(
                speciesArr, _rng,
                out DiscreteDistribution speciesDist,
                out DiscreteDistribution[] genomeDistArr,
                out int populatedSpeciesCount);

            // Resolve the interspecies mating proportion.
            double interspeciesMatingProportionResolved = _interspeciesMatingProportion;
            if(populatedSpeciesCount <= 1) {
                interspeciesMatingProportionResolved = 0.0;
            }

            // Create the offspring.
            var offspringList = CreateOffspring(
                speciesArr, speciesDist, genomeDistArr,
                interspeciesMatingProportionResolved);

            return offspringList;
        }

        #endregion

        #region Private Static Methods [CreateOffspring]

        private List<NeatGenome<T>> CreateOffspring(
            Species<T>[] speciesArr,
            DiscreteDistribution speciesDist,
            DiscreteDistribution[] genomeDistArr,
            double interspeciesMatingProportion)
        {
            // Calc total number of offspring to produce for the population as a whole.
            int offspringCount = speciesArr.Sum(x => x.Stats.OffspringCount);

            // Create an empty list to add the offspring to (with preallocated storage).
            var offspringList = new List<NeatGenome<T>>(offspringCount);

            for(int speciesIdx=0; speciesIdx < speciesArr.Length; speciesIdx++)
            {
                // Get the current species.
                Species<T> species = speciesArr[speciesIdx];
                
                // Get the DiscreteDistribution for genome selection within this species.
                DiscreteDistribution genomeDist = genomeDistArr[speciesIdx];

                // Determine how many offspring to create through asexual and sexual reproduction.
                SpeciesStats stats = species.Stats;
                int offspringCountAsexual = stats.OffspringAsexualCount;
                int offspringCountSexual = stats.OffspringSexualCount;

                // Special case: A species with a single genome cannot perform intra-species sexual reproduction.
                if(species.GenomeList.Count == 1)
                {
                    // Note. here we assign all the sexual reproduction allocation to asexual reproduction. In principle 
                    // we could still perform inter species sexual reproduction, but that complicates the code further
                    // for minimal gain.
                    offspringCountAsexual += offspringCountSexual;
                    offspringCountSexual = 0;
                }

                // Create a copy of speciesDist with the current species removed from the set of possibilities.
                // Note. The remaining probabilities are normalised to sum to one.
                DiscreteDistribution speciesDistUpdated = speciesDist.RemoveOutcome(speciesIdx);

                // Create offspring from the current species.
                CreateSpeciesOffspringAsexual(species, genomeDist, offspringCountAsexual, offspringList);
                CreateSpeciasOffspringSexual(speciesArr, species, speciesDistUpdated, genomeDistArr, genomeDist, offspringCountSexual, offspringList);
            }

            return offspringList;
        }

        private void CreateSpeciesOffspringAsexual(
            Species<T> species,
            DiscreteDistribution genomeDist,
            int offspringCount,
            List<NeatGenome<T>> offspringList)
        {
            var genomeList = species.GenomeList;

            // Produce the required number of offspring from asexual reproduction.
            for(int i=0; i < offspringCount; i++)
            {
                // Select/sample a genome from the species.
                int genomeIdx = genomeDist.Sample();
                var parentGenome = genomeList[genomeIdx];

                // Spawn a child genome and add it to offspringList.
                var childGenome = _reproductionAsexual.CreateChildGenome(parentGenome);
                offspringList.Add(childGenome);
            }
        }

        private void CreateSpeciasOffspringSexual(
            Species<T>[] speciesArr,
            Species<T> species,
            DiscreteDistribution speciesDistUpdated,
            DiscreteDistribution[] genomeDistArr,
            DiscreteDistribution genomeDist,
            int offspringCount,
            List<NeatGenome<T>> offspringList)
        {
            // Calc the number of offspring to create via inter-species sexual reproduction.
            int offspringCountSexualInter;
            if(_interspeciesMatingProportion == 0.0) {
                offspringCountSexualInter = 0;
            } else {
                offspringCountSexualInter = (int)NumericsUtils.ProbabilisticRound(_interspeciesMatingProportion * offspringCount, _rng);
            }

            // Calc the number of offspring to create via intra-species sexual reproduction.
            int offspringCountSexualIntra = offspringCount - offspringCountSexualInter;

            // Get genome list for the current species.
            var genomeList = species.GenomeList;
            
            // Produce the required number of offspring from inter-species sexual reproduction.
            for(int i=0; i < offspringCountSexualInter; i++)
            {
                // Select/sample parent A from the current species.
                int genomeIdx = genomeDist.Sample();
                var parentGenomeA = genomeList[genomeIdx];

                // Select another species to select parent B from.
                int speciesIdx = speciesDistUpdated.Sample();
                Species<T> speciesB = speciesArr[speciesIdx];

                // Select parent B from species B.
                DiscreteDistribution genomeDistB = genomeDistArr[speciesIdx];
                genomeIdx = genomeDistB.Sample();
                var parentGenomeB = speciesB.GenomeList[genomeIdx];

                // Create a child genome and add it to offspringList.
                var childGenome = _reproductionSexual.CreateGenome(parentGenomeA, parentGenomeB);
                offspringList.Add(childGenome);
            }

            // Produce the required number of offspring from intra-species sexual reproduction.
            for(int i=0; i < offspringCountSexualIntra; i++)
            {
                // Select/sample parent A from the species.
                int genomeIdx = genomeDist.Sample();
                var parentGenomeA = genomeList[genomeIdx];

                // Create a new distribution with parent A removed from the set of possibilities.
                DiscreteDistribution genomeDistUpdated = genomeDist.RemoveOutcome(genomeIdx);

                // Select/sample parent B from the species.
                genomeIdx = genomeDistUpdated.Sample();
                var parentGenomeB = genomeList[genomeIdx];

                // Create a child genome and add it to offspringList.
                var childGenome = _reproductionSexual.CreateGenome(parentGenomeA, parentGenomeB);
                offspringList.Add(childGenome);
            }
        }

        #endregion
    }
}
