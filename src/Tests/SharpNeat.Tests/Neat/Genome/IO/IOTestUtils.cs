using Xunit;

namespace SharpNeat.Neat.Genome.IO;

public static class IOTestUtils
{
    public static void CompareGenomeLists(IList<NeatGenome<double>> x, IList<NeatGenome<double>> y)
    {
        Assert.Equal(x.Count, y.Count);
        for(int i=0; i < x.Count; i++)
        {
            CompareGenomes(x[i], y[i]);
        }
    }

    public static void CompareGenomes(NeatGenome<double> x, NeatGenome<double> y)
    {
        // Compare connections.
        var xGenes = x.ConnectionGenes;
        var yGenes = y.ConnectionGenes;
        Assert.Equal(xGenes._connArr, yGenes._connArr);
        Assert.Equal(xGenes._weightArr, yGenes._weightArr);

        // Compare hidden node ID arrays.
        Assert.Equal(x.HiddenNodeIdArray, y.HiddenNodeIdArray);
    }
}
