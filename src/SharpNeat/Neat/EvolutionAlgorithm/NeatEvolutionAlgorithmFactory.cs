// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.Experiments;
using SharpNeat.Neat.DistanceMetrics;
using SharpNeat.Neat.Genome.Decoders;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation;

namespace SharpNeat.Neat.EvolutionAlgorithm;

/// <summary>
/// Utility methods for creating and correctly 'wiring up' instances of NeatEvolutionAlgorithm.
/// </summary>
public static class NeatEvolutionAlgorithmFactory
{
    /// <summary>
    /// Create a new instance of <see cref="NeatEvolutionAlgorithm{T}"/> for the given neat experiment, and neat
    /// population.
    /// </summary>
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    /// <param name="neatExperiment">A neat experiment instance; this conveys everything required to create a new
    /// evolution algorithm instance that is ready to be run.</param>
    /// <param name="neatPop">A pre constructed/loaded neat population; this must be compatible with the provided
    /// neat experiment, otherwise an exception will be thrown.</param>
    /// <returns>A new instance of <see cref="NeatEvolutionAlgorithm{T}"/>.</returns>
    public static NeatEvolutionAlgorithm<TScalar> CreateEvolutionAlgorithm<TScalar>(
        INeatExperiment<TScalar> neatExperiment,
        NeatPopulation<TScalar> neatPop)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Validate MetaNeatGenome and NeatExperiment are compatible; normally the former should have been created
        // based on the latter, but this is not enforced.
        MetaNeatGenome<TScalar> metaNeatGenome = neatPop.MetaNeatGenome;
        ValidateCompatible(neatExperiment, metaNeatGenome);

        var ea = CreateEvolutionAlgorithmInner(
            neatExperiment, neatPop);

