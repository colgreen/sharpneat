using System;
using System.Collections.Generic;
using SharpNeat.BlackBox;
using SharpNeat.Graphs;
using SharpNeat.NeuralNet;
using SharpNeat.NeuralNet.Double;
using SharpNeat.NeuralNet.Double.ActivationFunctions;
using Xunit;

namespace SharpNeat.Tests.NeuralNets.Double
{
    public class NeuralNetCyclicTests
    {
        #region Test Methods

        /// <summary>
        /// One input and output, a single connection between them (i.e no connectivity cycle), connection weight is zero.
        /// </summary>
        [Fact]
        public void SingleInput_WeightZero()
        {
            var connList = new List<WeightedDirectedConnection<double>>
            {
                new WeightedDirectedConnection<double>(0, 1, 0.0)
            };

            // Create graph.
            var digraph = WeightedDirectedGraphBuilder<double>.Create(connList, 1, 1);

            // Create neural net and run tests.
            var actFn = new Logistic();
            var net = new NeuralNetCyclic(digraph, actFn.Fn, 2);
            SingleInput_WeightZero_Inner(net);

            // Create vectorized neural net and run tests.
            var vnet = new NeuralNet.Double.Vectorized.NeuralNetCyclic(digraph, actFn.Fn, 2);
            SingleInput_WeightZero_Inner(vnet);
        }

        /// <summary>
        /// One input and output, a single connection between them (i.e no connectivity cycle), connection weight is one.
        /// </summary>
        [Fact]
        public void SingleInput_WeightOne()
        {
            var connList = new List<WeightedDirectedConnection<double>>
            {
                new WeightedDirectedConnection<double>(0, 1, 1.0)
            };

            // Create graph.
            var digraph = WeightedDirectedGraphBuilder<double>.Create(connList, 1, 1);

            // Create neural net and run tests.
            var actFn = new Logistic();
            var net = new NeuralNetCyclic(digraph, actFn.Fn, 1);
            SingleInput_WeightOne_Inner(net, actFn);

            // Create vectorized neural net and run tests.
            var vnet = new NeuralNet.Double.Vectorized.NeuralNetCyclic(digraph, actFn.Fn, 1);
            SingleInput_WeightOne_Inner(vnet, actFn);
        }

        /// <summary>
        /// One input and output, a single connection between them, and another connection 
        /// from the output node back to itself, thus forming the simplest possible cyclic network.
        /// </summary>
        [Fact]
        public void CyclicOutput()
        {
            var connList = new List<WeightedDirectedConnection<double>>
            {
                new WeightedDirectedConnection<double>(0, 1, 1.0),
                new WeightedDirectedConnection<double>(1, 1, 1.0)
            };

            // Create graph.
            var digraph = WeightedDirectedGraphBuilder<double>.Create(connList, 1, 1);

            // Create neural net and run tests.
            var actFn = new Logistic();
            var net = new NeuralNetCyclic(digraph, actFn.Fn, 1);
            CyclicOutput_Inner(net, actFn);

            // Create vectorized neural net and run tests.
            var vnet = new NeuralNet.Double.Vectorized.NeuralNetCyclic(digraph, actFn.Fn, 1);
            CyclicOutput_Inner(vnet, actFn);
        }

        [Fact]
        public void ComplexCyclic()
        {
            var connList = new List<WeightedDirectedConnection<double>>
            {
                new WeightedDirectedConnection<double>(0, 1, -2.0),
                new WeightedDirectedConnection<double>(0, 2, 1.0),
                new WeightedDirectedConnection<double>(1, 2, 1.0),
                new WeightedDirectedConnection<double>(2, 1, 1.0)
            };

            // Create graph.
            var digraph = WeightedDirectedGraphBuilder<double>.Create(connList, 1, 1);

            // Create neural net and run tests.
            var actFn = new Logistic();
            var net = new NeuralNetCyclic(digraph, actFn.Fn, 1);
            ComplexCyclic_Inner(net, actFn);

            // Create vectorized neural net and run tests.
            var vnet = new NeuralNet.Double.Vectorized.NeuralNetCyclic(digraph, actFn.Fn, 1);
            ComplexCyclic_Inner(vnet, actFn);
        }

