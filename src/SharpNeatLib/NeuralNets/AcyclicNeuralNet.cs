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
using SharpNeat.Network;
using SharpNeat.Network2;
using SharpNeat.Network2.Acyclic;
using SharpNeat.Phenomes;

namespace SharpNeat.NeuralNets
{
    /// <summary>
    /// A neural network implementation for acyclic networks.
    /// 
    /// Activation of acyclic networks can be far more efficient than cyclic networks because we can activate the network by 
    /// propagating a signal 'wave' from the input nodes through each layer to the output nodes, thus each node
    /// requires activation only once at most, whereas in cyclic networks we must (a) activate each node multiple times and 
    /// (b) have a scheme that defines when to stop activating the network.
    /// 
    /// Algorithm Overview.
    /// 1) The nodes are assigned a depth number based on how many connection hops they are from an input node. Where multiple 
    /// paths to a node exist the longest path determines the node's depth.
    /// 
    /// 2) Connections are similarly assigned a depth value which is defined as the depth of a connection's source node.
    /// 
    /// Note. Steps 1 and 2 are actually performed by AcyclicNetworkFactory.
    /// 
    /// 3) Reset all node activation values to zero. This resets any state from a previous activation.
    /// 
    /// 4) Each layer of the network can now be activated in turn to propagate the signals on the input nodes through the network.
    /// Input nodes do no apply an activation function so we start by activating the connections on the first layer (depth == 0), 
    /// this accumulates node pre-activation signals on all of the target nodes which can be anywhere from depth 1 to the highest 
    /// depth level. Having done this we apply the node activation function for all nodes at the layer 1 because we can now 
    /// guarantee that there will be no more incoming signals to those nodes. Repeat for all remaining layers in turn.
    /// </summary>
    public class AcyclicNeuralNet : IPhenome<double>
    {
        #region Instance Fields

        // Connection arrays.
        readonly DirectedConnection[] _connArr;
        readonly double[] _weightArr;

        // Array of layer information.
        readonly LayerInfo[] _layerInfoArr;        

        // Node activation function.
        readonly VecFnSegment<double> _activationFn;

        // Node activation level array (used for both pre and post activation levels).
        readonly double[] _activationArr;

        // Wrappers over _activationArr that map between black box inputs/outputs to the
        // corresponding underlying node activation levels.
        readonly VectorSegment<double> _inputSignalVector;
        readonly MappingVector<double> _outputSignalVector;

        // Convenient counts.
        readonly int _inputCount;
        readonly int _outputCount;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a AcyclicNeuralNet with the provided neural net definition parameters.
        /// </summary>
        /// <param name="diGraph">Network structure definition</param>
        /// <param name="activationFn">Node activation function.</param>
        /// <param name="boundedOutput">Indicates that the output values at the output nodes should be bounded to the interval [0,1]</param>
        public AcyclicNeuralNet(
            WeightedAcyclicDirectedGraph<double> diGraph,
            VecFnSegment<double> activationFn,
            bool boundedOutput)
        {
            // Store refs to network structure data.
            _connArr = diGraph.ConnectionArray;
            _weightArr = diGraph.WeightArray;
            _layerInfoArr = diGraph.LayerArray;

            // Store network activation function.
            _activationFn = activationFn;

            // Store input/output node counts.
            _inputCount = diGraph.InputNodeCount;
            _outputCount = diGraph.OutputNodeCount;

            // Create working array for node activation signals.
            _activationArr = new double[diGraph.TotalNodeCount];

            // Wrap a sub-range of the _activationArr that holds the activation values for the input nodes.
            _inputSignalVector = new VectorSegment<double>(_activationArr, 0, _inputCount);

            // Wrap the output nodes. Nodes have been sorted by depth within the network therefore the output
            // nodes can no longer be guaranteed to be in a contiguous segment at a fixed location. As such their
            // positions are indicated by outputNodeIdxArr, and so we package up this array with the node signal
            // array to abstract away the indirection described by outputNodeIdxArr.
            if(boundedOutput) {
                _outputSignalVector = new BoundedMappingVector(_activationArr, diGraph.OutputNodeIdxArr);
            } else {
                _outputSignalVector = new MappingVector<double>(_activationArr, diGraph.OutputNodeIdxArr);
            }
        }

        #endregion

        #region IBlackBox Members

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
        public IVector<double> InputVector => _inputSignalVector;

        /// <summary>
        /// Gets an array of output signals from the network, i.e. the network output vector.
        /// </summary>
        public IVector<double> OutputVector => _outputSignalVector;

        /// <summary>
        /// Activate the network. Activation reads input signals from InputSignalArray and writes output signals
        /// to OutputSignalArray.
        /// </summary>
        public void Activate()
        {   
            // Reset any state from a previous activation.
            for(int i=_inputCount; i<_activationArr.Length; i++) {
                _activationArr[i] = 0.0;
            }

            // Process all layers in turn.
            int conIdx = 0;
            int nodeIdx = _inputCount;

            for(int layerIdx=1; layerIdx < _layerInfoArr.Length; layerIdx++)
            {
                LayerInfo layerInfo = _layerInfoArr[layerIdx-1];

                // Push signals through the previous layer's connections to the current layer's nodes.
                for(; conIdx < layerInfo.EndConnectionIdx; conIdx++) {
                    _activationArr[_connArr[conIdx].TargetId] += _activationArr[_connArr[conIdx].SourceId] * _weightArr[conIdx];
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
