using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Network2;
using SharpNeat.NeuralNets;

namespace SharpNeatLib.Tests.Phenomes.NeuralNets
{
    [TestClass]
    public class CyclicNeuralNetTests
    {
        #region Test Methods

        /// <summary>
        /// One input and output, a single connection between them (i.e no connectivity cycle), connection weight is zero.
        /// </summary>
        [TestMethod]
        [TestCategory("CyclicNeuralNet")]
        public void SingleInput_WeightZero()
        {
            var connList = new List<IWeightedDirectedConnection<double>>();
            connList.Add(new WeightedDirectedConnection<double>(0, 1, 0.0));

            // Create graph.
            var digraph = WeightedDirectedGraphFactory<double>.Create(connList, 1, 1);

            // Create neural net
            var actFn = new LogisticFunction();
            var net = new CyclicNeuralNet(digraph, actFn.Fn, 2, false);

            // Note. The single connection weight is zero, so the input value has no affect.
            // Activate and test.
            net.InputVector[0] = 100.0;
            net.Activate();
            Assert.AreEqual(0.5, net.OutputVector[0]);

            // Activate and test.
            net.InputVector[0] = 0;
            net.Activate();
            Assert.AreEqual(0.5, net.OutputVector[0]);

            // Activate and test.
            net.InputVector[0] = -100;
            net.Activate();
            Assert.AreEqual(0.5, net.OutputVector[0]);
        }

        /// <summary>
        /// One input and output, a single connection between them (i.e no connectivity cycle), connection weight is one.
        /// </summary>
        [TestMethod]
        [TestCategory("CyclicNeuralNet")]
        public void SingleInput_WeightOne()
        {
            var connList = new List<IWeightedDirectedConnection<double>>();
            connList.Add(new WeightedDirectedConnection<double>(0, 1, 1.0));

            // Create graph.
            var digraph = WeightedDirectedGraphFactory<double>.Create(connList, 1, 1);

            // Create neural net
            var actFn = new LogisticFunction();
            var net = new CyclicNeuralNet(digraph, actFn.Fn, 1, false);

            // Activate and test.
            net.InputVector[0] = 0.0;
            for(int i=0; i<10; i++)
            {    
                net.Activate();
                Assert.AreEqual(0.5, net.OutputVector[0]);
            }

            // Activate and test.
            net.InputVector[0] = 1.0;
            for(int i=0; i<10; i++)
            {    
                net.Activate();
                Assert.AreEqual(actFn.Fn(1), net.OutputVector[0]);
            }

            // Activate and test.
            net.InputVector[0] = 10.0;
            for(int i=0; i<10; i++)
            {    
                net.Activate();
                Assert.AreEqual(actFn.Fn(10), net.OutputVector[0]);
            }
        }

        /// <summary>
        /// One input and output, a single connection between them, and another connection 
        /// from the output node back to itself, thus forming the simplest possible cyclic network.
        /// </summary>
        [TestMethod]
        [TestCategory("CyclicNeuralNet")]
        public void CyclicOutput()
        {

            var connList = new List<IWeightedDirectedConnection<double>>();
            connList.Add(new WeightedDirectedConnection<double>(0, 1, 1.0));
            connList.Add(new WeightedDirectedConnection<double>(1, 1, 1.0));

            // Create graph.
            var digraph = WeightedDirectedGraphFactory<double>.Create(connList, 1, 1);

            // Create neural net
            var actFn = new LogisticFunction();
            var net = new CyclicNeuralNet(digraph, actFn.Fn, 1, false);

            // Activate and test.
            const double input = 0.1;
            double inputVal = input;
            net.InputVector[0] = inputVal;
            
            for(int i=0; i < 10; i++)
            {
                net.Activate();
                double outputExpected = actFn.Fn(inputVal);
                Assert.AreEqual(outputExpected, net.OutputVector[0]);
                inputVal = input + outputExpected;
            }
        }

        [TestMethod]
        [TestCategory("CyclicNeuralNet")]
        public void ComplexCyclic()
        {
            var connList = new List<IWeightedDirectedConnection<double>>();
            connList.Add(new WeightedDirectedConnection<double>(0, 1, -2.0));
            connList.Add(new WeightedDirectedConnection<double>(0, 2, 1.0));
            connList.Add(new WeightedDirectedConnection<double>(1, 2, 1.0));
            connList.Add(new WeightedDirectedConnection<double>(2, 1, 1.0));

            // Create graph.
            var digraph = WeightedDirectedGraphFactory<double>.Create(connList, 1, 1);

            // Create neural net
            var actFn = new LogisticFunction();
            var net = new CyclicNeuralNet(digraph, actFn.Fn, 1, false);

            // Simulate network in C# and compare calculated outputs with actual network outputs.
            double[] preArr = new double[3];
            double[] postArr = new double[3];

            postArr[0] = 3.0;
            net.InputVector[0] = 3.0;

            for(int i=0; i < 10; i++)
            {
                preArr[1] = postArr[0] * -2.0 + postArr[2];
                preArr[2] = postArr[0] + postArr[1];

                postArr[1] = actFn.Fn(preArr[1]);
                postArr[2] = actFn.Fn(preArr[2]);

                net.Activate();
                
                Assert.AreEqual(postArr[1], net.OutputVector[0]);
            }
        }

        [TestMethod]
        [TestCategory("AcyclicNeuralNet")]
        public void MultipleInputsOutputs()
        {
            var connList = new List<IWeightedDirectedConnection<double>>();
            connList.Add(new WeightedDirectedConnection<double>(0, 5, 1.0));
            connList.Add(new WeightedDirectedConnection<double>(1, 3, 1.0));
            connList.Add(new WeightedDirectedConnection<double>(2, 4, 1.0));

            // Create graph.
            var digraph = WeightedDirectedGraphFactory<double>.Create(connList, 3, 3);

            // Create neural net
            var actFn = new LogisticFunction();
            var net = new CyclicNeuralNet(digraph, actFn.Fn, 1, false);

            // Activate and test.
            net.InputVector[0] = 1.0;
            net.InputVector[1] = 2.0;
            net.InputVector[2] = 3.0;
            net.Activate();
            Assert.AreEqual(actFn.Fn(2.0), net.OutputVector[0]);
            Assert.AreEqual(actFn.Fn(3.0), net.OutputVector[1]);
            Assert.AreEqual(actFn.Fn(1.0), net.OutputVector[2]);
        }


        #endregion
    }
}
