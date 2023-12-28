using Redzen;
using Xunit;

namespace SharpNeat.Neat.Reproduction.Sexual.Strategy.UniformCrossover;

public class EnumerateParentGenesTests
{
    [Fact]
    public void EnumerateParentGenes_CompareWithSelf()
    {
        NeatPopulation<double> pop = UniformCrossoverReproductionStrategyTestsUtils.CreateNeatPopulation(1);
        var genome = pop.GenomeList[0];

        (int,int)[] geneIndexPairArr = UniformCrossoverRecombinationStrategyUtils.EnumerateParentGenes(genome.ConnectionGenes, genome.ConnectionGenes).ToArray();

        (int,int)[] expectedArr = [
            (0,0), (1,1), (2,2),
            (3,3), (4,4), (5,5),
            (6,6), (7,7), (8,8),
            (9,9), (10,10), (11,11) ];

        Assert.True(SpanUtils.Equal<(int,int)>(expectedArr, geneIndexPairArr));
    }

    [Fact]
    public void EnumerateParentGenes_ExcessGeneInParent1()
    {
        NeatPopulation<double> pop = UniformCrossoverReproductionStrategyTestsUtils.CreateNeatPopulation(2);
        var genome1 = pop.GenomeList[0];
        var genome2 = pop.GenomeList[1];

        (int,int)[] geneIndexPairArr = UniformCrossoverRecombinationStrategyUtils.EnumerateParentGenes(genome1.ConnectionGenes, genome2.ConnectionGenes).ToArray();

        (int,int)[] expectedArr = [
            (0,0), (1,1), (2,2),
            (3,3), (4,4), (5,-1),
            (6,5), (7,6), (8,7),
            (9,8), (10,9), (11,10) ];

        Assert.True(SpanUtils.Equal<(int,int)>(expectedArr, geneIndexPairArr));
    }

    [Fact]
    public void EnumerateParentGenes_ExcessGeneInParent2()
    {
        NeatPopulation<double> pop = UniformCrossoverReproductionStrategyTestsUtils.CreateNeatPopulation(2);
        var genome1 = pop.GenomeList[1];
        var genome2 = pop.GenomeList[0];

        (int,int)[] geneIndexPairArr = UniformCrossoverRecombinationStrategyUtils.EnumerateParentGenes(genome1.ConnectionGenes, genome2.ConnectionGenes).ToArray();

        (int,int)[] expectedArr = [
            (0,0), (1,1), (2,2),
            (3,3), (4,4), (-1,5),
            (5,6), (6,7), (7,8),
            (8,9), (9,10), (10,11) ];

        Assert.True(SpanUtils.Equal<(int,int)>(expectedArr, geneIndexPairArr));
    }
}
