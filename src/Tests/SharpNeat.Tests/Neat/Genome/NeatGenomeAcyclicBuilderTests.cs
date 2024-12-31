﻿using SharpNeat.Graphs;
using SharpNeat.Graphs.Acyclic;
using SharpNeat.NeuralNets.ActivationFunctions;
using Xunit;
using static SharpNeat.Neat.Genome.ConnectionCompareUtils;

namespace SharpNeat.Neat.Genome;

public class NeatGenomeAcyclicBuilderTests
{
    [Fact]
    public void Simple()
    {
        var metaNeatGenome = MetaNeatGenome<double>.CreateAcyclic(3, 2, new ReLU<double>());
        var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(metaNeatGenome);

        // Simple acyclic graph.
        var connGenes = new ConnectionGenes<double>(4);
        connGenes[0] = (0, 3, 0.0);
        connGenes[1] = (1, 3, 1.0);
        connGenes[2] = (2, 3, 2.0);
        connGenes[3] = (2, 4, 3.0);

        // Wrap in a genome.
        var genome = genomeBuilder.Create(0, 0, connGenes);

        // Note. The genome builder creates a digraph representation of the genome and attaches/caches it on the genome object.
        var acyclicDigraph = (DirectedGraphAcyclic)genome.DirectedGraph;
        Assert.NotNull(acyclicDigraph);

        // The graph should be unchanged from the input connections.
        CompareConnectionLists(connGenes, acyclicDigraph.ConnectionIds, genome.ConnectionIndexMap);

        // Check the node count.
        Assert.Equal(5, acyclicDigraph.TotalNodeCount);
    }

    [Fact]
    public void DepthNodeReorderTest()
    {
        var metaNeatGenome = MetaNeatGenome<double>.CreateAcyclic(2, 2, new ReLU<double>());
        var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(metaNeatGenome);

        // Define graph connections.
        var connGenes = new ConnectionGenes<double>(5);
        connGenes[0] = (0, 4, 0.0);
        connGenes[1] = (4, 5, 1.0);
        connGenes[2] = (5, 2, 2.0);
        connGenes[3] = (1, 2, 3.0);
        connGenes[4] = (2, 3, 4.0);
        connGenes.Sort();

        // Wrap in a genome.
        var genome = genomeBuilder.Create(0, 0, connGenes);

        // Note. The genome builder creates a digraph representation of the genome and attaches/caches it on the genome object.
        var acyclicDigraph = (DirectedGraphAcyclic)genome.DirectedGraph;
        Assert.NotNull(acyclicDigraph);

        // Simulate the actual weight array that would occur in e.g. a WeightedDirectedGraphAcyclic or NeuralNetAcyclic.
        double[] weightArrActual = new double[connGenes._weightArr.Length];
        for(int i=0; i < weightArrActual.Length; i++)
        {
            weightArrActual[i] = connGenes._weightArr[genome.ConnectionIndexMap[i]];
        }

        // The nodes should have IDs allocated based on depth, i.e. the layer they are in.
        // And connections should be ordered by source node ID.
        var connArrExpected = new DirectedConnection[5];
        var weightArrExpected = new double[5];
        connArrExpected[0] = new DirectedConnection(0, 2); weightArrExpected[0] = 0.0;
        connArrExpected[1] = new DirectedConnection(1, 4); weightArrExpected[1] = 3.0;
        connArrExpected[2] = new DirectedConnection(2, 3); weightArrExpected[2] = 1.0;
        connArrExpected[3] = new DirectedConnection(3, 4); weightArrExpected[3] = 2.0;
        connArrExpected[4] = new DirectedConnection(4, 5); weightArrExpected[4] = 4.0;

        // Compare actual and expected connections.
        CompareConnectionLists(connArrExpected, weightArrExpected, acyclicDigraph.ConnectionIds, weightArrActual);

        // Test layer info.
        LayerInfo[] layerArrExpected =
        [
            new LayerInfo(2, 2),
            new LayerInfo(3, 3),
            new LayerInfo(4, 4),
            new LayerInfo(5, 5),
            new LayerInfo(6, 5),
        ];
        NetworkUtils.CompareLayerInfoLists(layerArrExpected, acyclicDigraph.LayerArray);

        // Check the node count.
        Assert.Equal(6, acyclicDigraph.TotalNodeCount);
    }
}
