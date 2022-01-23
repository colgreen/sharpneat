using System;
using Redzen.Collections;
using SharpNeat.BlackBox;
using SharpNeat.Graphs;
using SharpNeat.NeuralNets.Double.ActivationFunctions;
using Xunit;

namespace SharpNeat.NeuralNets.Double.Tests
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
            var connList = new LightweightList<WeightedDirectedConnection<double>>
            {
                new WeightedDirectedConnection<double>(0, 1, 0.0)
            };
            var connSpan = connList.AsSpan();

            // Create graph.
            var digraph = WeightedDirectedGraphBuilder<double>.Create(connSpan, 1, 1);

            // Create neural net and run tests.
            var actFn = new Logistic();
            var net = new NeuralNetCyclic(digraph, actFn.Fn, 2);
            SingleInput_WeightZero_Inner(net);

            // Create vectorized neural net and run tests.
            var vnet = new NeuralNets.Double.Vectorized.NeuralNetCyclic(digraph, actFn.Fn, 2);
            SingleInput_WeightZero_Inner(vnet);
        }

        /// <summary>
        /// One input and output, a single connection between them (i.e no connectivity cycle), connection weight is one.
        /// </summary>
        [Fact]
        public void SingleInput_WeightOne()
        {
            var connList = new LightweightList<WeightedDirectedConnection<double>>
            {
                new WeightedDirectedConnection<double>(0, 1, 1.0)
            };
            var connSpan = connList.AsSpan();

            // Create graph.
            var digraph = WeightedDirectedGraphBuilder<double>.Create(connSpan, 1, 1);

            // Create neural net and run tests.
            var actFn = new Logistic();
            var net = new NeuralNetCyclic(digraph, actFn.Fn, 1);
            SingleInput_WeightOne_Inner(net, actFn);

            // Create vectorized neural net and run tests.
            var vnet = new NeuralNets.Double.Vectorized.NeuralNetCyclic(digraph, actFn.Fn, 1);
            SingleInput_WeightOne_Inner(vnet, actFn);
        }

        /// <summary>
        /// One input and output, a single connection between them, and another connection
        /// from the output node back to itself, thus forming the simplest possible cyclic network.
        /// </summary>
        [Fact]
        public void CyclicOutput()
        {
            var connList = new LightweightList<WeightedDirectedConnection<double>>
            {
                new WeightedDirectedConnection<double>(0, 1, 1.0),
                new WeightedDirectedConnection<double>(1, 1, 1.0)
            };
            var connSpan = connList.AsSpan();

            // Create graph.
            var digraph = WeightedDirectedGraphBuilder<double>.Create(connSpan, 1, 1);

            // Create neural net and run tests.
            var actFn = new Logistic();
            var net = new NeuralNetCyclic(digraph, actFn.Fn, 1);
            CyclicOutput_Inner(net, actFn);

            // Create vectorized neural net and run tests.
            var vnet = new NeuralNets.Double.Vectorized.NeuralNetCyclic(digraph, actFn.Fn, 1);
            CyclicOutput_Inner(vnet, actFn);
        }

        [Fact]
        public void ComplexCyclic()
        {
            var connList = new LightweightList<WeightedDirectedConnection<double>>
            {
                new WeightedDirectedConnection<double>(0, 1, -2.0),
                new WeightedDirectedConnection<double>(0, 2, 1.0),
                new WeightedDirectedConnection<double>(1, 2, 1.0),
                new WeightedDirectedConnection<double>(2, 1, 1.0)
            };
            var connSpan = connList.AsSpan();

            // Create graph.
            var digraph = WeightedDirectedGraphBuilder<double>.Create(connSpan, 1, 1);

            // Create neural net and run tests.
            var actFn = new Logistic();
            var net = new NeuralNetCyclic(digraph, actFn.Fn, 1);
            ComplexCyclic_Inner(net, actFn);

            // Create vectorized neural net and run tests.
            var vnet = new NeuralNets.Double.Vectorized.NeuralNetCyclic(digraph, actFn.Fn, 1);
            ComplexCyclic_Inner(vnet, actFn);
        }

        [Fact]
        public void MultipleInputsOutputs()
        {
            var connList = new LightweightList<WeightedDirectedConnection<double>>
            {
                new WeightedDirectedConnection<double>(0, 5, 1.0),
                new WeightedDirectedConnection<double>(1, 3, 1.0),
                new WeightedDirectedConnection<double>(2, 4, 1.0)
            };
            var connSpan = connList.AsSpan();

            // Create graph.
            var digraph = WeightedDirectedGraphBuilder<double>.Create(connSpan, 3, 3);

            // Create neural net and run tests.
            var actFn = new Logistic();
            var net = new NeuralNetCyclic(digraph, actFn.Fn, 1);
            MultipleInputsOutputs_Inner(net, actFn);

            // Create neural net and run tests.
            var vnet = new NeuralNets.Double.Vectorized.NeuralNetCyclic(digraph, actFn.Fn, 1);
            MultipleInputsOutputs_Inner(vnet, actFn);
        }

        #endregion

        #region Private Static Methods

        private static void SingleInput_WeightZero_Inner(
            IBlackBox<double> net)
        {
            var inputs = net.InputVector.Span;

            // Note. The single connection weight is zero, so the input value has no affect.
            // Activate and test.
            inputs[0] = 100.0;
            net.Activate();
            Assert.Equal(0.5, net.OutputVector[0]);

            // Activate and test.
            inputs[0] = 0;
            net.Activate();
            Assert.Equal(0.5, net.OutputVector[0]);

            // Activate and test.
            inputs[0] = -100;
            net.Activate();
            Assert.Equal(0.5, net.OutputVector[0]);
        }

        private static void SingleInput_WeightOne_Inner(
            IBlackBox<double> net,
            IActivationFunction<double> actFn)
        {
            var inputs = net.InputVector.Span;

            // Activate and test.
            inputs[0] = 0.0;
            for(int i=0; i < 10; i++)
            {
                net.Activate();
                Assert.Equal(0.5, net.OutputVector[0]);
            }

            // Activate and test.
            double x;
            x = 1.0; actFn.Fn(ref x);
            inputs[0] = 1.0;
            for(int i=0; i < 10; i++)
            {
                net.Activate();
                
                Assert.Equal(x, net.OutputVector[0]);
            }

            // Activate and test.
            x = 10.0; actFn.Fn(ref x);
            inputs[0] = 10.0;
            for(int i=0; i < 10; i++)
            {
                net.Activate();
                Assert.Equal(x, net.OutputVector[0]);
            }
        }

        private static void CyclicOutput_Inner(
            IBlackBox<double> net,
            IActivationFunction<double> actFn)
        {
            // Activate and test.
            const double input = 0.1;
            var inputs = net.InputVector.Span;

            double inputVal = input;
            inputs[0] = inputVal;

            for(int i=0; i < 10; i++)
            {
                net.Activate();
                double outputExpected = inputVal; actFn.Fn(ref outputExpected);
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
            var inputs = net.InputVector.Span;

            postArr[0] = 3.0;
            inputs[0] = 3.0;

            for(int i=0; i < 10; i++)
            {
                preArr[1] = postArr[0] * -2.0 + postArr[2];
                preArr[2] = postArr[0] + postArr[1];

                actFn.Fn(ref preArr[1], ref postArr[1]);
                actFn.Fn(ref preArr[2], ref postArr[2]);

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
            inputs[0] = 3.0;

            for(int i=0; i < 10; i++)
            {
                preArr[1] = postArr[0] * -2.0 + postArr[2];
                preArr[2] = postArr[0] + postArr[1];

                actFn.Fn(ref preArr[1], ref postArr[1]);
                actFn.Fn(ref preArr[2], ref postArr[2]);

                net.Activate();

                Assert.Equal(postArr[1], net.OutputVector[0]);
            }
        }

        private static void MultipleInputsOutputs_Inner(
            IBlackBox<double> net,
            IActivationFunction<double> actFn)
        {
            double x;
            var inputs = net.InputVector.Span;

            // Activate and test.
            inputs[0] = 1.0;
            inputs[1] = 2.0;
            inputs[2] = 3.0;
            net.Activate();

            x = 2.0; actFn.Fn(ref x);
            Assert.Equal(x, net.OutputVector[0]);

            x = 3.0; actFn.Fn(ref x);
            Assert.Equal(x, net.OutputVector[1]);

            x = 1.0; actFn.Fn(ref x);
            Assert.Equal(x, net.OutputVector[2]);
        }

        #endregion
    }
}
