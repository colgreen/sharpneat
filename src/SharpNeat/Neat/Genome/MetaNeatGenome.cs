// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.NeuralNets;

namespace SharpNeat.Neat.Genome;

/// <summary>
/// NeatGenome metadata.
/// Genome related values/settings that are consistent across all genomes for the lifetime of an evolutionary algorithm run.
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
public class MetaNeatGenome<TScalar>
    where TScalar : struct
{
    /// <summary>
    /// Input node count.
    /// </summary>
    public int InputNodeCount { get; }

    /// <summary>
    /// Output node count.
    /// </summary>
    public int OutputNodeCount { get; }

    /// <summary>
    /// Indicates if the genomes that are evolved are acyclic, i.e. they should have no recurrent/cyclic
    /// connection paths.
    /// </summary>
    public bool IsAcyclic { get; }

    /// <summary>
    /// For cyclic neural networks (i.e. if <see cref="IsAcyclic"/> = false) this defines how many timesteps to
    /// run the neural net per call to Activate().
    /// </summary>
    public int CyclesPerActivation { get; }

    /// <summary>
    /// The neuron activation function to use in evolved networks. NEAT uses the same activation
    /// function at each node.
    /// </summary>
    public IActivationFunction<TScalar> ActivationFn { get; }

    /// <summary>
    /// Maximum connection weight scale/magnitude.
    /// E.g. a value of 5 defines a weight range of -5 to 5.
    /// The weight range is strictly enforced, e.g. when creating new connections and mutating existing ones.
    /// </summary>
    public double ConnectionWeightScale { get; }

    /// <summary>
    /// The total number of input and output nodes.
    /// </summary>
    public int InputOutputNodeCount => InputNodeCount + OutputNodeCount;

    #region Construction

    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="inputNodeCount">Input node count.</param>
    /// <param name="outputNodeCount">Output node count.</param>
    /// <param name="isAcyclic">Indicates if the genomes that are evolved are acyclic, i.e. they should have no
    /// recurrent/cyclic connection paths.</param>
    /// <param name="cyclesPerActivation">For cyclic neural networks (i.e. if <see cref="IsAcyclic"/> = false)
    /// this defines how many timesteps to run the neural net per call to Activate().</param>
    /// <param name="activationFn">The neuron activation function to use in evolved networks. NEAT uses the same
    /// activation function at each node.</param>
    /// <param name="connectionWeightScale">Maximum connection weight scale/magnitude.</param>
    internal MetaNeatGenome(
        int inputNodeCount, int outputNodeCount,
        bool isAcyclic, int cyclesPerActivation,
        IActivationFunction<TScalar> activationFn,
        double connectionWeightScale = 5.0)
    {
        // Note. Zero input nodes is allowed, but zero output nodes is nonsensical.
        ArgumentOutOfRangeException.ThrowIfNegative(inputNodeCount);
        ArgumentOutOfRangeException.ThrowIfLessThan(outputNodeCount, 1);
        if(!isAcyclic && cyclesPerActivation < 1) throw new ArgumentOutOfRangeException(nameof(cyclesPerActivation));

        InputNodeCount = inputNodeCount;
        OutputNodeCount = outputNodeCount;
        IsAcyclic = isAcyclic;
        CyclesPerActivation = cyclesPerActivation;
        ActivationFn = activationFn;
        ConnectionWeightScale = connectionWeightScale;
    }

    #endregion

    #region Static Factory Methods

    /// <summary>
    /// Create a new instance of <see cref="MetaNeatGenome{T}"/>, with <see cref="IsAcyclic"/> set to true, i.e.,
    /// for evolving acyclic neural networks.
    /// </summary>
    /// <param name="inputNodeCount">Input node count.</param>
    /// <param name="outputNodeCount">Output node count.</param>
    /// <param name="activationFn">The neuron activation function to use in evolved networks. NEAT uses the same
    /// activation function at each node.</param>
    /// <param name="connectionWeightScale">Maximum connection weight scale/magnitude.</param>
    /// <returns>A new instance of <see cref="MetaNeatGenome{T}"/>.</returns>
    public static MetaNeatGenome<TScalar> CreateAcyclic(
        int inputNodeCount, int outputNodeCount,
        IActivationFunction<TScalar> activationFn,
        double connectionWeightScale = 5.0)
    {
        return new MetaNeatGenome<TScalar>(
            inputNodeCount, outputNodeCount,
            true, 0,
            activationFn,
            connectionWeightScale);
    }

    /// <summary>
    /// Create a new instance of <see cref="MetaNeatGenome{T}"/>, with <see cref="IsAcyclic"/> set to false, i.e.,
    /// for evolving cyclic neural networks.
    /// </summary>
    /// <param name="inputNodeCount">Input node count.</param>
    /// <param name="outputNodeCount">Output node count.</param>
    /// <param name="cyclesPerActivation">For cyclic neural networks (i.e. if <see cref="IsAcyclic"/> = false)
    /// this defines how many timesteps to run the neural net per call to Activate().</param>
    /// <param name="activationFn">The neuron activation function to use in evolved networks. NEAT uses the same
    /// activation function at each node.</param>
    /// <param name="connectionWeightScale">Maximum connection weight scale/magnitude.</param>
    /// <returns>A new instance of <see cref="MetaNeatGenome{T}"/>.</returns>
    public static MetaNeatGenome<TScalar> CreateCyclic(
        int inputNodeCount, int outputNodeCount,
        int cyclesPerActivation,
        IActivationFunction<TScalar> activationFn,
        double connectionWeightScale = 5.0)
    {
        return new MetaNeatGenome<TScalar>(
            inputNodeCount, outputNodeCount,
            false, cyclesPerActivation,
            activationFn,
            connectionWeightScale);
    }

    #endregion
}
