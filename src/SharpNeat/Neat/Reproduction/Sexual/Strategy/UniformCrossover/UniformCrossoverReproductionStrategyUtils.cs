// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Graphs;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover;

/// <summary>
/// Static utility methods for the uniform crossover reproduction strategy.
/// </summary>
internal class UniformCrossoverReproductionStrategyUtils
{
    /// <summary>
    /// Performs a 'parallel walk' over the connection genes of parent1 and parent2.
    /// </summary>
    /// <typeparam name="T">Connection weight data type.</typeparam>
    /// <param name="parent1">Parent 1.</param>
    /// <param name="parent2">Parent 2.</param>
    /// <returns>An enumerable over indexes into parent1 and parent2's connection genes.</returns>
    public static IEnumerable<ValueTuple<int,int>> EnumerateParentGenes<T>(ConnectionGenes<T> parent1, ConnectionGenes<T> parent2)
        where T : struct
    {
        // Special case. Empty gene arrays.
        // Note. In NEAT the genomes should always have at least one connection/gene,
        // but we perform the test anyway for completeness.
        if(parent1.Length ==  0 && parent2.Length == 0)
            yield break;

        // Handle scenarios where one of the ID arrays is empty.
        if(parent1.Length == 0)
        {
            for(int i=0; i < parent2.Length; i++)
                yield return ValueTuple.Create(-1, i);

            yield break;
        }

        if(parent2.Length == 0)
        {
            for(int i=0; i < parent1.Length; i++)
                yield return ValueTuple.Create(i, -1);

            yield break;
        }

        // Both arrays are non-empty; compare their contents.
        int idx1 = 0;
        int idx2 = 0;

        for(;;)
        {
            DirectedConnection conn1 = parent1._connArr[idx1];
            DirectedConnection conn2 = parent2._connArr[idx2];

            if(conn2 < conn1)
            {
                // conn2 is disjoint.
                yield return ValueTuple.Create(-1, idx2);

                // Move to the next element in idArr2.
                idx2++;
            }
            else if(conn1 == conn2)
            {
                // Matching connections.
                yield return ValueTuple.Create(idx1, idx2);

                // Move to the next elements in idArr1, idArr2.
                idx1++;
                idx2++;
            }
            else // (id2 > id1)
            {
                // id1 is disjoint.
                yield return ValueTuple.Create(idx1, -1);

                // Move to the next element in idArr1.
                idx1++;
            }

            // Check if we have reached the end of one (or both) of the lists. If we have reached the end of both then
            // although we enter the first 'if' block it doesn't matter because the contained loop is not entered if both
            // lists have been exhausted.
            if(parent1.Length == idx1)
            {
                // All remaining list2 genes are excess.
                for(; idx2 < parent2.Length; idx2++)
                    yield return ValueTuple.Create(-1, idx2);

                yield break;
            }

            if(parent2.Length == idx2)
            {
                // All remaining list1 genes are excess.
                for(; idx1 < parent1.Length; idx1++)
                    yield return ValueTuple.Create(idx1, -1);

                yield break;
            }
        }
    }
}
