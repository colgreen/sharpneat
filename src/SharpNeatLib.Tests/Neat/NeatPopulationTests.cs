using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat;
using SharpNeat.Neat.Genome;

namespace SharpNeatLib.Tests.Neat
{
    [TestClass]
    public class NeatPopulationTests
    {
        [TestMethod]
        [TestCategory("NeatPopulation")]
        public void TestCreatePopulation()
        {
            MetaNeatGenome metaNeatGenome = new MetaNeatGenome();
            metaNeatGenome.InputNodeCount = 3;
            metaNeatGenome.OutputNodeCount = 1;
            metaNeatGenome.IsAcyclic = true;

            int count = 10;
            NeatPopulation<double> neatPop = NeatPopulationFactory<double>.CreatePopulation(metaNeatGenome, 1.0, count);
            Assert.AreEqual(count, neatPop.GenomeList.Count);
            Assert.AreEqual((uint)count, neatPop.GenomeIdSeq.Peek);

            // The factory assigns the same innovations IDs to matching structures in the genomes it creates.
            // So in this cases there are only 4 nodes and 3 connections in each genome, and they are each identifiably
            // the same structure (e.g. input 0 or whatever) in each of the genomes and so have the same innovation ID
            // across all of the genomes.
            // Thus in total although we created N genomes there are only 7 innovation IDs allocated.
            Assert.AreEqual((uint)7, neatPop.InnovationIdSeq.Peek);

            // The structures should all be recorded in the 'structure buffers'; these are used by the mutation logic 
            // to identify where a mutation would create a structure that already exists elsewhere in the population 
            // (e.g. a connection between nodes 0 and 10), and thus to re-use the same innovation ID where possible.
            //
            // Note. Input and output nodes aren't recorded in the buffer because they are fixed/invariant, i.e. present on all 
            // genomes all of the time.
            Assert.AreEqual(0, neatPop.AddedNodeBuffer.Length);


            // Connections are recorded.
            Assert.AreEqual(3, neatPop.AddedConnectionBuffer.Length);




            //// Loop the created genomes.
            //for(int i=0; i<count; i++)
            //{
            //    neatPop.GenomeList


            //}
        }





    }
}
