using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Tests.Neat.Genome.IO
{
    public static class IOTestUtils
    {
        public static void CompareGenomeLists(IList<NeatGenome<double>> x, IList<NeatGenome<double>> y)
        {
            Assert.AreEqual(x.Count, y.Count);
            for(int i=0; i < x.Count; i++) {
                CompareGenomes(x[i], y[i]);
            }
        }

        public static void CompareGenomes(NeatGenome<double> x, NeatGenome<double> y)
        {
            // Compare connections.
            var xGenes = x.ConnectionGenes;
            var yGenes = y.ConnectionGenes;
            ArrayTestUtils.Compare(xGenes._connArr, yGenes._connArr);
            ArrayTestUtils.Compare(xGenes._weightArr, yGenes._weightArr);

            // Compare hidden node ID arrays.
            ArrayTestUtils.Compare(x.HiddenNodeIdArray, y.HiddenNodeIdArray);
        }
    }
}
