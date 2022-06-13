// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Graphs;
using SharpNeat.Graphs.Acyclic;
using SharpNeat.IO.Models;

namespace SharpNeat.NeuralNets.IO;

/// <summary>
/// Static methods for converting <see cref="NetFileModel"/> to a neural network instance.
/// </summary>
public static class NeuralNetConverter
{
    // TODO: NeuralNetConverter unit tests.

    /// <summary>
    /// Convert a <see cref="NetFileModel"/> to a neural network instance.
    /// </summary>
    /// <param name="model">The <see cref="NetFileModel"/> instance to convert from.</param>
    /// <param name="enableHardwareAcceleratedNeuralNets">If true then hardware accelerated neural net
    /// implementations are used.</param>
    /// <param name="enableHardwareAcceleratedActivationFunctions">If true then hardware accelerated activation
    /// functions are used, where available.</param>
    /// <returns>A new neural net instance.</returns>
    public static IBlackBox<double> ToNeuralNet(
        NetFileModel model,
        bool enableHardwareAcceleratedNeuralNets = false,
        bool enableHardwareAcceleratedActivationFunctions = false)
    {
        ArgumentNullException.ThrowIfNull(model);

        var actFnlib = new DefaultActivationFunctionFactory<double>(
            enableHardwareAcceleratedActivationFunctions);

        return ToNeuralNet(model, actFnlib, enableHardwareAcceleratedNeuralNets);
    }

    /// <summary>
    /// Convert a <see cref="NetFileModel"/> to a neural network instance.
    /// </summary>
    /// <param name="model">The <see cref="NetFileModel"/> instance to convert from.</param>
    /// <param name="activationFnLib">Activation function library.</param>
    /// <param name="enableHardwareAcceleratedNeuralNets">If true then hardware accelerated neural net
    /// implementations are used.</param>
    /// <returns>A new neural net instance.</returns>
    public static IBlackBox<double> ToNeuralNet(
        NetFileModel model,
        IActivationFunctionFactory<double> activationFnLib,
        bool enableHardwareAcceleratedNeuralNets = false)
    {
        ArgumentNullException.ThrowIfNull(model);

        if(model.IsAcyclic)
        {
            return ToAcyclicNeuralNet(
                model, activationFnLib,
                enableHardwareAcceleratedNeuralNets);
        }
        else
        {
            return ToCyclicNeuralNet(
                model, activationFnLib,
                enableHardwareAcceleratedNeuralNets);
        }
    }

    /// <summary>
    /// Convert a <see cref="NetFileModel"/> to an acyclic neural network instance.
    /// </summary>
    /// /// <param name="model">The <see cref="NetFileModel"/> instance to convert from.</param>
    /// <param name="enableHardwareAcceleratedNeuralNets">If true then hardware accelerated neural net
    /// implementations are used.</param>
    /// <param name="enableHardwareAcceleratedActivationFunctions">If true then hardware accelerated activation
    /// functions are used, where available.</param>
    /// <returns>A new acyclic neural net instance.</returns>
    public static IBlackBox<double> ToAcyclicNeuralNet(
        NetFileModel model,
        bool enableHardwareAcceleratedNeuralNets = false,
        bool enableHardwareAcceleratedActivationFunctions = false)
    {
        var actFnlib = new DefaultActivationFunctionFactory<double>(
            enableHardwareAcceleratedActivationFunctions);

        return ToAcyclicNeuralNet(model, actFnlib, enableHardwareAcceleratedNeuralNets);
    }

    /// <summary>
    /// Convert a <see cref="NetFileModel"/> to an acyclic neural network instance.
    /// </summary>
    /// <param name="model">The <see cref="NetFileModel"/> instance to convert from.</param>
    /// <param name="activationFnLib">Activation function library.</param>
    /// <param name="enableHardwareAcceleratedNeuralNets">If true then hardware accelerated neural net
    /// implementations are used.</param>
    /// <returns>A new acyclic neural net instance.</returns>
    public static IBlackBox<double> ToAcyclicNeuralNet(
        NetFileModel model,
        IActivationFunctionFactory<double> activationFnLib,
        bool enableHardwareAcceleratedNeuralNets = false)
    {
        // Perform some basic validation checks.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(activationFnLib);

        if (!model.IsAcyclic)
            throw new ArgumentException("Invalid model; the model must describe an acyclic network.", nameof(model));

        // Get an activation function instance.
        IActivationFunction<double> actFn = ResolveActivationFn(model, activationFnLib);

        // Compile the digraph weighted connections.
        WeightedDirectedConnection<double>[] conns = BuildConnections(model);

        // Create a weighted digraph.
        var weightedDigraphAcyclic = WeightedDirectedGraphAcyclicBuilder<double>.Create(
            conns, model.InputCount, model.OutputCount);

        // Create a working neural net.
        if (!enableHardwareAcceleratedNeuralNets)
        {
            return new Double.NeuralNetAcyclic(
                weightedDigraphAcyclic,
                actFn.Fn);
        }
        else
        {
            return new Double.Vectorized.NeuralNetAcyclic(
                weightedDigraphAcyclic,
                actFn.Fn);
        }
    }