        return ea;
    }

    /// <summary>
    /// Create a new instance of <see cref="NeatEvolutionAlgorithm{T}"/> for the given neat experiment.
    /// </summary>
    /// <param name="neatExperiment">A neat experiment instance; this conveys everything required to create a new
    /// evolution algorithm instance that is ready to be run.</param>
    /// <returns>A new instance of <see cref="NeatEvolutionAlgorithm{T}"/>.</returns>
    public static NeatEvolutionAlgorithm<TScalar> CreateEvolutionAlgorithm<TScalar>(
        INeatExperiment<TScalar> neatExperiment)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        var metaNeatGenome = neatExperiment.CreateMetaNeatGenome();

        // Create an initial population of genomes.
        NeatPopulation<TScalar> neatPop = NeatPopulationFactory<TScalar>.CreatePopulation(
            metaNeatGenome,
            connectionsProportion: neatExperiment.InitialInterconnectionsProportion,
            popSize: neatExperiment.PopulationSize);

        var ea = CreateEvolutionAlgorithmInner<TScalar>(
            neatExperiment, neatPop);

        return ea;
    }

    #region Private Static Methods

    private static NeatEvolutionAlgorithm<TScalar> CreateEvolutionAlgorithmInner<TScalar>(
            INeatExperiment<TScalar> neatExperiment,
            NeatPopulation<TScalar> neatPop)
            where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Create a genomeList evaluator based on the experiment's configuration settings.
        var genomeBatchEvaluator = CreateGenomeBatchEvaluator(neatExperiment);

        // Create a speciation strategy based on the experiment's configuration settings.
        var speciationStrategy = CreateSpeciationStrategy<TScalar>(neatExperiment);

        // Create an instance of the default connection weight mutation scheme.
        var weightMutationScheme = WeightMutationSchemeFactory.CreateDefaultScheme<TScalar>(
            neatExperiment.ConnectionWeightScale);

        // Pull all of the parts together into an evolution algorithm instance.
        var ea = new NeatEvolutionAlgorithm<TScalar>(
            neatExperiment.EvolutionAlgorithmSettings,
            genomeBatchEvaluator,
            speciationStrategy,
            neatPop,
            neatExperiment.ComplexityRegulationStrategy,
            neatExperiment.AsexualReproductionSettings,
            neatExperiment.RecombinationSettings,
            weightMutationScheme);

        return ea;
    }

    // TODO: Creation of an IGenomeBatchEvaluator needs to be the responsibility of INeatExperimentFactory (or the evaluation scheme),
    // to allow for tasks that require the entire population to be evaluated as a whole, e.g. simulated life/worlds.
    // Furthermore, a new interface IPhenomeBatchEvaluator will be needed to allow the code for those types of task to be abstracted away from the type of genome in use.
    private static IGenomeBatchEvaluator<NeatGenome<TScalar>> CreateGenomeBatchEvaluator<TScalar>(
        INeatExperiment<TScalar> neatExperiment)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Create a genome decoder based on experiment config settings.
        var genomeDecoder =
            NeatGenomeDecoderFactory.CreateGenomeDecoder<TScalar>(
                neatExperiment.IsAcyclic,
                neatExperiment.EnableHardwareAcceleratedNeuralNets);

        // Resolve degreeOfParallelism (-1 is allowed in config, but must be resolved here to an actual degree).
        int degreeOfParallelismResolved = ResolveDegreeOfParallelism(neatExperiment.DegreeOfParallelism);

        // Create a genomeList evaluator, and return.
        var genomeBatchEvaluator = GenomeBatchEvaluatorFactory.CreateEvaluator<NeatGenome<TScalar>,IBlackBox<TScalar>>(
            genomeDecoder,
            neatExperiment.EvaluationScheme,
            degreeOfParallelismResolved);

        return genomeBatchEvaluator;
    }

    private static ISpeciationStrategy<NeatGenome<TScalar>, TScalar> CreateSpeciationStrategy<TScalar>(
        INeatExperiment<TScalar> neatExperiment)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Resolve a degreeOfParallelism (-1 is allowed in config, but must be resolved here to an actual degree).
        int degreeOfParallelismResolved = ResolveDegreeOfParallelism(neatExperiment.DegreeOfParallelism);

        // Define a distance metric to use for k-means speciation; this is the default from sharpneat 2.x.
        var distanceMetric = new ManhattanDistanceMetric<TScalar>(1.0, 0.0, 10.0);

        // Use k-means speciation strategy; this is the default from sharpneat 2.x.
        // Create a serial (single threaded) strategy if degreeOfParallelism is one.
        if (degreeOfParallelismResolved == 1)
            return new Speciation.GeneticKMeans.GeneticKMeansSpeciationStrategy<TScalar>(distanceMetric, 5);

        // Create a parallel (multi-threaded) strategy for degreeOfParallelism > 1.
        return new Speciation.GeneticKMeans.Parallelized.GeneticKMeansSpeciationStrategy<TScalar>(distanceMetric, 5, degreeOfParallelismResolved);
    }

    #endregion

    #region Private Static Methods [Low Level Helper Methods]

    private static void ValidateCompatible<TScalar>(
        INeatExperiment<TScalar> neatExperiment,
        MetaNeatGenome<TScalar> metaNeatGenome)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Confirm that neatExperiment and metaNeatGenome are compatible with each other.
        if (neatExperiment.EvaluationScheme.InputCount != metaNeatGenome.InputNodeCount)
            throw new ArgumentException("InputNodeCount does not match INeatExperiment.", nameof(metaNeatGenome));

        if (neatExperiment.EvaluationScheme.OutputCount != metaNeatGenome.OutputNodeCount)
            throw new ArgumentException("OutputNodeCount does not match INeatExperiment.", nameof(metaNeatGenome));

        if (neatExperiment.IsAcyclic != metaNeatGenome.IsAcyclic)
            throw new ArgumentException("IsAcyclic does not match INeatExperiment.", nameof(metaNeatGenome));

        if (neatExperiment.ConnectionWeightScale != metaNeatGenome.ConnectionWeightScale)
            throw new ArgumentException("ConnectionWeightScale does not match INeatExperiment.", nameof(metaNeatGenome));

        // Note. neatExperiment.ActivationFnName is not being checked against metaNeatGenome.ActivationFn, as the
        // name information is not present on the ActivationFn object.
    }

    private static int ResolveDegreeOfParallelism(
        int configuredDegreeOfParallelism)
    {
        // Resolve special value of -1 to the number of logical CPU cores.
        if(configuredDegreeOfParallelism == -1)
            return Environment.ProcessorCount;
        else ArgumentOutOfRangeException.ThrowIfLessThan(configuredDegreeOfParallelism, 1);

        return configuredDegreeOfParallelism;
    }

    #endregion
}
