using System.Collections.Generic;
using SharpNeat.BlackBox;
using SharpNeat.Network;
using SharpNeat.Network.Acyclic;
using SharpNeat.NeuralNet;
using SharpNeat.NeuralNet.Double;
using SharpNeat.NeuralNet.Double.ActivationFunctions;
using Xunit;

namespace SharpNeat.Tests.NeuralNets.Double
{
    public class NeuralNetAcyclicTests
    {
        #region Test Methods

        [Fact]
        public void SingleInput_WeightZero()
        {
            var connList = new List<WeightedDirectedConnection<double>> {
                new WeightedDirectedConnection<double>(0,1,0.0)
            };

            // Create graph.
            var digraph = WeightedDirectedGraphAcyclicBuilder<double>.Create(connList, 1, 1);

            // Create neural net and run tests.
            var actFn = new Logistic();
            var net = new NeuralNetAcyclic(digraph, actFn.Fn);
            SingleInput_WeightZero_Inner(net);

            // Create vectorized neural net and run tests.
            var vnet = new NeuralNet.Double.Vectorized.NeuralNetAcyclic(digraph, actFn.Fn);
            SingleInput_WeightZero_Inner(vnet);
        }

        [Fact]
        public void SingleInput_WeightOne()
        {
            var connList = new List<WeightedDirectedConnection<double>> {
                new WeightedDirectedConnection<double>(0,1,1.0)
            };

            // Create graph.
            var digraph = WeightedDirectedGraphAcyclicBuilder<double>.Create(connList, 1, 1);

            // Create neural net and run tests.
            var actFn = new Logistic();
            var net = new NeuralNetAcyclic(digraph, actFn.Fn);
            SingleInput_WeightOne_Inner(net, actFn);

            // Create vectorized neural net and run tests.
            var vnet = new NeuralNet.Double.Vectorized.NeuralNetAcyclic(digraph, actFn.Fn);
            SingleInput_WeightOne_Inner(vnet, actFn);
        }

        [Fact]
        public void TwoInputs_WeightHalf()
        {
            var connList = new List<WeightedDirectedConnection<double>>
            {
                new WeightedDirectedConnection<double>(0, 2, 0.5),
                new WeightedDirectedConnection<double>(1, 2, 0.5)
            };

            // Create graph.
            var digraph = WeightedDirectedGraphAcyclicBuilder<double>.Create(connList, 2, 1);

            // Create neural net and run tests.
            var actFn = new Logistic();
            var net = new NeuralNetAcyclic(digraph, actFn.Fn);
            TwoInputs_WeightHalf_Inner(net, actFn);

            // Create vectorized neural net and run tests.
            var vnet = new NeuralNet.Double.Vectorized.NeuralNetAcyclic(digraph, actFn.Fn);
            TwoInputs_WeightHalf_Inner(vnet, actFn);
        }

        [Fact]
        public void HiddenNode()
        {
            var connList = new List<WeightedDirectedConnection<double>>
            {
                new WeightedDirectedConnection<double>(0, 3, 0.5),
                new WeightedDirectedConnection<double>(1, 3, 0.5),
                new WeightedDirectedConnection<double>(3, 2, 2.0)
            };

            // Create graph.
            var digraph = WeightedDirectedGraphAcyclicBuilder<double>.Create(connList, 2, 1);

            // Create neural net and run tests.
            var actFn = new Logistic();
            var net = new NeuralNetAcyclic(digraph, actFn.Fn);
            HiddenNode_Inner(net, actFn);

            // Create vectorized neural net and run tests.
            var vnet = new NeuralNet.Double.Vectorized.NeuralNetAcyclic(digraph, actFn.Fn);
            HiddenNode_Inner(vnet, actFn);
        }

        [Fact]
        public void Complex_WeightOne()
        {
            var connList = new List<WeightedDirectedConnection<double>>
            {
                new WeightedDirectedConnection<double>(0, 4, 1.0),
                new WeightedDirectedConnection<double>(1, 4, 1.0),
                new WeightedDirectedConnection<double>(1, 5, 1.0),
                new WeightedDirectedConnection<double>(3, 4, 1.0),
                new WeightedDirectedConnection<double>(4, 2, 0.9),
                new WeightedDirectedConnection<double>(5, 3, 1.0)
            };

            // Create graph.
            var digraph = WeightedDirectedGraphAcyclicBuilder<double>.Create(connList, 2, 2);

            // Create neural net and run tests.
            var actFn = new Logistic();
            var net = new NeuralNetAcyclic(digraph, actFn.Fn);
            Complex_WeightOne_Inner(net, actFn);

             // Create vectorized neural net and run tests.
            var vnet = new NeuralNet.Double.Vectorized.NeuralNetAcyclic(digraph, actFn.Fn);
            Complex_WeightOne_Inner(vnet, actFn);
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
            var digraph = WeightedDirectedGraphAcyclicBuilder<double>.Create(connList, 3, 3);

            // Create neural net and run tests.
            var actFn = new Logistic();
            var net = new NeuralNetAcyclic(digraph, actFn.Fn);
            MultipleInputsOutputs_Inner(net, actFn);

            // Create vectorized neural net and run tests.
            var vnet = new NeuralNet.Double.Vectorized.NeuralNetAcyclic(digraph, actFn.Fn);
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
            net.Activate();
            Assert.Equal(0.5, net.OutputVector[0]);

            // Activate and test.
            net.InputVector[0] = 1.0;
            net.Activate();
            Assert.Equal(actFn.Fn(1), net.OutputVector[0]);

            // Activate and test.
            net.InputVector[0] = 10.0;
            net.Activate();
            Assert.Equal(actFn.Fn(10.0), net.OutputVector[0]);
        }

        private static void TwoInputs_WeightHalf_Inner(
            IBlackBox<double> net,
            IActivationFunction<double> actFn)
        {
            // Activate and test.
            net.InputVector[0] = 0.0;
            net.InputVector[1] = 0.0;
            net.Activate();
            Assert.Equal(0.5, net.OutputVector[0]);

            // Activate and test.
            net.InputVector[0] = 1.0;
            net.InputVector[1] = 2.0;
            net.Activate();
            Assert.Equal(actFn.Fn(1.5), net.OutputVector[0]);

            // Activate and test.
            net.InputVector[0] = 10.0;
            net.InputVector[1] = 20.0;
            net.Activate();
            Assert.Equal(actFn.Fn(15.0), net.OutputVector[0]);
        }

        private static void HiddenNode_Inner(
            IBlackBox<double> net,
            IActivationFunction<double> actFn)
        {
            // Activate and test.
            net.InputVector[0] = 0.0;
            net.InputVector[1] = 0.0;
            net.Activate();
            Assert.Equal(actFn.Fn(1.0), net.OutputVector[0]);

            // Activate and test.
            net.InputVector[0] = 0.5;
            net.InputVector[1] = 0.25;
            net.Activate();
            Assert.Equal(actFn.Fn(actFn.Fn(0.375) * 2.0), net.OutputVector[0]);
        }

        private static void Complex_WeightOne_Inner(
            IBlackBox<double> net,
            IActivationFunction<double> actFn)
        {
            // Activate and test.
            net.InputVector[0] = 0.5;
            net.InputVector[1] = 0.25;
            net.Activate();

            double output1 = actFn.Fn(actFn.Fn(0.25));
            Assert.Equal(output1, net.OutputVector[1]);

            double output0 = actFn.Fn(actFn.Fn(output1 + 0.5 + 0.25) * 0.9);
            Assert.Equal(output0, net.OutputVector[0]);
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
