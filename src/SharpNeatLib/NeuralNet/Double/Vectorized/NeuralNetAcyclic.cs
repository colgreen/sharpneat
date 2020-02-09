/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2019 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Numerics;
using SharpNeat.BlackBox;
using SharpNeat.Network.Acyclic;

namespace SharpNeat.NeuralNet.Double.Vectorized
{
    /// <summary>
    /// A version of <see cref="Double.NeuralNetAcyclic"/> that utilises some vectorized operations
    /// for improved performance on hardware platforms that support them.
    /// </summary>
    public sealed class NeuralNetAcyclic : IBlackBox<double>
    {
        #region Instance Fields

        // Connection arrays.
        readonly int[] _srcIdArr;
        readonly int[] _tgtIdArr;
        readonly double[] _weightArr;

        // Array of layer information.
        readonly LayerInfo[] _layerInfoArr;        

        // Node activation function.
        readonly VecFnSegment<double> _activationFn;

        // Node activation level array (used for both pre and post activation levels).
        readonly double[] _activationArr;

        // Wrappers over _activationArr that map between input/output vectors to the
        // corresponding underlying node activation levels.
        readonly VectorSegment<double> _inputVector;
        readonly IVector<double> _outputVector;

        // Convenient counts.
        readonly int _inputCount;
        readonly int _outputCount;

        // Connection inputs array.
        readonly double[] _conInputArr = new double[Vector<double>.Count];

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a AcyclicNeuralNet with the provided neural net definition parameters.
        /// </summary>
        /// <param name="digraph">Network structure definition</param>
        /// <param name="activationFn">Node activation function.</param>
        public NeuralNetAcyclic(
            WeightedDirectedGraphAcyclic<double> digraph,
            VecFnSegment<double> activationFn)
            : this(digraph, digraph.WeightArray, activationFn)
        {}

        /// <summary>
        /// Constructs a AcyclicNeuralNet with the provided neural net definition parameters.
        /// </summary>
        /// <param name="digraph">Network structure definition</param>
        /// <param name="weightArr">Connection weights array.</param>
        /// <param name="activationFn">Node activation function.</param>
        public NeuralNetAcyclic(
            DirectedGraphAcyclic digraph,
            double[] weightArr,
            VecFnSegment<double> activationFn)
        {
            // Store refs to network structure data.
            _srcIdArr = digraph.ConnectionIdArrays._sourceIdArr;
            _tgtIdArr = digraph.ConnectionIdArrays._targetIdArr;
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
            _inputVector = new VectorSegment<double>(_activationArr, 0, _inputCount);

            // Wrap the output nodes. Nodes have been sorted by depth within the network therefore the output
            // nodes can no longer be guaranteed to be in a contiguous segment at a fixed location. As such their
            // positions are indicated by outputNodeIdxArr, and so we package up this array with the node signal
            // array to abstract away the indirection described by outputNodeIdxArr.
            _outputVector = new MappingVector<double>(_activationArr, digraph.OutputNodeIdxArr);
        }

        #endregion

        #region IBlackBox

        /// <summary>
        /// Gets the number of input nodes.
        /// </summary>
        public int InputCount => _inputCount;

        /// <summary>
        /// Gets the number of output nodes.
        /// </summary>
        public int OutputCount => _outputCount;

        /// <summary>
        /// Gets an array for used for passing input signals to the network, i.e. the network input vector.
        /// </summary>
        public IVector<double> InputVector => _inputVector;

        /// <summary>
        /// Gets an array of output signals from the network, i.e. the network output vector.
        /// </summary>
        public IVector<double> OutputVector => _outputVector;

        /// <summary>
        /// Activate the network. Activation reads input signals from InputSignalArray and writes output signals
        /// to OutputSignalArray.
        /// </summary>
        public void Activate()
        {   
            // Reset hidden and output node activation levels, ready for next activation.
            // Note. this reset is performed here instead of after the below loop because this resets the output
            // node values, which are the outputs of the network as a whole following activation; hence
            // they need to be remain unchanged until they have been read by the caller of Activate().
            Array.Clear(_activationArr, _inputCount, _activationArr.Length - _inputCount);

            // Init vector related variables.
            int width = Vector<double>.Count;
            double[] conInputArr = _conInputArr;

            // Process all layers in turn.
            int conIdx = 0;
            int nodeIdx = _inputCount;

            // Loop through network layers.
            for(int layerIdx=1; layerIdx < _layerInfoArr.Length; layerIdx++)
            {
                LayerInfo layerInfo = _layerInfoArr[layerIdx-1];

                // Push signals through the previous layer's connections to the current layer's nodes.
                for(; conIdx <= layerInfo.EndConnectionIdx - width; conIdx += width) 
                {
                    // Load source node output values into a vector.
                    for(int k=0; k < width; k++) {
                        conInputArr[k] = _activationArr[_srcIdArr[conIdx + k]];
                    }
                    var conInputVec = new Vector<double>(conInputArr);

                    // Load connection weights into a vector.
                    var weightVec = new Vector<double>(_weightArr, conIdx);

                    // Multiply connection source inputs and connection weights.
                    var conOutputVec = conInputVec * weightVec;
                    
                    // Save/accumulate connection output values onto the connection target nodes.
                    for(int k=0; k < width; k++) {
                        _activationArr[_tgtIdArr[conIdx+k]] += conOutputVec[k];
                    }
                }

                // Loop remaining connections
                for(; conIdx < layerInfo.EndConnectionIdx; conIdx++) {
                    _activationArr[_tgtIdArr[conIdx]] = Math.FusedMultiplyAdd(_activationArr[_srcIdArr[conIdx]], _weightArr[conIdx], _activationArr[_tgtIdArr[conIdx]]);
                }

                // Activate current layer's nodes.
                //
                // Pass the pre-activation levels through the activation function.
                // Note. The resulting post-activation levels are stored in _activationArr.
                layerInfo = _layerInfoArr[layerIdx];
                _activationFn(_activationArr, nodeIdx, layerInfo.EndNodeIdx);

                // Update nodeIdx to point at first node in the next layer.
                nodeIdx = layerInfo.EndNodeIdx;
            }
        }

        /// <summary>
        /// Reset the network's internal state.
        /// </summary>
        public void ResetState()
        {
            // Unnecessary for this implementation. The node activation signal state is completely overwritten on each activation.
        }

        #endregion
    }
}
