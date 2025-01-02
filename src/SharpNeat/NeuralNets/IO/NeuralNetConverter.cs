// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.Graphs.Acyclic;
using SharpNeat.IO.Models;
using SharpNeat.NeuralNets.ActivationFunctions;

namespace SharpNeat.NeuralNets.IO;

/// <summary>
/// Static methods for converting <see cref="NetFileModel"/> to a neural network instance.
/// </summary>
public static class NeuralNetConverter
{
    /// <summary>
    /// Convert a <see cref="NetFileModel"/> to a neural network instance.
    /// </summary>
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    /// <param name="model">The <see cref="NetFileModel"/> instance to convert from.</param>
    /// <param name="enableHardwareAcceleratedNeuralNets">If true then hardware accelerated neural net
    /// implementations are used.</param>
    /// <param name="enableHardwareAcceleratedActivationFunctions">If true then hardware accelerated activation
    /// functions are used, where available.</param>
    /// <returns>A new neural net instance.</returns>
    public static IBlackBox<TScalar> ToNeuralNet<TScalar>(
        NetFileModel model,
        bool enableHardwareAcceleratedNeuralNets = false,
        bool enableHardwareAcceleratedActivationFunctions = false)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        ArgumentNullException.ThrowIfNull(model);

        var actFnlib = new DefaultActivationFunctionFactory<TScalar>(
            enableHardwareAcceleratedActivationFunctions);

