﻿using Redzen.Random;
using Redzen.Sorting;
using SharpNeat.Graphs;
using SharpNeat.Neat.Genome;
using Xunit;

namespace SharpNeat.Neat;

public class NeatPopulationTests
{
    [Fact]
    public void CreatePopulation()
    {
        MetaNeatGenome<double> metaNeatGenome =
            MetaNeatGenome<double>.CreateAcyclic(
                inputNodeCount: 3,
                outputNodeCount: 2,
                activationFn: new NeuralNets.ActivationFunctions.ReLU<double>());

        int count = 10;
        NeatPopulation<double> neatPop = NeatPopulationFactory<double>.CreatePopulation(metaNeatGenome, 1.0, count, RandomDefaults.CreateRandomSource());
        Assert.Equal(count, neatPop.GenomeList.Count);
        Assert.Equal(count, neatPop.GenomeIdSeq.Peek);

        // The population factory assigns the same innovation IDs to matching structures in the genomes it creates.
        // In this test there are 5 nodes and 6 connections in each genome, and they are each identifiably
        // the same structure in each of the genomes (e.g. input 0 or whatever) and so have the same innovation ID
        // across all of the genomes.
        // Thus in total although we created N genomes there are only 5 innovation IDs allocated (5 nodes).
        Assert.Equal(5, neatPop.InnovationIdSeq.Peek);

        // Loop the created genomes.
        for(int i=0; i < count; i++)
        {
            var genome = neatPop.GenomeList[i];
            Assert.Equal(i, genome.Id);
            Assert.Equal(0, genome.BirthGeneration);

            TestGenome(genome);
        }
    }

    [Fact]
    public void VerifyInitialConnections()
    {
        MetaNeatGenome<double> metaNeatGenome =
            MetaNeatGenome<double>.CreateAcyclic(
                inputNodeCount: 100,
                outputNodeCount: 200,
                activationFn: new NeuralNets.ActivationFunctions.ReLU<double>());

        NeatPopulation<double> neatPop = NeatPopulationFactory<double>.CreatePopulation(
            metaNeatGenome,
            0.5,
            2);

        NeatGenome<double> genome = neatPop.GenomeList[0];

        Assert.Equal(10_000, genome.ConnectionGenes.Length);
        Assert.True(SortUtils.IsSortedAscending<DirectedConnection>(genome.ConnectionGenes._connArr));

        CalcWeightMinMaxMean(genome.ConnectionGenes._weightArr, out double min, out double max, out double mean);

        Assert.True(min < -genome.MetaNeatGenome.ConnectionWeightScale * 0.98);
        Assert.True(max > genome.MetaNeatGenome.ConnectionWeightScale * 0.98);
        Assert.True(Math.Abs(mean) < 0.1);
    }

    #region Private Static Methods

    private static void TestGenome(NeatGenome<double> genome)
    {
        Assert.NotNull(genome);
        Assert.NotNull(genome.MetaNeatGenome);
        Assert.Equal(3, genome.MetaNeatGenome.InputNodeCount);
        Assert.Equal(2, genome.MetaNeatGenome.OutputNodeCount);
        Assert.True(genome.MetaNeatGenome.IsAcyclic);
        Assert.Equal(5.0, genome.MetaNeatGenome.ConnectionWeightScale);

        double x = 0.1;
        genome.MetaNeatGenome.ActivationFn.Fn(ref x);
        Assert.Equal(0.1, x);

        x = -0.1;
        genome.MetaNeatGenome.ActivationFn.Fn(ref x);
        Assert.Equal(0.0, x);

        Assert.Equal(6, genome.ConnectionGenes.Length);
        Assert.True(SortUtils.IsSortedAscending<DirectedConnection>(genome.ConnectionGenes._connArr));
    }

    private static void CalcWeightMinMaxMean(double[] weightArr, out double min, out double max, out double mean)
    {
        double total = weightArr[0];
        min = total;
        max = total;

        for(int i=1; i < weightArr.Length; i++)
        {
            double weight = weightArr[i];
            total += weight;
            min = Math.Min(min, weight);
            max = Math.Max(max, weight);
        }

        mean = total / weightArr.Length;
    }

    #endregion
}
