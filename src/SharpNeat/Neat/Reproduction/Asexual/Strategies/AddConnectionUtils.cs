// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Neat.Reproduction.Asexual.Strategies;

/// <summary>
/// Static utility methods related to the adding connections to a genome. I.e., am 'add connection' mutation.
/// </summary>
internal static class AddConnectionUtils
{
    /// <summary>
    /// Gets the ID of the node with the specified node index.
    /// </summary>
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    /// <param name="parent">Parent genome.</param>
    /// <param name="idx">Node index.</param>
    /// <returns>The ID of the node with the specified node index.</returns>
    public static int GetNodeIdFromIndex<TScalar>(NeatGenome<TScalar> parent, int idx)
        where TScalar : unmanaged
    {
        // For input/output nodes their index is their ID.
        if(idx < parent.MetaNeatGenome.InputOutputNodeCount)
            return idx;

        // All other nodes are hidden nodes; use a pre-built array of all hidden node IDs.
        return parent.HiddenNodeIdArray[idx - parent.MetaNeatGenome.InputOutputNodeCount];
    }
}
