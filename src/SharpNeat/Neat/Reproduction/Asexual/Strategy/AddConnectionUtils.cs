// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Neat.Reproduction.Asexual.Strategy;

/// <summary>
/// Static utility methods related to the adding connections to a genome. I.e., am 'add connection' mutation.
/// </summary>
internal static class AddConnectionUtils
{
    /// <summary>
    /// Gets the ID of the node with the specified node index.
    /// </summary>
    /// <typeparam name="T">Neural net numeric data type.</typeparam>
    /// <param name="parent">Parent genome.</param>
    /// <param name="idx">Node index.</param>
    /// <returns>The ID of the node with the specified node index.</returns>
    public static int GetNodeIdFromIndex<T>(NeatGenome<T> parent, int idx)
        where T : struct
    {
        // For input/output nodes their index is their ID.
        if(idx < parent.MetaNeatGenome.InputOutputNodeCount)
            return idx;

        // All other nodes are hidden nodes; use a pre-built array of all hidden node IDs.
        return parent.HiddenNodeIdArray[idx - parent.MetaNeatGenome.InputOutputNodeCount];
    }
}
