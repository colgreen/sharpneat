// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using Redzen.Numerics;
using Redzen.Numerics.Distributions;
using SharpNeat.Neat.Reproduction.Asexual;
using SharpNeat.Neat.Reproduction.Sexual;

namespace SharpNeat.Neat.EvolutionAlgorithm;

// TODO: Unit tests.

/// <summary>
/// For creating new offspring genomes, from one or two parents selected stochastically from a population.
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
internal sealed class OffspringBuilder<TScalar>
    where TScalar : struct, IBinaryFloatingPointIeee754<TScalar>
{
    readonly NeatReproductionAsexual<TScalar> _reproductionAsexual;
    readonly NeatReproductionSexual<TScalar> _reproductionSexual;
    readonly double _interspeciesMatingProportion;
    readonly IComparer<FitnessInfo> _fitnessComparer;

    #region Constructor

    /// <summary>
    /// Construct a new instance of <see cref="OffspringBuilder{T}"/>.
    /// </summary>
    /// <param name="reproductionAsexual">Asexual reproduction strategy.</param>
    /// <param name="reproductionSexual">Sexual reproduction strategy.</param>
    /// <param name="interspeciesMatingProportion">Inter-species mating proportion.</param>
    /// <param name="fitnessComparer">Fitness comparer.</param>
    public OffspringBuilder(
        NeatReproductionAsexual<TScalar> reproductionAsexual,
        NeatReproductionSexual<TScalar> reproductionSexual,
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

    /// <summary>
    /// Creates new offspring from existing parent genomes.
    /// </summary>
    /// <param name="speciesArr">An array of species, containing existing elite genomes.</param>
    /// <param name="rng">Random source.</param>
    /// <param name="offspringAsexualCount">Returns the number of new offspring created using asexual reproduction.</param>
    /// <param name="offspringSexualCount">Returns the number of new offspring created using sexual reproduction.</param>
    /// <param name="offspringInterspeciesCount">Returns the number of new offspring created using sexual
    /// reproduction between genomes from different species.</param>
    /// <returns>A new list containing the created offspring genomes.</returns>
    /// <remarks>
    /// Each species contains a genome list, which should have been trimmed back to the elite genomes only,
    /// in order to make space for the new offspring. New offspring are created by application of sexual and
    /// asexual reproduction on the existing genomes.
    ///
    /// The number of offspring genomes to create is determined by <see cref="SpeciesStats.OffspringAsexualCount"/>
    /// and <see cref="SpeciesStats.OffspringSexualCount"/>.
    /// </remarks>
    public List<NeatGenome<TScalar>> CreateOffspring(
        Species<TScalar>[] speciesArr,
        IRandomSource rng,
        out int offspringAsexualCount,
        out int offspringSexualCount,
        out int offspringInterspeciesCount)
    {
        // Create selection distributions.
        // Notes.
        // speciesDist is for selecting species when performing inter-species sexual reproduction, i.e. selecting parent genomes
        // from two separate species.
        // genomeDistArr is an array of distributions, one per species; this is for selecting genomes for intra-species reproduction.
        CreateSelectionDistributionUtils<TScalar>.CreateSelectionDistributions(
            speciesArr,
            out DiscreteDistribution<double> speciesDist,
            out DiscreteDistribution<double>?[] genomeDistArr,
            out int populatedSpeciesCount);

        // Resolve the interspecies mating proportion.
        double interspeciesMatingProportionResolved = _interspeciesMatingProportion;
        if(populatedSpeciesCount <= 1)
            interspeciesMatingProportionResolved = 0.0;

        // Create the offspring.
        var offspringList = CreateOffspring(
            speciesArr, speciesDist, genomeDistArr,
            interspeciesMatingProportionResolved, rng,
            out offspringAsexualCount,
            out offspringSexualCount,
            out offspringInterspeciesCount);

        return offspringList;
    }

    #endregion

    #region Private Static Methods [CreateOffspring]

    private List<NeatGenome<TScalar>> CreateOffspring(
        Species<TScalar>[] speciesArr,
        DiscreteDistribution<double> speciesDist,
        DiscreteDistribution<double>?[] genomeDistArr,
        double interspeciesMatingProportion,
        IRandomSource rng,
        out int asexualCount, out int sexualCount,
        out int interspeciesCount)
    {
        asexualCount = sexualCount = interspeciesCount = 0;

        // Calc total number of offspring to produce for the population as a whole.
        int offspringCount = speciesArr.Sum(x => x.Stats.OffspringCount);

        // Create an empty list to add the offspring to (with preallocated storage).
        var offspringList = new List<NeatGenome<TScalar>>(offspringCount);

        // Loop the species.
        for(int speciesIdx = 0; speciesIdx < speciesArr.Length; speciesIdx++)
        {
            // Get the current species.
            Species<TScalar> species = speciesArr[speciesIdx];

            // Skip species that have been marked to not produce any offspring.
            if(species.Stats.SelectionSizeInt == 0)
                continue;

            // Get the DiscreteDistribution for genome selection within the current species.
            var genomeDist = genomeDistArr[speciesIdx]!;

            // Determine how many offspring to create through asexual and sexual reproduction.
            SpeciesStats stats = species.Stats;
            int offspringCountAsexual = stats.OffspringAsexualCount;
            int offspringCountSexual = stats.OffspringSexualCount;

            // Special case: A species with a single genome marked for selection, cannot perform intra-species sexual reproduction.
            if(species.Stats.SelectionSizeInt == 1)
            {
                // Note. here we assign all the sexual reproduction allocation to asexual reproduction. In principle
                // we could still perform inter-species sexual reproduction, but that complicates the code further
                // for minimal gain.
                offspringCountAsexual += offspringCountSexual;
                offspringCountSexual = 0;
            }

            // Create a copy of speciesDist with the current species removed from the set of possibilities.
            // Note. The remaining probabilities are normalised to sum to one.
            var speciesDistUpdated = speciesDist.RemoveOutcome(speciesIdx);

            // Create offspring from the current species.
            CreateSpeciesOffspringAsexual(
                species, genomeDist, offspringCountAsexual, offspringList, rng);

            CreateSpeciesOffspringSexual(
                speciesArr, species, speciesDistUpdated,
                genomeDistArr, genomeDist,
                offspringCountSexual, offspringList,
                interspeciesMatingProportion, rng,
                out int interspeciesCountTmp);

            // Keep track of how many offspring have been created via asexual and sexual reproduction.
            asexualCount += offspringCountAsexual;
            sexualCount += offspringCountSexual;
            interspeciesCount += interspeciesCountTmp;
        }

        return offspringList;
    }

    private void CreateSpeciesOffspringAsexual(
        Species<TScalar> species,
        DiscreteDistribution<double> genomeDist,
        int offspringCount,
        List<NeatGenome<TScalar>> offspringList,
        IRandomSource rng)
    {
        var genomeList = species.GenomeList;

        // Produce the required number of offspring from asexual reproduction.
        for(int i=0; i < offspringCount; i++)
        {
            // Select/sample a genome from the species.
            int genomeIdx = genomeDist.Sample(rng);
            var parentGenome = genomeList[genomeIdx];

            // Spawn a child genome and add it to offspringList.
            var childGenome = _reproductionAsexual.CreateChildGenome(parentGenome, rng);
            offspringList.Add(childGenome);
        }
    }

    private void CreateSpeciesOffspringSexual(
        Species<TScalar>[] speciesArr,
        Species<TScalar> species,
        DiscreteDistribution<double> speciesDistUpdated,
        DiscreteDistribution<double>?[] genomeDistArr,
        DiscreteDistribution<double> genomeDist,
        int offspringCount,
        List<NeatGenome<TScalar>> offspringList,
        double interspeciesMatingProportion,
        IRandomSource rng,
        out int offspringInterspeciesCount)
    {
        // Calc the number of offspring to create via inter-species sexual reproduction.
        int offspringCountSexualInter;
        if(interspeciesMatingProportion == 0.0)
            offspringInterspeciesCount = offspringCountSexualInter = 0;
        else
            offspringInterspeciesCount = offspringCountSexualInter = (int)NumericsUtils.StochasticRound(interspeciesMatingProportion * offspringCount, rng);

        // Calc the number of offspring to create via intra-species sexual reproduction.
        int offspringCountSexualIntra = offspringCount - offspringCountSexualInter;

        // Get genome list for the current species.
        var genomeList = species.GenomeList;

        // Produce the required number of offspring from intra-species sexual reproduction.
        for(int i=0; i < offspringCountSexualIntra; i++)
        {
            // Select/sample parent A from the species.
            int genomeIdx = genomeDist.Sample(rng);
            var parentGenomeA = genomeList[genomeIdx];

            // Create a new distribution with parent A removed from the set of possibilities.
            var genomeDistUpdated = genomeDist.RemoveOutcome(genomeIdx);

            // Select/sample parent B from the species.
            genomeIdx = genomeDistUpdated.Sample(rng);
            var parentGenomeB = genomeList[genomeIdx];

            // Create a child genome and add it to offspringList.
            var childGenome = _reproductionSexual.CreateGenome(parentGenomeA, parentGenomeB, rng);
            offspringList.Add(childGenome);
        }

        // Produce the required number of offspring from inter-species sexual reproduction.
        for(int i=0; i < offspringCountSexualInter; i++)
        {
            // Select/sample parent A from the current species.
            int genomeIdx = genomeDist.Sample(rng);
            var parentGenomeA = genomeList[genomeIdx];

            // Select another species to select parent B from.
            int speciesIdx = speciesDistUpdated.Sample(rng);
            Species<TScalar> speciesB = speciesArr[speciesIdx];

            // Select parent B from species B.
            var genomeDistB = genomeDistArr[speciesIdx]!;
            genomeIdx = genomeDistB.Sample(rng);
            var parentGenomeB = speciesB.GenomeList[genomeIdx];

            // Ensure parentA is the fittest of the two parents.
            if(_fitnessComparer.Compare(parentGenomeA.FitnessInfo, parentGenomeB.FitnessInfo) < 0)
                VariableUtils.Swap(ref parentGenomeA!, ref parentGenomeB!);

            // Create a child genome and add it to offspringList.
            var childGenome = _reproductionSexual.CreateGenome(parentGenomeA, parentGenomeB, rng);
            offspringList.Add(childGenome);
        }
    }

    #endregion
}
