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
                activationFn: new SharpNeat.NeuralNets.Double.ActivationFunctions.ReLU());

            int count = 10;
            NeatPopulation<double> neatPop = NeatPopulationFactory<double>.CreatePopulation(metaNeatGenome, 1.0, count);
            Assert.AreEqual(count, neatPop.GenomeList.Count);
            Assert.AreEqual(count, neatPop.GenomeIdSeq.Peek);

            // The factory assigns the same innovation IDs to matching structures in the genomes it creates.
            // So in this cases there are only 5 nodes and 6 connections in each genome, and they are each identifiably
            // the same structure in each of the genomes (e.g. input 0 or whatever) and so have the same innovation ID
            // across all of the genomes.
            // Thus in total although we created N genomes there are only 11 innovation IDs allocated.
            Assert.AreEqual(11, neatPop.InnovationIdSeq.Peek);

            // The structures should all be recorded in the 'structure buffers'; these are used by the mutation logic 
            // to identify where a mutation would create a structure that already exists elsewhere in the population 
            // (e.g. a connection between nodes 0 and 3), and thus allows for re-use of the same innovation ID where possible.
            //
            // Note. Input and output nodes aren't recorded in the buffer because they are fixed/invariant, i.e. present on all 
            // genomes all of the time, and not the result of an 'add node' mutation.
            //
            // Connections directly from inputs and outputs are also not recorded, but their innovations IDs are
            // defined by convention and thus recognised by AddedConnectionBuffer.

            // Test lookups.
            var buff = neatPop.AddedConnectionBuffer;
            TestLookupSuccess(buff, 0, 3, 5);
            TestLookupSuccess(buff, 0, 4, 6);
            TestLookupSuccess(buff, 1, 3, 7);
            TestLookupSuccess(buff, 1, 4, 8);
            TestLookupSuccess(buff, 2, 3, 9);
            TestLookupSuccess(buff, 2, 4, 10);

            // Test lookup failure.
            TestLookupFail(buff, 3, 0);
            TestLookupFail(buff, 4, 0);
            TestLookupFail(buff, 3, 1);
            TestLookupFail(buff, 4, 1);
            TestLookupFail(buff, 3, 2);
            TestLookupFail(buff, 4, 2);

            TestLookupFail(buff, 0, 5);
            TestLookupFail(buff, 1, 5);
            TestLookupFail(buff, 2, 5);
            TestLookupFail(buff, 2, 5);
            
            TestLookupFail(buff, 5, 3);
            TestLookupFail(buff, 5, 6);

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
            Assert.AreEqual(6, genome.ConnectionGeneArray.Length);
            Assert.IsTrue(ConnectionGeneUtils.IsSorted<double>(genome.ConnectionGeneArray));
        }

        [TestMethod]
        [TestCategory("NeatPopulation")]
        public void TestInitialConnections()
        {
            MetaNeatGenome<double> metaNeatGenome = new MetaNeatGenome<double>(
                inputNodeCount: 100,
                outputNodeCount: 200,
                isAcyclic: true,
                activationFn: new SharpNeat.NeuralNets.Double.ActivationFunctions.ReLU());

            
            NeatPopulation<double> neatPop = NeatPopulationFactory<double>.CreatePopulation(metaNeatGenome, 0.5, 1);
            NeatGenome<double> genome = neatPop.GenomeList[0];

            Assert.AreEqual(10000, genome.ConnectionGeneArray.Length);
            Assert.IsTrue(ConnectionGeneUtils.IsSorted<double>(genome.ConnectionGeneArray));

            double min, max, mean;
            CalcWeightMinMaxMean(genome.ConnectionGeneArray, out min, out max, out mean);

            Assert.IsTrue(min < -genome.MetaNeatGenome.ConnectionWeightRange * 0.98);
            Assert.IsTrue(max > genome.MetaNeatGenome.ConnectionWeightRange * 0.98);
            Assert.IsTrue(Math.Abs(mean) < 0.1);
        }

        #endregion

        #region Private Static Methods

        private static void TestLookupSuccess(AddedConnectionBuffer buff, int srcId, int tgtId, int expectedConnectionId)
        {
            int connectionId;
            Assert.AreEqual(true, buff.TryLookup(new DirectedConnection(srcId, tgtId), out connectionId));
            Assert.AreEqual(expectedConnectionId, connectionId);
        }

        private static void TestLookupFail(AddedConnectionBuffer buff, int srcId, int tgtId)
        {
            int connectionId;
            Assert.AreEqual(false, buff.TryLookup(new DirectedConnection(srcId, tgtId), out connectionId));
        }

        private void CalcWeightMinMaxMean(ConnectionGene<double>[] connGeneArr, out double min, out double max, out double mean)
        {
            double total = connGeneArr[0].Weight;
            min = total;
            max = total;
            
            for(int i=1; i<connGeneArr.Length; i++)
            {
                double weight = connGeneArr[i].Weight;
                total += weight;
                min = Math.Min(min, weight);
                max = Math.Max(max, weight);
            }

            mean = total / connGeneArr.Length;
        }

        #endregion


    }
}
