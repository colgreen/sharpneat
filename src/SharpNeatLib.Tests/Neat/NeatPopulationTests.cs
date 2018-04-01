using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;

namespace SharpNeatLib.Tests.Neat
{
    [TestClass]
    public class NeatPopulationTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("NeatPopulation")]
        public void TestCreatePopulation()
        {
            MetaNeatGenome<double> metaNeatGenome = new MetaNeatGenome<double>(
                inputNodeCount: 3,
                outputNodeCount: 2,
                isAcyclic: true,
                activationFn: new SharpNeat.NeuralNet.Double.ActivationFunctions.ReLU());

            int count = 10;
            NeatPopulation<double> neatPop = NeatPopulationFactory<double>.CreatePopulation(metaNeatGenome, 1.0, count);
            Assert.AreEqual(count, neatPop.GenomeList.Count);
            Assert.AreEqual(count, neatPop.GenomeIdSeq.Peek);

            // The population factory assigns the same innovation IDs to matching structures in the genomes it creates.
            // In this test there are 5 nodes and 6 connections in each genome, and they are each identifiably
            // the same structure in each of the genomes (e.g. input 0 or whatever) and so have the same innovation ID
            // across all of the genomes.
            // Thus in total although we created N genomes there are only 5 innovation IDs allocated (5 nodes).
            Assert.AreEqual(5, neatPop.InnovationIdSeq.Peek);

            // Loop the created genomes.
            for(int i=0; i<count; i++) 
            {
                var genome = neatPop.GenomeList[i];
                Assert.AreEqual(i, genome.Id);
                Assert.AreEqual(0, genome.BirthGeneration);

                TestGenome(genome);
            }
        }

        private void TestGenome(NeatGenome<double> genome)
        {
            Assert.IsNotNull(genome);
            Assert.IsNotNull(genome.MetaNeatGenome);
            Assert.AreEqual(3, genome.MetaNeatGenome.InputNodeCount);
            Assert.AreEqual(2, genome.MetaNeatGenome.OutputNodeCount);
            Assert.AreEqual(true, genome.MetaNeatGenome.IsAcyclic);
            Assert.AreEqual(5.0, genome.MetaNeatGenome.ConnectionWeightRange);
            Assert.AreEqual(0.1, genome.MetaNeatGenome.ActivationFn.Fn(0.1));
            Assert.AreEqual(0.0, genome.MetaNeatGenome.ActivationFn.Fn(-0.1));
            Assert.AreEqual(6, genome.ConnectionGenes.Length);
            Assert.IsTrue(DirectedConnectionUtils.IsSorted(genome.ConnectionGenes._connArr));
        }

        [TestMethod]
        [TestCategory("NeatPopulation")]
        public void TestInitialConnections()
        {
            MetaNeatGenome<double> metaNeatGenome = new MetaNeatGenome<double>(
                inputNodeCount: 100,
                outputNodeCount: 200,
                isAcyclic: true,
                activationFn: new SharpNeat.NeuralNet.Double.ActivationFunctions.ReLU());

            NeatPopulation<double> neatPop = NeatPopulationFactory<double>.CreatePopulation(metaNeatGenome, 0.5, 1);
            NeatGenome<double> genome = neatPop.GenomeList[0];

            Assert.AreEqual(10000, genome.ConnectionGenes.Length);
            Assert.IsTrue(DirectedConnectionUtils.IsSorted(genome.ConnectionGenes._connArr));

            CalcWeightMinMaxMean(genome.ConnectionGenes._weightArr, out double min, out double max, out double mean);

            Assert.IsTrue(min < -genome.MetaNeatGenome.ConnectionWeightRange * 0.98);
            Assert.IsTrue(max > genome.MetaNeatGenome.ConnectionWeightRange * 0.98);
            Assert.IsTrue(Math.Abs(mean) < 0.1);
        }

        #endregion

        #region Private Static Methods

        private void CalcWeightMinMaxMean(double[] weightArr, out double min, out double max, out double mean)
        {
            double total = weightArr[0];
            min = total;
            max = total;
            
            for(int i=1; i<weightArr.Length; i++)
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
}
