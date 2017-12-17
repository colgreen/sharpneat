using System.Collections.Generic;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;

namespace SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover
{
    public partial class UniformCrossoverReproductionStrategy<T> : ISexualReproductionStrategy<T>
        where T : struct
    {
        /// <summary>
        /// Static utility methods for the uniform crossover reproduction strategy.
        /// </summary>
        static class Utils
        {
            public static IEnumerable<GeneIndexPair> EnumerateParentGenes(ConnectionGenes<T> parent1, ConnectionGenes<T> parent2)
            {
                // Special case. Empty gene arrays.
                // Note. In NEAT the genomes should always have at least one connection/gene,
                // but we perform the test anyway for completeness.
                if(0 ==  parent1.Length && 0 == parent2.Length) {
                    yield break;
                }

                // Handle scenarios where one of the ID arrays is empty.
                if(0 == parent1.Length) 
                {
                    for(int i=0; i < parent2.Length; i++) {
                        yield return new GeneIndexPair(-1, i);
                    }
                    yield break;
                }

                if(0 == parent2.Length) 
                {
                    for(int i=0; i < parent1.Length; i++) {
                        yield return new GeneIndexPair(i, -1);
                    }
                    yield break;
                }

                // Both arrays are non-empty; compare their contents.
                int idx1 = 0;
                int idx2 = 0;

                for(;;)
                {
                    DirectedConnection connGene1 = new DirectedConnection(parent1._connArr[idx1]);
                    DirectedConnection connGene2 = new DirectedConnection(parent2._connArr[idx2]);

                    if(connGene2 < connGene1)
                    {   
                        // id2 is disjoint.
                        yield return new GeneIndexPair(-1, idx2);

                        // Move to the next element in idArr2.
                        idx2++;
                    }
                    else if(connGene1 == connGene2)
                    {
                        // Matching IDs.
                        yield return new GeneIndexPair(idx1, idx2);

                        // Move to the next elements in idArr1, idArr2.
                        idx1++;
                        idx2++;
                    }
                    else // (id2 > id1)
                    {
                        // id1 is disjoint.
                        yield return new GeneIndexPair(idx1, -1);

                        // Move to the next element in idArr1.
                        idx1++;
                    }

                    // Check if we have reached the end of one (or both) of the lists. If we have reached the end of both then 
                    // although we enter the first 'if' block it doesn't matter because the contained loop is not entered if both 
                    // lists have been exhausted.
                    if(parent1.Length == idx1)
                    {   
                        // All remaining list2 genes are excess.
                        for(; idx2 < parent2.Length; idx2++) {
                            yield return new GeneIndexPair(-1, idx2);
                        }
                        yield break;
                    }

                    if(parent2.Length == idx2)
                    {   
                        // All remaining list1 genes are excess.
                        for(; idx1 < parent1.Length; idx1++) {
                            yield return new GeneIndexPair(idx1, -1);
                        }
                        yield break;
                    }
                }
            }
        }
    }
}
