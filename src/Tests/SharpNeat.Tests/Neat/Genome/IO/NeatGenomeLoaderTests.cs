﻿using SharpNeat.NeuralNets.ActivationFunctions;
using Xunit;

namespace SharpNeat.Neat.Genome.IO;

public class NeatGenomeLoaderTests
{
    [Fact]
    public void LoadGenome()
    {
        var metaNeatGenome = MetaNeatGenome<double>.CreateAcyclic(3, 2, new ReLU<double>());

        // Load test genome.
        NeatGenome<double> genomeLoaded = NeatGenomeLoader.Load(
            "TestData/example1.net", metaNeatGenome, 0);

        // Manually build an equivalent genome.
        using NeatGenome<double> genomeBuilt = CreateGenome1(metaNeatGenome);

        // Compare the two genomes.
        IOTestUtils.CompareGenomes(genomeLoaded, genomeBuilt);
    }

    #region Private Static Methods

    private static NeatGenome<double> CreateGenome1(MetaNeatGenome<double> metaNeatGenome)
    {
        var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(metaNeatGenome);

        // Define a genome that matches the one defined in example1.net.
        var connGenes = new ConnectionGenes<double>(12);
        connGenes[0] = (0, 5, 0.5);
        connGenes[1] = (0, 7, 0.7);
        connGenes[2] = (0, 3, 0.3);
        connGenes[3] = (1, 5, 1.5);
        connGenes[4] = (1, 7, 1.7);
        connGenes[5] = (1, 3, 1.3);
        connGenes[6] = (1, 6, 1.6);
        connGenes[7] = (1, 8, 1.8);
        connGenes[8] = (1, 4, 1.4);
        connGenes[9] = (2, 6, 2.6);
        connGenes[10] = (2, 8, 2.8);
        connGenes[11] = (2, 4, 2.4);

        // Ensure the connections are sorted correctly.
        connGenes.Sort();

        // Wrap in a genome.
        NeatGenome<double> genome = genomeBuilder.Create(0, 0, connGenes);

        return genome;
    }

    #endregion
}