        return ToNeuralNet(model, actFnlib, enableHardwareAcceleratedNeuralNets);
    }

    /// <summary>
    /// Convert a <see cref="NetFileModel"/> to a neural network instance.
    /// </summary>
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    /// <param name="model">The <see cref="NetFileModel"/> instance to convert from.</param>
    /// <param name="activationFnLib">Activation function library.</param>
    /// <param name="enableHardwareAcceleratedNeuralNets">If true then hardware accelerated neural net
    /// implementations are used.</param>
    /// <returns>A new neural net instance.</returns>
    public static IBlackBox<TScalar> ToNeuralNet<TScalar>(
        NetFileModel model,
        IActivationFunctionFactory<TScalar> activationFnLib,
        bool enableHardwareAcceleratedNeuralNets = false)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
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
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    /// <param name="model">The <see cref="NetFileModel"/> instance to convert from.</param>
    /// <param name="enableHardwareAcceleratedNeuralNets">If true then hardware accelerated neural net
    /// implementations are used.</param>
    /// <param name="enableHardwareAcceleratedActivationFunctions">If true then hardware accelerated activation
    /// functions are used, where available.</param>
    /// <returns>A new acyclic neural net instance.</returns>
    public static IBlackBox<TScalar> ToAcyclicNeuralNet<TScalar>(
        NetFileModel model,
        bool enableHardwareAcceleratedNeuralNets = false,
        bool enableHardwareAcceleratedActivationFunctions = false)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        var actFnlib = new DefaultActivationFunctionFactory<TScalar>(
            enableHardwareAcceleratedActivationFunctions);

        return ToAcyclicNeuralNet(model, actFnlib, enableHardwareAcceleratedNeuralNets);
    }

    /// <summary>
    /// Convert a <see cref="NetFileModel"/> to an acyclic neural network instance.
    /// </summary>
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    /// <param name="model">The <see cref="NetFileModel"/> instance to convert from.</param>
    /// <param name="activationFnLib">Activation function library.</param>
    /// <param name="enableHardwareAcceleratedNeuralNets">If true then hardware accelerated neural net
    /// implementations are used.</param>
    /// <returns>A new acyclic neural net instance.</returns>
    public static IBlackBox<TScalar> ToAcyclicNeuralNet<TScalar>(
        NetFileModel model,
        IActivationFunctionFactory<TScalar> activationFnLib,
        bool enableHardwareAcceleratedNeuralNets = false)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Perform some basic validation checks.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(activationFnLib);

        if (!model.IsAcyclic)
            throw new ArgumentException("Invalid model; the model must describe an acyclic network.", nameof(model));

        // Get an activation function instance.
        IActivationFunction<TScalar> actFn = ResolveActivationFn(model, activationFnLib);

        // Compile the digraph weighted connections.
        WeightedDirectedConnection<TScalar>[] conns = BuildConnections<TScalar>(model);

        // Create a weighted digraph.
        var weightedDigraphAcyclic = WeightedDirectedGraphAcyclicBuilder<TScalar>.Create(
            conns, model.InputCount, model.OutputCount);

        // Create a working neural net.
        if (!enableHardwareAcceleratedNeuralNets)
        {
            return new NeuralNetAcyclic<TScalar>(
                weightedDigraphAcyclic,
                actFn.Fn);
        }
        else
        {
            return new Vectorized.NeuralNetAcyclic<TScalar>(
                weightedDigraphAcyclic,
                actFn.Fn);
        }
    }

    /// <summary>
    /// Convert a <see cref="NetFileModel"/> to a cyclic neural network instance.
    /// </summary>
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    /// <param name="model">The <see cref="NetFileModel"/> instance to convert from.</param>
    /// <param name="enableHardwareAcceleratedNeuralNets">If true then hardware accelerated neural net
    /// implementations are used.</param>
    /// <param name="enableHardwareAcceleratedActivationFunctions">If true then hardware accelerated activation
    /// functions are used, where available.</param>
    /// <returns>A new cyclic neural net instance.</returns>
    public static IBlackBox<TScalar> ToCyclicNeuralNet<TScalar>(
        NetFileModel model,
        bool enableHardwareAcceleratedNeuralNets = false,
        bool enableHardwareAcceleratedActivationFunctions = false)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        var actFnlib = new DefaultActivationFunctionFactory<TScalar>(
            enableHardwareAcceleratedActivationFunctions);

        return ToCyclicNeuralNet(model, actFnlib, enableHardwareAcceleratedNeuralNets);
    }

    /// <summary>
    /// Convert a <see cref="NetFileModel"/> to a cyclic neural network instance.
    /// </summary>
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    /// <param name="model">The <see cref="NetFileModel"/> instance to convert from.</param>
    /// <param name="activationFnLib">Activation function library.</param>
    /// <param name="enableHardwareAcceleratedNeuralNets">If true then hardware accelerated neural net
    /// implementations are used.</param>
    /// <returns>A new cyclic neural net instance.</returns>
    public static IBlackBox<TScalar> ToCyclicNeuralNet<TScalar>(
        NetFileModel model,
        IActivationFunctionFactory<TScalar> activationFnLib,
        bool enableHardwareAcceleratedNeuralNets = false)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Perform some basic validation checks.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(activationFnLib);

        if(model.IsAcyclic)
            throw new ArgumentException("Invalid model; the model must describe a cyclic network.", nameof(model));

        // Get an activation function instance.
        IActivationFunction<TScalar> actFn = ResolveActivationFn(model, activationFnLib);

        // Compile the digraph weighted connections.
        WeightedDirectedConnection<TScalar>[] conns = BuildConnections<TScalar>(model);

        // Create a weighted digraph.
        var weightedDigraph = WeightedDirectedGraphBuilder<TScalar>.Create(
            conns, model.InputCount, model.OutputCount);

        // Create a working neural net.
        if(!enableHardwareAcceleratedNeuralNets)
        {
            return new NeuralNetCyclic<TScalar>(
                weightedDigraph,
                actFn.Fn,
                model.CyclesPerActivation);
        }
        else
        {
            return new Vectorized.NeuralNetCyclic<TScalar>(
                weightedDigraph,
                actFn.Fn,
                model.CyclesPerActivation);
        }
    }

    #region Private Static Methods

    private static IActivationFunction<TScalar> ResolveActivationFn<TScalar>(
        NetFileModel model,
        IActivationFunctionFactory<TScalar> activationFnLib)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        if(model.ActivationFns.Count == 0)
            throw new ArgumentException("Invalid model; no activation function defined.", nameof(model));

        ActivationFnLine actFnLine = model.ActivationFns[0];
        if(actFnLine.Id != 0)
            throw new ArgumentException("Invalid model; The first activation function must have an ID of 0.", nameof(model));

        // Get an activation function instance.
        IActivationFunction<TScalar> actFn = activationFnLib.GetActivationFunction(actFnLine.Code);
        return actFn;
    }

    private static WeightedDirectedConnection<TScalar>[] BuildConnections<TScalar>(
        NetFileModel model)
        where TScalar : unmanaged, INumber<TScalar>
    {
        var connLines = model.Connections;
        int count = connLines.Count;
        var conns = new WeightedDirectedConnection<TScalar>[count];

        for(int i=0; i < count; i++)
        {
            ConnectionLine connLine = connLines[i];
            conns[i] = new WeightedDirectedConnection<TScalar>(
                connLine.SourceId, connLine.TargetId, TScalar.CreateChecked(connLine.Weight));
        }

        Array.Sort(conns);
        return conns;
    }

    #endregion
}
