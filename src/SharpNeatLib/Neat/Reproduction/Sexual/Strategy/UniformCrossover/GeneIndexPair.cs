using System.Diagnostics;

namespace SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover
{
    struct GeneIndexPair
    {
        // Note. And index of -1 indicates no match on a genome or a given gene.
        public readonly int Idx1;
        public readonly int Idx2;

        public GeneIndexPair(int idx1, int idx2)
        {
            Debug.Assert(idx1 >= 0 || idx2 >=0);

            this.Idx1 = idx1;
            this.Idx2 = idx2;
        }
    }
}
