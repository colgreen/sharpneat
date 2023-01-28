using Redzen.Collections;
using SharpNeat.Graphs;
using SharpNeat.Graphs.Acyclic;
using SharpNeat.NeuralNets.Double.ActivationFunctions;
using Xunit;

namespace SharpNeat.NeuralNets.Double;

public class NeuralNetAcyclicTests
{
    [Fact]
    public void SingleInput_WeightZero()
    {
        var connList = new LightweightList<WeightedDirectedConnection<double>> {
            new WeightedDirectedConnection<double>(0,1,0.0)
        };
        var connSpan = connList.AsSpan();

        // Create graph.
        var digraph = WeightedDirectedGraphAcyclicBuilder<double>.Create(connSpan, 1, 1);

        // Create neural net and run tests.
        var actFn = new Logistic();
        using var net = new NeuralNetAcyclic(digraph, actFn.Fn);
        SingleInput_WeightZero_Inner(net);

        // Create vectorized neural net and run tests.
        using var vnet = new Vectorized.NeuralNetAcyclic(digraph, actFn.Fn);
        SingleInput_WeightZero_Inner(vnet);
    }

    [Fact]
    public void SingleInput_WeightOne()
    {
        var connList = new LightweightList<WeightedDirectedConnection<double>> {
            new WeightedDirectedConnection<double>(0,1,1.0)
        };
        var connSpan = connList.AsSpan();

        // Create graph.
        var digraph = WeightedDirectedGraphAcyclicBuilder<double>.Create(connSpan, 1, 1);

        // Create neural net and run tests.
        var actFn = new Logistic();
        using var net = new NeuralNetAcyclic(digraph, actFn.Fn);
        SingleInput_WeightOne_Inner(net, actFn);

        // Create vectorized neural net and run tests.
        using var vnet = new Vectorized.NeuralNetAcyclic(digraph, actFn.Fn);
        SingleInput_WeightOne_Inner(vnet, actFn);
    }

    [Fact]
    public void TwoInputs_WeightHalf()
    {
        var connList = new LightweightList<WeightedDirectedConnection<double>>
        {
            new WeightedDirectedConnection<double>(0, 2, 0.5),
            new WeightedDirectedConnection<double>(1, 2, 0.5)
        };
        var connSpan = connList.AsSpan();

        // Create graph.
        var digraph = WeightedDirectedGraphAcyclicBuilder<double>.Create(connSpan, 2, 1);

        // Create neural net and run tests.
        var actFn = new Logistic();
        using var net = new NeuralNetAcyclic(digraph, actFn.Fn);
        TwoInputs_WeightHalf_Inner(net, actFn);

        // Create vectorized neural net and run tests.
        using var vnet = new Vectorized.NeuralNetAcyclic(digraph, actFn.Fn);
        TwoInputs_WeightHalf_Inner(vnet, actFn);
    }

    [Fact]
    public void HiddenNode()
    {
        var connList = new LightweightList<WeightedDirectedConnection<double>>
        {
            new WeightedDirectedConnection<double>(0, 3, 0.5),
            new WeightedDirectedConnection<double>(1, 3, 0.5),
            new WeightedDirectedConnection<double>(3, 2, 2.0)
        };
        var connSpan = connList.AsSpan();

        // Create graph.
        var digraph = WeightedDirectedGraphAcyclicBuilder<double>.Create(connSpan, 2, 1);

        // Create neural net and run tests.
        var actFn = new Logistic();
        using var net = new NeuralNetAcyclic(digraph, actFn.Fn);
        HiddenNode_Inner(net, actFn);

        // Create vectorized neural net and run tests.
        using var vnet = new Vectorized.NeuralNetAcyclic(digraph, actFn.Fn);
        HiddenNode_Inner(vnet, actFn);
    }

    [Fact]
    public void Complex_WeightOne()
    {
        var connList = new LightweightList<WeightedDirectedConnection<double>>
        {
            new WeightedDirectedConnection<double>(0, 4, 1.0),
            new WeightedDirectedConnection<double>(1, 4, 1.0),
            new WeightedDirectedConnection<double>(1, 5, 1.0),
            new WeightedDirectedConnection<double>(3, 4, 1.0),
            new WeightedDirectedConnection<double>(4, 2, 0.9),
            new WeightedDirectedConnection<double>(5, 3, 1.0)
        };
        var connSpan = connList.AsSpan();

        // Create graph.
        var digraph = WeightedDirectedGraphAcyclicBuilder<double>.Create(connSpan, 2, 2);

        // Create neural net and run tests.
        var actFn = new Logistic();
        using var net = new NeuralNetAcyclic(digraph, actFn.Fn);
        Complex_WeightOne_Inner(net, actFn);

        // Create vectorized neural net and run tests.
        using var vnet = new Vectorized.NeuralNetAcyclic(digraph, actFn.Fn);
        Complex_WeightOne_Inner(vnet, actFn);
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
        var digraph = WeightedDirectedGraphAcyclicBuilder<double>.Create(connSpan, 3, 3);

        // Create neural net and run tests.
        var actFn = new Logistic();
        using var net = new NeuralNetAcyclic(digraph, actFn.Fn);
        MultipleInputsOutputs_Inner(net, actFn);

        // Create vectorized neural net and run tests.
        using var vnet = new Vectorized.NeuralNetAcyclic(digraph, actFn.Fn);
        MultipleInputsOutputs_Inner(vnet, actFn);
    }

