/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using SharpNeat.Network;
using SharpNeat.Network2;

namespace SharpNeat.Phenomes.NeuralNets.Acyclic
{
    /// <summary>
    /// An AcyclicNetwork that allows for a different activation function at each node; this
    /// quality is required for CPPN networks.
    /// </summary>
    public class HeterogeneousAcyclicNeuralNet : IBlackBox<double>
    {
    //=== Fixed data. Network structure and activation functions/data.

        // Array of node activation functions.
        readonly Func<double,double>[] _activationFnArr;
        
        // Connection arrays.
        readonly DirectedConnection[] _connArr;
        readonly double[] _weightArr;

        // Array of layer information. Feed-forward-only network activation can be performed most 
        // efficiently by propagating signals through the network one layer at a time.
        readonly LayerInfo[] _layerInfoArr;

    //=== Working data.
        
        // Array of node activation signals.
        readonly double[] _activationArr;

    //=== Misc.
        // Wrappers over _activationArr that map between black box inputs/outputs to the
        // corresponding underlying node activation levels.
        readonly SignalArray<double> _inputSignalArrayWrapper;
        readonly MappingSignalArray<double> _outputSignalArrayWrapper;

        // Convenient counts.
        readonly int _inputNodeCount;
        readonly int _outputNodeCount;

        #region Constructor

        /// <summary>
        /// Construct an AcyclicNetwork with provided network definition data structures.
        /// </summary>
        /// <param name="activationFnArr">Array of node activation functions.</param>
        /// <param name="connInfoArr">Array of connections.</param>
        /// <param name="layerInfoArr">Array of layer information.</param>
        /// <param name="outputNodeIdxArr">An array that specifies the index of each output neuron within _activationArr.
        /// This is necessary because the neurons have been sorted by their depth in the network structure and are therefore
        /// no longer in their original positions. Note however that the input neurons *are* in their original 
        /// positions as they are defined as being at depth zero.</param>
        /// <param name="nodeCount">Number of nodes in the network.</param>
        /// <param name="inputNodeCount">Number of input nodes in the network.</param>
        /// <param name="outputNodeCount">Number of output nodes in the network.</param>
        /// <param name="boundedOutput">Indicates that the output values at the output nodes should be bounded to the interval [0,1]</param>
        public HeterogeneousAcyclicNeuralNet(Func<double,double>[] activationFnArr,
                                  DirectedConnection[] connArr,
                                  double[] weightArr,
                                  LayerInfo[] layerInfoArr,
                                  int[] outputNodeIdxArr,
                                  int nodeCount,
                                  int inputNodeCount,
                                  int outputNodeCount,
                                  bool boundedOutput)
        {
            // Store refs to network structure data.
            _activationFnArr = activationFnArr;
            _connArr = connArr;
            _weightArr = weightArr;
            _layerInfoArr = layerInfoArr;

            // Create working array for node activation signals.
            _activationArr = new double[nodeCount];

            // Wrap a sub-range of the _activationArr that holds the activation values for the input nodes.
            _inputSignalArrayWrapper = new SignalArray<double>(_activationArr, 0, inputNodeCount);

            // Wrap the output nodes. Nodes have been sorted by depth within the network therefore the output
            // nodes can no longer be guaranteed to be in a contiguous segment at a fixed location. As such their
            // positions are indicated by outputNodeIdxArr, and so we package up this array with the node signal
            // array to abstract away the level of indirection described by outputNodeIdxArr.
            if(boundedOutput) {
                _outputSignalArrayWrapper = new BoundedMappingSignalArray(_activationArr, outputNodeIdxArr);
            } else {
                _outputSignalArrayWrapper = new MappingSignalArray<double>(_activationArr, outputNodeIdxArr);
            }

            // Store counts for use during activation.
            _inputNodeCount = inputNodeCount;
            _outputNodeCount = outputNodeCount;
        }

        #endregion

        #region IBlackBox Members

        /// <summary>
        /// Gets the number of inputs.
        /// </summary>
        public int InputCount
        {
            get { return _inputNodeCount; }
        }

        /// <summary>
        /// Gets the number of outputs.
        /// </summary>
        public int OutputCount
        {
            get { return _outputNodeCount; }
        }

        /// <summary>
        /// Gets an array for feeding input signals to the network.
        /// </summary>
        public ISignalArray<double> InputSignalArray
        {
            get { return _inputSignalArrayWrapper; }
        }

        /// <summary>
        /// Gets an array of output signals from the network.
        /// </summary>
        public ISignalArray<double> OutputSignalArray
        {
            get { return _outputSignalArrayWrapper; }
        }

        /// <summary>
        /// Activate the network. Activation reads input signals from InputSignalArray and writes output signals
        /// to OutputSignalArray.
        /// </summary>
        public virtual void Activate()
        {   
            // Reset any state from a previous activation.
            for(int i=_inputNodeCount; i<_activationArr.Length; i++) {
                _activationArr[i] = 0.0;
            }

            // Process all layers in turn.
            int conIdx=0, nodeIdx=_inputNodeCount;
            for(int layerIdx=1; layerIdx < _layerInfoArr.Length; layerIdx++)
            {
                LayerInfo layerInfo = _layerInfoArr[layerIdx-1];

                // Push signals through the previous layer's connections to the current layer's nodes.
                for(; conIdx < layerInfo._endConnectionIdx; conIdx++) {
                    _activationArr[_connArr[conIdx].TargetId] += _activationArr[_connArr[conIdx].SourceId] * _weightArr[conIdx];
                }

                // TODO: Performance tune the activation function method call.
                // The call to Calculate() cannot be inlined because it is via an interface and therefore requires a virtual table lookup.
                // The obvious/simplest performance improvement would be to pass an array of values to Calculate().
                // 
                // Activate current layer's nodes.
                layerInfo = _layerInfoArr[layerIdx];
                for(; nodeIdx < layerInfo._endNodeIdx; nodeIdx++) {
                    _activationArr[nodeIdx] = _activationFnArr[nodeIdx](_activationArr[nodeIdx]);
                }
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
