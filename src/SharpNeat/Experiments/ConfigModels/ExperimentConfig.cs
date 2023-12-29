// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Reproduction.Asexual;
using SharpNeat.Neat.Reproduction.Recombination;

namespace SharpNeat.Experiments.ConfigModels;

/// <summary>
/// Model type for NEAT experiment configuration.
/// </summary>
public record ExperimentConfig
{
    /// <summary>
    /// A unique human-readable ID associated with the experiment.
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// Experiment name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Experiment description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// A boolean flag that indicates if the genomes that are evolved are acyclic,
    /// i.e. they should have no recurrent/cyclic connection paths.
    /// </summary>
    public bool? IsAcyclic { get; init; }

    /// <summary>
    /// For cyclic neural networks (i.e. if <see cref="IsAcyclic"/> is false) this defines how many timesteps to
    /// run the neural net per call to Activate().
    /// </summary>
    public int? CyclesPerActivation { get; init; }

    /// <summary>
    /// Name of the neuron activation function to use in evolved networks.
    /// </summary>
    public string? ActivationFnName { get; init; }

    /// <summary>
    /// The population size to use for the experiment.
    /// </summary>
    public int? PopulationSize { get; init; }

    /// <summary>
    /// The initial interconnections proportion. This is the proportion of possible
    /// direct connections from the input nodes to the output nodes that are to be created in
    /// each initial/seed genome. The connections to create are selected at random (using a
    /// select-without-replacement method).
    /// </summary>
    public double? InitialInterconnectionsProportion { get; init; }

    /// <summary>
    /// The maximum connection weight scale/magnitude.
    /// E.g. a value of 5 defines a weight range of -5 to 5.
    /// The weight range is strictly enforced, e.g. when creating new connections and mutating existing ones.
    /// </summary>
    public double? ConnectionWeightScale { get; init; }

    /// <summary>
    /// The number of CPU threads to distribute work to.
    /// Set to -1 to use a thread count that matches the number of logical CPU cores.
    /// </summary>
    public int? DegreeOfParallelism { get; init; }

    /// <summary>
    /// Enable use of hardware accelerated neural network implementations, i.e. alternative implementations that use
    /// CPU SIMD/vector instructions.
    /// </summary>
    /// <remarks>
    /// The vectorized code is provided by alternative classes, and these classes tend to be more complex than their
    /// 'baseline' non-vectorized equivalents. Therefore when debugging a problem it is often useful to disable use
    /// of all vectorized code in order to rule out that code as the source of a problem/bug.
    ///
    /// Furthermore, enabling hardware acceleration has been observed to often result in slower execution speed,
    /// probably because NEAT deals with non-homogeneous, irregular neural network structures that are generally not
    /// conducive to the application of vectorized code.
    /// </remarks>
    public bool? EnableHardwareAcceleratedNeuralNets { get; init; }

    /// <summary>
    /// Enable use of hardware accelerated neural network activation functions, i.e. alternative implementations that use
    /// CPU SIMD/vector instructions.
    /// </summary>
    /// <remarks>
    /// The vectorized code is provided by alternative classes, and these classes tend to be more complex than their
    /// 'baseline' non-vectorized equivalents. Therefore when debugging a problem it is often useful to disable use
    /// of all vectorized code in order to rule out that code as the source of a problem/bug.
    ///
    /// Furthermore, enabling hardware acceleration has been observed to often result in slower execution speed,
    /// probably because NEAT deals with non-homogeneous, irregular neural network structures that are generally not
    /// conducive to the application of vectorized code.
    /// </remarks>
    public bool? EnableHardwareAcceleratedActivationFunctions { get; init; }

    /// <summary>
    /// NEAT evolution algorithm configuration.
    /// </summary>
    public NeatEvolutionAlgorithmSettings? EvolutionAlgorithm { get; init; }

    /// <summary>
    /// NEAT asexual reproduction configuration.
    /// </summary>
    public NeatAsexualReproductionSettings? AsexualReproduction { get; init; }

    /// <summary>
    /// NEAT recombination configuration.
    /// </summary>
    public NeatRecombinationSettings? Recombination { get; init; }

    /// <summary>
    /// Complexity regulation strategy configuration.
    /// </summary>
    public ComplexityRegulationStrategyConfig? ComplexityRegulationStrategy { get; init; }
}