    #region Private Static Methods

    private static void SingleInput_WeightZero_Inner(
        IBlackBox<double> net)
    {
        var inputs = net.Inputs.Span;
        var outputs = net.Outputs.Span;

        // Note. The single connection weight is zero, so the input value has no affect.
        // Activate and test.
        inputs[0] = 100.0;
        net.Activate();
        Assert.Equal(0.5, outputs[0]);

        // Activate and test.
        inputs[0] = 0;
        net.Activate();
        Assert.Equal(0.5, outputs[0]);

        // Activate and test.
        inputs[0] = -100;
        net.Activate();
        Assert.Equal(0.5, outputs[0]);
    }

    private static void SingleInput_WeightOne_Inner(
        IBlackBox<double> net,
        IActivationFunction<double> actFn)
    {
        double x;
        var inputs = net.Inputs.Span;
        var outputs = net.Outputs.Span;

        // Activate and test.
        inputs[0] = 0.0;
        net.Activate();
        Assert.Equal(0.5, outputs[0]);

        // Activate and test.
        inputs[0] = 1.0;
        net.Activate();
        x=1.0; actFn.Fn(ref x);
        Assert.Equal(x, outputs[0]);

        // Activate and test.
        inputs[0] = 10.0;
        net.Activate();
        x=10.0; actFn.Fn(ref x);
        Assert.Equal(x, outputs[0]);
    }

    private static void TwoInputs_WeightHalf_Inner(
        IBlackBox<double> net,
        IActivationFunction<double> actFn)
    {
        double x;
        var inputs = net.Inputs.Span;
        var outputs = net.Outputs.Span;

        // Activate and test.
        inputs[0] = 0.0;
        inputs[1] = 0.0;
        net.Activate();
        Assert.Equal(0.5, outputs[0]);

        // Activate and test.
        inputs[0] = 1.0;
        inputs[1] = 2.0;
        net.Activate();
        x = 1.5; actFn.Fn(ref x);
        Assert.Equal(x, outputs[0]);

        // Activate and test.
        inputs[0] = 10.0;
        inputs[1] = 20.0;
        net.Activate();
        x = 15.0; actFn.Fn(ref x);
        Assert.Equal(x, outputs[0]);
    }

    private static void HiddenNode_Inner(
        IBlackBox<double> net,
        IActivationFunction<double> actFn)
    {
        double x;
        var inputs = net.Inputs.Span;
        var outputs = net.Outputs.Span;

        // Activate and test.
        inputs[0] = 0.0;
        inputs[1] = 0.0;
        net.Activate();
        x = 1.0; actFn.Fn(ref x);
        Assert.Equal(x, outputs[0]);

        // Activate and test.
        inputs[0] = 0.5;
        inputs[1] = 0.25;
        net.Activate();
        x = 0.375; actFn.Fn(ref x);
        x *= 2; actFn.Fn(ref x);
        Assert.Equal(x, outputs[0]);
    }

    private static void Complex_WeightOne_Inner(
        IBlackBox<double> net,
        IActivationFunction<double> actFn)
    {
        double x;
        var inputs = net.Inputs.Span;
        var outputs = net.Outputs.Span;

        // Activate and test.
        inputs[0] = 0.5;
        inputs[1] = 0.25;
        net.Activate();

        x = 0.25; actFn.Fn(ref x); actFn.Fn(ref x);
        double output1 = x;
        Assert.Equal(output1, outputs[1]);

        x = output1 + 0.5 + 0.25; actFn.Fn(ref x);
        x *= 0.9; actFn.Fn(ref x);
        double output0 = x;
        Assert.Equal(output0, outputs[0]);
    }

    private static void MultipleInputsOutputs_Inner(
        IBlackBox<double> net,
        IActivationFunction<double> actFn)
    {
        double x;
        var inputs = net.Inputs.Span;
        var outputs = net.Outputs.Span;

        // Activate and test.
        inputs[0] = 1.0;
        inputs[1] = 2.0;
        inputs[2] = 3.0;
        net.Activate();

        x = 2.0; actFn.Fn(ref x);
        Assert.Equal(x, outputs[0]);

        x = 3.0; actFn.Fn(ref x);
        Assert.Equal(x, outputs[1]);

        x = 1.0; actFn.Fn(ref x);
        Assert.Equal(x, outputs[2]);
    }

    #endregion
}
