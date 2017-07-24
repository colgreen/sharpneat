using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Network2;
using SharpNeat.Network2.Acyclic;
using SharpNeat.NeuralNets;

namespace SharpNeatLib.Tests.Phenomes.NeuralNets
{

    [TestClass]
    public class AcyclicNeuralNetTests
    {

        #region Test Methods

        [TestMethod]
        [TestCategory("AcyclicNeuralNet")]
        public void SingleInputOutput()
        {
            var connList = new List<IWeightedDirectedConnection<double>>();
            connList.Add(new WeightedDirectedConnection<double>(0, 1, 0.0));





            // Create graph.
            var digraph = WeightedAcyclicDirectedGraphFactory<double>.Create(connList, 1, 1);



            //new AcyclicNeuralNet(digraph, SteepenedSigmoid)
            
        }

        #endregion


    }



}
