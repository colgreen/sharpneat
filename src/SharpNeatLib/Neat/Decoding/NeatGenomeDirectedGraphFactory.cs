using System;
using System.Collections.Generic;
using System.Linq;
using SharpNeat.Neat.Genome;
using SharpNeat.Network2;

namespace SharpNeat.Neat.Decoding
{
    public static class NeatGenomeDirectedGraphFactory
    {
        public static DirectedGraph Create(NeatGenome neatGenome)
        {
            // Define an enumeration over the input and output nodes IDs.
            MetaNeatGenome meta = neatGenome.MetaNeatGenome;
            int ioNodeCount = meta.InputNodeCount + meta.OutputNodeCount;
            IEnumerable<int> ioNodeIds = Enumerable.Range(0, ioNodeCount);

            // Invoke the general purpose digraph factory.
            return DirectedGraphFactory.Create(neatGenome.ConnectionGeneList, ioNodeIds);
        }
    }
}
