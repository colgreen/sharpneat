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
    /// <param name="connList">A list of sourceId-targetId-weight tuples, that together describe the graph/network.</param>
    /// <param name="activationFns">A list of activations functions.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="inputCount"/> is less than zero.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="outputCount"/> is less than one.</exception>
    public NetFileModel(
        int inputCount, int outputCount,
        bool isAcyclic,
        List<ConnectionLine> connList,
        List<ActivationFnLine> activationFns)
    {
        // Note. Zero input nodes is allowed, but zero output nodes is nonsensical.
        if(inputCount < 0) throw new ArgumentOutOfRangeException(nameof(inputCount));
        if(outputCount < 1) throw new ArgumentOutOfRangeException(nameof(outputCount));

        InputCount = inputCount;
        OutputCount = outputCount;
        IsAcyclic = isAcyclic;
        Connections = connList ?? throw new ArgumentNullException(nameof(connList));
        ActivationFns = activationFns ?? throw new ArgumentNullException(nameof(activationFns));
    }
}
