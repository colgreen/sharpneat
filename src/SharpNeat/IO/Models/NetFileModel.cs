// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.IO.Models;

/// <summary>
/// An object model that represents instances of the 'net' file format.
/// </summary>
public class NetFileModel
{
    /// <summary>
    /// Input node count.
    /// </summary>
    public int InputCount { get; }

    /// <summary>
    /// Output node count.
    /// </summary>
    public int OutputCount { get; }

    /// <summary>
    /// Indicates of the graph/network is acyclic.
    /// </summary>
    public bool IsAcyclic { get; }

    /// <summary>
    /// For cyclic networks, this specifies the number of timesteps are run per activation of the network. Not
    /// used when <see cref="IsAcyclic"/> is true.
    /// </summary>
    public int CyclesPerActivation { get; }

    /// <summary>
    /// A list of sourceId-targetId-weight tuples, that together describe the graph/network.
    /// </summary>
    public List<ConnectionLine> Connections { get; }

    /// <summary>
    /// A list of activations functions.
    /// </summary>
    /// <remarks>
    /// Each line contains an integer ID and a code (e.g. 'ReLU').
    /// The IDs are integers and are always defined in a continuous sequence starting at zero.
    /// Currently only one line is allowed, and this describes the activation function to use for all nodes in the
    /// network.
    /// </remarks>
    public List<ActivationFnLine> ActivationFns { get; }

    /// <summary>
    /// Initialises a new instance of <see cref="NetFileModel"/>.
    /// </summary>
    /// <param name="inputCount">Input node count.</param>
    /// <param name="outputCount">Output node count.</param>
    /// <param name="isAcyclic">Indicates of the graph/network is acyclic.</param>
    /// <param name="cyclesPerActivation">For cyclic networks, this specifies the number of timesteps are run per activation of the network.</param>
    /// <param name="connList">A list of sourceId-targetId-weight tuples, that together describe the graph/network.</param>
    /// <param name="activationFns">A list of activations functions.</param>
    internal NetFileModel(
        int inputCount, int outputCount,
        bool isAcyclic,
        int cyclesPerActivation,
        List<ConnectionLine> connList,
        List<ActivationFnLine> activationFns)
    {
        // Note. Zero input nodes is allowed, but zero output nodes is nonsensical.
        if(inputCount < 0) throw new ArgumentOutOfRangeException(nameof(inputCount));
        if(outputCount < 1) throw new ArgumentOutOfRangeException(nameof(outputCount));
        if(!isAcyclic && cyclesPerActivation < 1) throw new ArgumentOutOfRangeException(nameof(cyclesPerActivation));

        InputCount = inputCount;
        OutputCount = outputCount;
        IsAcyclic = isAcyclic;
        CyclesPerActivation = cyclesPerActivation;
        Connections = connList ?? throw new ArgumentNullException(nameof(connList));
        ActivationFns = activationFns ?? throw new ArgumentNullException(nameof(activationFns));

        if (activationFns.Count == 0)
            throw new ArgumentException("Collection cannot be empty.", nameof(activationFns));

        if (activationFns[0].Id != 0)
            throw new ArgumentException("The first activation function must have an ID of 0.", nameof(activationFns));
    }

    /// <summary>
    /// Create a new instance of <see cref="NetFileModel"/>, representing an acyclic network.
    /// </summary>
    /// <param name="inputCount">Input node count.</param>
    /// <param name="outputCount">Output node count.</param>
    /// <param name="connList">A list of sourceId-targetId-weight tuples, that together describe the graph/network.</param>
    /// <param name="activationFns">A list of activations functions.</param>
    /// <returns>A new instance of <see cref="NetFileModel"/>.</returns>
    public static NetFileModel CreateAcyclic(
        int inputCount, int outputCount,
        List<ConnectionLine> connList,
        List<ActivationFnLine> activationFns)
    {
        return new NetFileModel(
            inputCount, outputCount,
            true, 0,
            connList, activationFns);
    }

    /// <summary>
    /// Create a new instance of <see cref="NetFileModel"/>, representing an cyclic network.
    /// </summary>
    /// <param name="inputCount">Input node count.</param>
    /// <param name="outputCount">Output node count.</param>
    /// <param name="cyclesPerActivation">For cyclic networks, this specifies the number of timesteps are run per activation of the network.</param>
    /// <param name="connList">A list of sourceId-targetId-weight tuples, that together describe the graph/network.</param>
    /// <param name="activationFns">A list of activations functions.</param>
    /// <returns>A new instance of <see cref="NetFileModel"/>.</returns>
    public static NetFileModel CreateCyclic(
        int inputCount, int outputCount,
        int cyclesPerActivation,
        List<ConnectionLine> connList,
        List<ActivationFnLine> activationFns)
    {
        return new NetFileModel(
            inputCount, outputCount,
            true, cyclesPerActivation,
            connList, activationFns);
    }
}
