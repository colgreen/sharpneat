/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System.Collections.Generic;
using System.Linq;
using Redzen;
using Redzen.Numerics;
using Redzen.Numerics.Distributions;
using Redzen.Random;
using SharpNeat.Evaluation;
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
        readonly double _interspeciesMatingProportion;
        readonly IComparer<FitnessInfo> _fitnessComparer;

        #endregion

        #region Constructor

        public OffspringBuilder(
            NeatReproductionAsexual<T> reproductionAsexual,
            NeatReproductionSexual<T> reproductionSexual,
            double interspeciesMatingProportion,
            IComparer<FitnessInfo> fitnessComparer)
        {
            _reproductionAsexual = reproductionAsexual;
            _reproductionSexual = reproductionSexual;
            _interspeciesMatingProportion = interspeciesMatingProportion;
            _fitnessComparer = fitnessComparer;
        }

        #endregion

        #region Public Methods

        public List<NeatGenome<T>> CreateOffspring(Species<T>[] speciesArr, IRandomSource rng)
        {
            // Create selection distributions.
            // Notes.
            // speciesDist is for selecting species when performing inter-species sexual reproduction, i.e. selecting parent genomes 
            // from two separate species. 
            // genomeDistArr is an array of distributions, one per species; this is for selecting genomes for intra-species reproduction.
            CreateSelectionDistributionUtils<T>.CreateSelectionDistributions(
                speciesArr,
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
                interspeciesMatingProportionResolved, rng);

            return offspringList;
        }

        #endregion

        #region Private Static Methods [CreateOffspring]

        private List<NeatGenome<T>> CreateOffspring(
            Species<T>[] speciesArr,
            DiscreteDistribution speciesDist,
            DiscreteDistribution[] genomeDistArr,
            double interspeciesMatingProportion,
            IRandomSource rng)
        {
            // Calc total number of offspring to produce for the population as a whole.
            int offspringCount = speciesArr.Sum(x => x.Stats.OffspringCount);

            // Create an empty list to add the offspring to (with preallocated storage).
            var offspringList = new List<NeatGenome<T>>(offspringCount);

            // Loop the species.
            for(int speciesIdx=0; speciesIdx < speciesArr.Length; speciesIdx++)
            {
                // Get the current species.
                Species<T> species = speciesArr[speciesIdx];
                
                // Get the DiscreteDistribution for genome selection within the current species.
                DiscreteDistribution genomeDist = genomeDistArr[speciesIdx];

                // Determine how many offspring to create through asexual and sexual reproduction.
                SpeciesStats stats = species.Stats;
                int offspringCountAsexual = stats.OffspringAsexualCount;
                int offspringCountSexual = stats.OffspringSexualCount;

                // Special case: A species with a single genome marked for selection, cannot perform intra-species sexual reproduction.
                if(species.Stats.SelectionSizeInt == 1)
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
                CreateSpeciesOffspringAsexual(
                    species, genomeDist, offspringCountAsexual, offspringList, rng);

                CreateSpeciesOffspringSexual(
                    speciesArr, species, speciesDistUpdated,
                    genomeDistArr, genomeDist,
                    offspringCountSexual, offspringList,
                    interspeciesMatingProportion, rng);
            }

            return offspringList;
        }

        private void CreateSpeciesOffspringAsexual(
            Species<T> species,
            DiscreteDistribution genomeDist,
            int offspringCount,
            List<NeatGenome<T>> offspringList,
            IRandomSource rng)
        {
            var genomeList = species.GenomeList;

            // Produce the required number of offspring from asexual reproduction.
            for(int i=0; i < offspringCount; i++)
            {
                // Select/sample a genome from the species.
                int genomeIdx = DiscreteDistribution.Sample(rng, genomeDist);
                var parentGenome = genomeList[genomeIdx];

                // Spawn a child genome and add it to offspringList.
                var childGenome = _reproductionAsexual.CreateChildGenome(parentGenome, rng);
                offspringList.Add(childGenome);
            }
        }

        private void CreateSpeciesOffspringSexual(
            Species<T>[] speciesArr,
            Species<T> species,
            DiscreteDistribution speciesDistUpdated,
            DiscreteDistribution[] genomeDistArr,
            DiscreteDistribution genomeDist,
            int offspringCount,
            List<NeatGenome<T>> offspringList,
            double interspeciesMatingProportion,
            IRandomSource rng)
        {
            // Calc the number of offspring to create via inter-species sexual reproduction.
            int offspringCountSexualInter;
            if(interspeciesMatingProportion == 0.0) {
                offspringCountSexualInter = 0;
            } else {
                offspringCountSexualInter = (int)NumericsUtils.ProbabilisticRound(interspeciesMatingProportion * offspringCount, rng);
            }

            // Calc the number of offspring to create via intra-species sexual reproduction.
            int offspringCountSexualIntra = offspringCount - offspringCountSexualInter;

            // Get genome list for the current species.
            var genomeList = species.GenomeList;
            
            // Produce the required number of offspring from inter-species sexual reproduction.
            for(int i=0; i < offspringCountSexualInter; i++)
            {
                // Select/sample parent A from the current species.
                int genomeIdx = DiscreteDistribution.Sample(rng, genomeDist);
                var parentGenomeA = genomeList[genomeIdx];

                // Select another species to select parent B from.
                int speciesIdx = DiscreteDistribution.Sample(rng, speciesDistUpdated);
                Species<T> speciesB = speciesArr[speciesIdx];

                // Select parent B from species B.
                DiscreteDistribution genomeDistB = genomeDistArr[speciesIdx];
                genomeIdx = DiscreteDistribution.Sample(rng, genomeDistB);
                var parentGenomeB = speciesB.GenomeList[genomeIdx];

                // Ensure parentA is the fittest of the two parents.
                if(_fitnessComparer.Compare(parentGenomeA.FitnessInfo, parentGenomeB.FitnessInfo) < 0) {
                    VariableUtils.Swap(ref parentGenomeA, ref parentGenomeB);
                }

                // Create a child genome and add it to offspringList.
                var childGenome = _reproductionSexual.CreateGenome(parentGenomeA, parentGenomeB, rng);
                offspringList.Add(childGenome);
            }

            // Produce the required number of offspring from intra-species sexual reproduction.
            for(int i=0; i < offspringCountSexualIntra; i++)
            {
                // Select/sample parent A from the species.
                int genomeIdx = DiscreteDistribution.Sample(rng, genomeDist);
                var parentGenomeA = genomeList[genomeIdx];

                // Create a new distribution with parent A removed from the set of possibilities.
                DiscreteDistribution genomeDistUpdated = genomeDist.RemoveOutcome(genomeIdx);

                // Select/sample parent B from the species.
                genomeIdx = DiscreteDistribution.Sample(rng, genomeDistUpdated);
                var parentGenomeB = genomeList[genomeIdx];

                // Create a child genome and add it to offspringList.
                var childGenome = _reproductionSexual.CreateGenome(parentGenomeA, parentGenomeB, rng);
                offspringList.Add(childGenome);
            }
        }

        #endregion
    }
}