        [Fact]
        public void MultipleInputsOutputs()
        {
            var connList = new List<WeightedDirectedConnection<double>>
            {
                new WeightedDirectedConnection<double>(0, 5, 1.0),
                new WeightedDirectedConnection<double>(1, 3, 1.0),
                new WeightedDirectedConnection<double>(2, 4, 1.0)
            };

            // Create graph.
            var digraph = WeightedDirectedGraphBuilder<double>.Create(connList, 3, 3);

            // Create neural net and run tests.
            var actFn = new Logistic();
            var net = new NeuralNetCyclic(digraph, actFn.Fn, 1);
            MultipleInputsOutputs_Inner(net, actFn);

            // Create neural net and run tests.
            var vnet = new NeuralNet.Double.Vectorized.NeuralNetCyclic(digraph, actFn.Fn, 1);
            MultipleInputsOutputs_Inner(vnet, actFn);
        }

        #endregion

        #region Private Static Methods

        private static void SingleInput_WeightZero_Inner(
            IBlackBox<double> net)
        {
            // Note. The single connection weight is zero, so the input value has no affect.
            // Activate and test.
            net.InputVector[0] = 100.0;
            net.Activate();
            Assert.Equal(0.5, net.OutputVector[0]);

            // Activate and test.
            net.InputVector[0] = 0;
            net.Activate();
            Assert.Equal(0.5, net.OutputVector[0]);

            // Activate and test.
            net.InputVector[0] = -100;
            net.Activate();
            Assert.Equal(0.5, net.OutputVector[0]);
        }

        private static void SingleInput_WeightOne_Inner(
            IBlackBox<double> net,
            IActivationFunction<double> actFn)
        {
            // Activate and test.
            net.InputVector[0] = 0.0;
            for(int i=0; i < 10; i++)
            {    
                net.Activate();
                Assert.Equal(0.5, net.OutputVector[0]);
            }

            // Activate and test.
            net.InputVector[0] = 1.0;
            for(int i=0; i < 10; i++)
            {    
                net.Activate();
                Assert.Equal(actFn.Fn(1), net.OutputVector[0]);
            }

            // Activate and test.
            net.InputVector[0] = 10.0;
            for(int i=0; i < 10; i++)
            {    
                net.Activate();
                Assert.Equal(actFn.Fn(10), net.OutputVector[0]);
            }
        }

        private static void CyclicOutput_Inner(
            IBlackBox<double> net,
            IActivationFunction<double> actFn)
        {
            // Activate and test.
            const double input = 0.1;
            double inputVal = input;
            net.InputVector[0] = inputVal;
            
            for(int i=0; i < 10; i++)
            {
                net.Activate();
                double outputExpected = actFn.Fn(inputVal);
                Assert.Equal(outputExpected, net.OutputVector[0]);
                inputVal = input + outputExpected;
            }
        }

        private static void ComplexCyclic_Inner(
            IBlackBox<double> net,
            IActivationFunction<double> actFn)
        {
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
                
                Assert.Equal(postArr[1], net.OutputVector[0]);
            }

            // Rest the network's internal state.
            net.ResetState();
            Assert.Equal(0.0, net.OutputVector[0]);

            // Run the test again.
            Array.Clear(preArr, 0, preArr.Length);
            Array.Clear(postArr, 0, postArr.Length);

            postArr[0] = 3.0;
            net.InputVector[0] = 3.0;

            for(int i=0; i < 10; i++)
            {
                preArr[1] = postArr[0] * -2.0 + postArr[2];
                preArr[2] = postArr[0] + postArr[1];

                postArr[1] = actFn.Fn(preArr[1]);
                postArr[2] = actFn.Fn(preArr[2]);

                net.Activate();
                
                Assert.Equal(postArr[1], net.OutputVector[0]);
            }
        }

        private static void MultipleInputsOutputs_Inner(
            IBlackBox<double> net,
            IActivationFunction<double> actFn)
        {
            // Activate and test.
            net.InputVector[0] = 1.0;
            net.InputVector[1] = 2.0;
            net.InputVector[2] = 3.0;
            net.Activate();
            Assert.Equal(actFn.Fn(2.0), net.OutputVector[0]);
            Assert.Equal(actFn.Fn(3.0), net.OutputVector[1]);
            Assert.Equal(actFn.Fn(1.0), net.OutputVector[2]);
        }

        #endregion
    }
}