    /// <summary>
    /// Convert a <see cref="NetFileModel"/> to a cyclic neural network instance.
    /// </summary>
    /// /// <param name="model">The <see cref="NetFileModel"/> instance to convert from.</param>
    /// <param name="enableHardwareAcceleratedNeuralNets">If true then hardware accelerated neural net
    /// implementations are used.</param>
    /// <param name="enableHardwareAcceleratedActivationFunctions">If true then hardware accelerated activation
    /// functions are used, where available.</param>
    /// <returns>A new cyclic neural net instance.</returns>
    public static IBlackBox<double> ToCyclicNeuralNet(
        NetFileModel model,
        bool enableHardwareAcceleratedNeuralNets = false,
        bool enableHardwareAcceleratedActivationFunctions = false)
    {
        var actFnlib = new DefaultActivationFunctionFactory<double>(
            enableHardwareAcceleratedActivationFunctions);

        return ToCyclicNeuralNet(model, actFnlib, enableHardwareAcceleratedNeuralNets);
    }

    /// <summary>
    /// Convert a <see cref="NetFileModel"/> to a cyclic neural network instance.
    /// </summary>
    /// <param name="model">The <see cref="NetFileModel"/> instance to convert from.</param>
    /// <param name="activationFnLib">Activation function library.</param>
    /// <param name="enableHardwareAcceleratedNeuralNets">If true then hardware accelerated neural net
    /// implementations are used.</param>
    /// <returns>A new cyclic neural net instance.</returns>
    public static IBlackBox<double> ToCyclicNeuralNet(
        NetFileModel model,
        IActivationFunctionFactory<double> activationFnLib,
        bool enableHardwareAcceleratedNeuralNets = false)
    {
        // Perform some basic validation checks.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(activationFnLib);

        if(model.IsAcyclic)
            throw new ArgumentException("Invalid model; the model must describe a cyclic network.", nameof(model));

        // Get an activation function instance.
        IActivationFunction<double> actFn = ResolveActivationFn(model, activationFnLib);

        // Compile the digraph weighted connections.
        WeightedDirectedConnection<double>[] conns = BuildConnections(model);

        // Create a weighted digraph.
        var weightedDigraph = WeightedDirectedGraphBuilder<double>.Create(
            conns, model.InputCount, model.OutputCount);

        // Create a working neural net.
        if(!enableHardwareAcceleratedNeuralNets)
        {
            return new Double.NeuralNetCyclic(
                weightedDigraph,
                actFn.Fn,
                model.CyclesPerActivation);
        }
        else
        {
            return new Double.Vectorized.NeuralNetCyclic(
                weightedDigraph,
                actFn.Fn,
                model.CyclesPerActivation);
        }
    }

    #region Private Static Methods

    private static IActivationFunction<double> ResolveActivationFn(
        NetFileModel model,
        IActivationFunctionFactory<double> activationFnLib)
    {
        if(model.ActivationFns.Count == 0)
            throw new ArgumentException("Invalid model; no activation function defined.", nameof(model));

        ActivationFnLine actFnLine = model.ActivationFns[0];
        if(actFnLine.Id != 0)
            throw new ArgumentException("Invalid model; The first activation function must have an ID of 0.", nameof(model));

        // Get an activation function instance.
        IActivationFunction<double> actFn = activationFnLib.GetActivationFunction(actFnLine.Code);
        return actFn;
    }

    private static WeightedDirectedConnection<double>[] BuildConnections(
        NetFileModel model)
    {
        var connLines = model.Connections;
        int count = connLines.Count;
        var conns = new WeightedDirectedConnection<double>[count];

        for(int i=0; i < count; i++)
        {
            ConnectionLine connLine = connLines[i];
            conns[i] = new WeightedDirectedConnection<double>(
                connLine.SourceId, connLine.TargetId, connLine.Weight);
        }

        Array.Sort(conns);
        return conns;
    }

    #endregion
}
