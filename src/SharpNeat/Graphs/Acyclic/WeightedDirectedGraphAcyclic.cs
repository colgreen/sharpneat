// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Graphs.Acyclic;

/// <summary>
/// Represents a weighted acyclic directed graph.
/// </summary>
/// <typeparam name="T">Weight data type.</typeparam>
public class WeightedDirectedGraphAcyclic<T> : DirectedGraphAcyclic
    where T : struct
{
    /// <summary>
    /// Construct with the given node counts, connection data, layer information, indexes of the output nodes,
    /// and connection weights.
    /// </summary>
    /// <param name="inputCount">Input node count.</param>
    /// <param name="outputCount">Output node count.</param>
    /// <param name="totalNodeCount">Total node count.</param>
    /// <param name="connIds">The connection source and target node IDs.</param>
    /// <param name="layerArr">Layer information for the acyclic graph.</param>
    /// <param name="outputNodeIdxArr">An array containing the node index of each output node.</param>
    /// <param name="weightArr">Connection weights array.</param>
    internal WeightedDirectedGraphAcyclic(
        int inputCount,
        int outputCount,
        int totalNodeCount,
        in ConnectionIds connIds,
        LayerInfo[] layerArr,
        int[] outputNodeIdxArr,
        T[] weightArr)
    : base(inputCount, outputCount, totalNodeCount, in connIds, layerArr, outputNodeIdxArr)
    {
        WeightArray = weightArr;
    }

    /// <summary>
    /// Connection weight array.
    /// </summary>
    public T[] WeightArray { get; }
}
