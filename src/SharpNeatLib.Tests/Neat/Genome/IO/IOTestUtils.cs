using SharpNeat.Neat.Genome;

namespace SharpNeat.Tests.Neat.Genome.IO
{
    public static class IOTestUtils
    {
        public static void CompareGenomes(NeatGenome<double> x, NeatGenome<double> y)
        {
            // Compare connections.
            var xGenes = x.ConnectionGenes;
            var yGenes = y.ConnectionGenes;
            TestUtils.Compare(xGenes._connArr, yGenes._connArr);
            TestUtils.Compare(xGenes._weightArr, yGenes._weightArr);

            // Compare hidden node ID arrays.
            TestUtils.Compare(x.HiddenNodeIdArray, y.HiddenNodeIdArray);
        }
    }
}
