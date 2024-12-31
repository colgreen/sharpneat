using FluentAssertions;
using SharpNeat.Graphs;
using SharpNeat.IO;
using SharpNeat.IO.Models;
using SharpNeat.NeuralNets.ActivationFunctions;
using Xunit;

namespace SharpNeat.Neat.Genome.IO;

public class NeatGenomeConverterTests
{
    [Fact]
    public void GenomeShouldConvertToNetFileModel()
    {
        // Arrange.
        var metaNeatGenome = MetaNeatGenome<double>.CreateAcyclic(3, 2, new ReLU<double>());
        var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(metaNeatGenome);

        // Simple acyclic graph.
        var connGenes = new ConnectionGenes<double>(6);
        connGenes[0] = (0, 3, 0.123);
        connGenes[1] = (1, 3, 1.234);
        connGenes[2] = (2, 3, -0.5835);
        connGenes[3] = (2, 4, 5.123456789);
        connGenes[4] = (2, 5, 2.5);
        connGenes[5] = (5, 4, 5.4);

        // Wrap in a genome.
        NeatGenome<double> genome = genomeBuilder.Create(0, 0, connGenes);

        // Act.
        NetFileModel netFileModel = NeatGenomeConverter.ToNetFileModel(genome);

        // Assert.
        netFileModel.Should().NotBeNull();
        netFileModel.InputCount.Should().Be(3);
        netFileModel.OutputCount.Should().Be(2);
        netFileModel.IsAcyclic.Should().BeTrue();
        netFileModel.CyclesPerActivation.Should().Be(0);

        var conns = netFileModel.Connections;
        conns.Should().NotBeNull().And.HaveCount(6);
        conns[0].Validate(0, 3, 0.123);
        conns[1].Validate(1, 3, 1.234);
        conns[2].Validate(2, 3, -0.5835);
        conns[3].Validate(2, 4, 5.123456789);
        conns[4].Validate(2, 5, 2.5);
        conns[5].Validate(5, 4, 5.4);
    }

    [Fact]
    public void NetFileModelShouldConvertToGenome()
    {
        // Arrange.
        NetFileModel netFileModel = NetFile.Load("TestData/example2.net");
        var metaNeatGenome = MetaNeatGenome<double>.CreateAcyclic(3, 2, new ReLU<double>());

        // Act.
        NeatGenome<double> genome = NeatGenomeConverter.ToNeatGenome(
            netFileModel, metaNeatGenome, 101);

        // Assert.
        genome.Id.Should().Be(101);

        genome.ConnectionGenes.Length.Should().Be(6);
        DirectedConnection[] connArr = genome.ConnectionGenes._connArr;
        connArr[0].Validate(0, 3);
        connArr[1].Validate(1, 3);
        connArr[2].Validate(2, 3);
        connArr[3].Validate(2, 4);
        connArr[4].Validate(2, 5);
        connArr[5].Validate(5, 4);

        double[] weightArr = genome.ConnectionGenes._weightArr;
        weightArr.Should().BeEquivalentTo(
            [0.123, 1.234, -0.5835, 5.123456789, 2.5, 5.4],
            o => o.WithStrictOrdering());
    }
}
