// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Neat.ComplexityRegulation;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Reproduction.Asexual;
using SharpNeat.Neat.Reproduction.Sexual;

namespace SharpNeat.Experiments;

/// <summary>
/// An interface that brings together a number of settings objects that make up a given experiment.
/// </summary>
/// <typeparam name="TScalar">Black box input/output data type.</typeparam>
public interface INeatExperiment<TScalar>
    where TScalar : struct
{
    /// <summary>
    /// Matches <see cref="INeatExperimentFactory.Id"/> from the experiment factory that created the current experiment instance.
    /// </summary>
    /// <remarks>
    /// It is possible to create <see cref="INeatExperiment{T}"/> without using an <see cref="INeatExperimentFactory"/>, in those cases this
    /// property can be set to null.
    /// </remarks>
    string FactoryId { get; }

    /// <summary>
    /// A unique human-readable ID associated with the experiment.
    /// </summary>
    /// <remarks>
    /// This will often match <see cref="FactoryId"/>, but there could be two or more experiments that are created from the same
    /// <see cref="INeatExperimentFactory"/> , and differ only by their configuration settings.
    /// </remarks>
    string Id { get; set; }

    /// <summary>
    /// Experiment name.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Experiment description.
    /// </summary>
    string Description { get; set; }

    /// <summary>
    /// Experiment evaluation scheme object.
    /// </summary>
    IBlackBoxEvaluationScheme<TScalar> EvaluationScheme { get; }

    /// <summary>
    /// A boolean flag that indicates if the genomes that are evolved are acyclic,
    /// i.e. they should have no recurrent/cyclic connection paths.
    /// </summary>
    bool IsAcyclic { get; set; }

    /// <summary>
    /// For cyclic neural networks (i.e. if <see cref="IsAcyclic"/> is false) this defines how many timesteps to run the neural net per call to Activate().
    /// </summary>
    int CyclesPerActivation { get; set; }

    /// <summary>
    /// Name of the neuron activation function to use in evolved networks.
    /// </summary>
    string ActivationFnName { get; set; }

    /// <summary>
    /// The <see cref="EvolutionAlgorithmSettings"/> to be used for the experiment.
    /// </summary>
    NeatEvolutionAlgorithmSettings EvolutionAlgorithmSettings { get; }

    /// <summary>
    /// The asexual reproduction settings to use for the experiment.
    /// </summary>
    NeatReproductionAsexualSettings ReproductionAsexualSettings { get; }

    /// <summary>
    /// The sexual reproduction settings to use for the experiment.
    /// </summary>
    NeatReproductionSexualSettings ReproductionSexualSettings { get; }

    /// <summary>
    /// The population size to use for the experiment.
    /// </summary>
    int PopulationSize { get; set; }

    /// <summary>
    /// The initial interconnections proportion. This is the proportion of possible
    /// direct connections from the input nodes to the output nodes that are to be created in
    /// each initial genome. The connections to create are selected at random (using a
    /// select-without-replacement method).
    /// </summary>
    double InitialInterconnectionsProportion { get; set; }

    /// <summary>
    /// The maximum connection weight scale/magnitude.
    /// E.g. a value of 5 defines a weight range of -5 to 5.
    /// The weight range is strictly enforced, e.g. when creating new connections and mutating existing ones.
    /// </summary>
    double ConnectionWeightScale { get; set; }

    /// <summary>
    /// The complexity regulation strategy to use for the experiment.
    /// </summary>
    IComplexityRegulationStrategy ComplexityRegulationStrategy { get; set; }

    /// <summary>
    /// The number of CPU threads to distribute work to.
    /// Set to -1 to use a thread count that matches the number of logical CPU cores.
    /// </summary>
    int DegreeOfParallelism { get; set; }

    /// <summary>
    /// Enable use of hardware accelerated neural network implementations, i.e. alternate implementations that use
    /// CPU SIMD/vector instructions.
    /// </summary>
    /// <remarks>
    /// The vectorized code is provided by alternative classes, and these classes tend to be more complex than their
    /// 'baseline' non-vectorized equivalents. Therefore when debugging a problem it is often useful to disable use
    /// of all vectorized code so as to rule out that code as the source of a problem/bug.
    ///
    /// Furthermore, enabling hardware acceleration has been observed to often result in slower execution speed,
    /// probably because NEAT deals with non-homogeneous, irregular neural network structures that are generally not
    /// conducive to the application of vectorized code.
    /// </remarks>
    bool EnableHardwareAcceleratedNeuralNets { get; set; }

    /// <summary>
    /// Enable use of hardware accelerated neural network activation functions, i.e. alternate implementations that use
    /// CPU SIMD/vector instructions.
    /// </summary>
    /// <remarks>
    /// The vectorized code is provided by alternative classes, and these classes tend to be more complex than their
    /// 'baseline' non-vectorized equivalents. Therefore when debugging a problem it is often useful to disable use
    /// of all vectorized code so as to rule out that code as the source of a problem/bug.
    ///
    /// Furthermore, enabling hardware acceleration has been observed to often result in slower execution speed,
    /// probably because NEAT deals with non-homogeneous, irregular neural network structures that are generally not
    /// conducive to the application of vectorized code.
    /// </remarks>
    bool EnableHardwareAcceleratedActivationFunctions { get; set; }
}
