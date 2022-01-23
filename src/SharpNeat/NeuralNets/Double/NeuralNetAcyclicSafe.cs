/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Buffers;
using System.Diagnostics;
using SharpNeat.BlackBox;
using SharpNeat.Graphs;
using SharpNeat.Graphs.Acyclic;

namespace SharpNeat.NeuralNets.Double
{
    /// <summary>
    /// This class is functionally equivalent to <see cref="NeuralNetAcyclic"/>, but doesn't use any of the unsafe
    /// memory pointer techniques used in that class, and therefore this class is much slower.
    ///
    /// This class is intended to be used as a safe reference implementation that can be used when testing or
    /// debugging <see cref="NeuralNetAcyclic"/>.
    /// </summary>
    public sealed class NeuralNetAcyclicSafe : IBlackBox<double>
    {
        #region Instance Fields

        // Connection arrays.
        readonly ConnectionIds _connIds;
        readonly double[] _weightArr;

        // Array of layer information.
        readonly LayerInfo[] _layerInfoArr;

        // Node activation function.
        readonly VecFn<double> _activationFn;

        // Node activation level array (used for both pre and post activation levels).
        readonly double[] _activationArr;

        // Convenient counts.
        readonly int _inputCount;
        readonly int _outputCount;
        volatile bool _isDisposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a AcyclicNeuralNet with the provided neural net definition parameters.
        /// </summary>
        /// <param name="digraph">Network structure definition.</param>
        /// <param name="activationFn">Node activation function.</param>
        public NeuralNetAcyclicSafe(
            WeightedDirectedGraphAcyclic<double> digraph,
            VecFn<double> activationFn)
            : this(digraph, digraph.WeightArray, activationFn)
        { }

        /// <summary>
        /// Constructs a AcyclicNeuralNet with the provided neural net definition parameters.
        /// </summary>
        /// <param name="digraph">Network structure definition.</param>
        /// <param name="weightArr">Connection weights array.</param>
        /// <param name="activationFn">Node activation function.</param>
        public NeuralNetAcyclicSafe(
            DirectedGraphAcyclic digraph,
            double[] weightArr,
            VecFn<double> activationFn)
        {
            Debug.Assert(digraph.ConnectionIds.GetSourceIdSpan().Length == weightArr.Length);

            // Store refs to network structure data.
            _connIds = digraph.ConnectionIds;
            _weightArr = weightArr;
            _layerInfoArr = digraph.LayerArray;

            // Store network activation function.
            _activationFn = activationFn;

            // Store input/output node counts.
            _inputCount = digraph.InputCount;
            _outputCount = digraph.OutputCount;

            // Create working array for node activation signals.
            _activationArr = new double[digraph.TotalNodeCount];

            // Wrap a sub-range of the _activationArr that holds the activation values for the input nodes.
            this.Inputs = new Memory<double>(_activationArr, 0, _inputCount);

            // Wrap the output nodes. Nodes have been sorted by depth within the network therefore the output
            // nodes can no longer be guaranteed to be in a contiguous segment at a fixed location. As such their
            // positions are indicated by outputNodeIdxArr, and so we package up this array with the node signal
            // array to abstract away the indirection described by outputNodeIdxArr.
            this.Outputs = new MappingVector<double>(_activationArr, digraph.OutputNodeIdxArr);
        }

        #endregion

        #region IBlackBox

        /// <summary>
        /// Gets a memory segment used for passing input signals to the network, i.e. the network input vector.
        /// </summary>
        public Memory<double> Inputs { get; }

        /// <summary>
        /// Gets an array of output signals from the network, i.e. the network output vector.
        /// </summary>
        public IVector<double> Outputs { get; }

        /// <summary>
        /// Activate the network. Activation reads input signals from InputSignalArray and writes output signals
        /// to OutputSignalArray.
        /// </summary>
        public void Activate()
        {
            ReadOnlySpan<int> srcIds = _connIds.GetSourceIdSpan();
            ReadOnlySpan<int> tgtIds = _connIds.GetTargetIdSpan();
            ReadOnlySpan<double> weights = _weightArr.AsSpan();
            Span<double> activations = _activationArr.AsSpan();

            // Reset hidden and output node activation levels, ready for next activation.
            // Note. this reset is performed here instead of after the below loop because this resets the output
            // node values, which are the outputs of the network as a whole following activation; hence
            // they need to be remain unchanged until they have been read by the caller of Activate().
            activations.Slice(_inputCount).Clear();

            // Process all layers in turn.
            int conIdx = 0;
            int nodeIdx = _inputCount;

            // Loop through network layers.
            for(int layerIdx = 0; layerIdx < _layerInfoArr.Length - 1; layerIdx++)
            {
                LayerInfo layerInfo = _layerInfoArr[layerIdx];

                // Push signals through the current layer's connections to the target nodes (that are all in 'downstream' layers).
                for(; conIdx < layerInfo.EndConnectionIdx; conIdx++)
                {
                    // Get the connection source signal, multiply it by the connection weight, add the result
                    // to the target node's current pre-activation level, and store the result.
                    activations[tgtIds[conIdx]] =
                        Math.FusedMultiplyAdd(
                            activations[srcIds[conIdx]],
                            weights[conIdx],
                            activations[tgtIds[conIdx]]);
                }

                // Activate the next layer's nodes. This is possible because we know that all connections that
                // target these nodes have been processed, either during processing on the current layer's
                // connections, or earlier layers. This means that the final output value/signal (i.e post
                // activation function output) is available for all connections and nodes in the lower/downstream
                // layers.
                //
                // Pass the pre-activation levels through the activation function.
                // Note. The resulting post-activation levels are stored in _activationArr.
                layerInfo = _layerInfoArr[layerIdx + 1];
                _activationFn(
                    ref activations[nodeIdx],
                    layerInfo.EndNodeIdx - nodeIdx);

                // Update nodeIdx to point at first node in the next layer.
                nodeIdx = layerInfo.EndNodeIdx;
            }
        }

        /// <summary>
        /// Reset the network's internal state.
        /// </summary>
        public void Reset()
        {
            // Unnecessary for this implementation. The node activation signal state is completely overwritten on each activation.
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Releases both managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if(!_isDisposed)
            {
                _isDisposed = true;
                ArrayPool<double>.Shared.Return(_activationArr);
            }
        }

        #endregion
    }
}
