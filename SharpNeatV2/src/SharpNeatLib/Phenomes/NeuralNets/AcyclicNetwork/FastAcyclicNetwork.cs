/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */
using SharpNeat.Network;

namespace SharpNeat.Phenomes.NeuralNets
{
    /// <summary>
    /// A neural network implementation for acyclic networks.
    /// 
    /// Activation of acyclic networks can be far more efficient than cyclic networks because we can activate the network by 
    /// propogating a signal 'wave' from the input nodes through each depth layer through to teh output nodes, thus each node
    /// requries activating only once at most, whereas in cyclic networks we have to activate each node multiple times and we 
    /// muts have a scheme for determinign when to stop activating.
    /// 
    /// Algorithm Overview.
    /// 1) The nodes are assigned a depth number based on how many connection hops they are from an input nodes. Where multiple 
    /// paths to a node exist the longest path determines the node's depth.
    /// 
    /// 2) Connections are similarly assigned a depth value which is defined as the depth of a connection's source node.
    /// 
    /// Note. Steps 1 and 2 are actually performed by FastAcyclicNetworkFactory.
    /// 
    /// 3) Reset all node activation values to zero. This resets any state from a previous activation.
    /// 
    /// 4) Each layer of the network can now be activated in turn to propogate the signals on the input nodes through the network.
    /// Input nodes do no apply an activation function so we start by activating the connections on the first layer (depth == 0), 
    /// this accumulates node pre-activation signals on all of the target nodes which can be anywhere from depth 1 to the highest 
    /// depth level. Haveing done this we apply the node activation function for all nodes at the layer 1 because we can now 
    /// guarantee that there will be no more incomign signals to those nodes. Repeat for all remaining layers in turn.
    /// </summary>
    public class FastAcyclicNetwork : IBlackBox
    {
    //=== Fixed data. Network structure and activation functions/data.
        /// <summary>
        /// Array of node activation functions.
        /// </summary>
        readonly IActivationFunction[] _nodeActivationFnArr;
        /// <summary>
        /// Array of node activation function auxiliary arguments.
        /// </summary>
        readonly double[][] _nodeAuxArgsArr;
        /// <summary>
        /// Array of connections.
        /// </summary>
        readonly FastConnection[] _connectionArr;
        /// <summary>
        /// Array of layer information. Feedforward-only network activation can be performed most 
        /// efficiently by propogating signals through the network one layer at a time.
        /// </summary>
        readonly LayerInfo[] _layerInfoArr;

    //=== Working data.
        /// <summary>
        /// Array of node activation signals.
        /// </summary>
        readonly double[] _activationArr;

    //=== Misc.
        // Wrappers over _activationArr that map between black box inputs/outputs to the
        // corresponding underlying node activation levels.
        readonly SignalArray _inputSignalArrayWrapper;
        readonly MappingSignalArray _outputSignalArrayWrapper;

        // Convenient counts.
        readonly int _inputNodeCount;
        readonly int _outputNodeCount;
        readonly int _inputAndBiasNodeCount;

        #region Constructor

        /// <summary>
        /// Construct a FastAcyclicNetwork with provided network definition data structures.
        /// </summary>
        /// <param name="nodeActivationFnArr">Array of neuron activation functions.</param>
        /// <param name="nodeAuxArgsArr">Array of neuron activation function arguments.</param>
        /// <param name="connectionArr">Array of connections.</param>
        /// <param name="layerInfoArr">Array of layer information.</param>
        /// <param name="outputNodeIdxArr">An array that specifies the index of each output neuron within _activationArr.
        /// This is necessary because the neurons have been sorted by their depth in the network structure and are therefore
        /// no longer in their original positions. Note however that the bias and input neurons *are* in their original 
        /// positions as they are defined as being at depth zero.</param>
        /// <param name="nodeCount">Number of nodes in the network.</param>
        /// <param name="inputNodeCount">Number of input nodes in the network.</param>
        /// <param name="outputNodeCount">Number of output nodes in the network.</param>
        public FastAcyclicNetwork(IActivationFunction[] nodeActivationFnArr,
                                  double[][] nodeAuxArgsArr,
                                  FastConnection[] connectionArr,
                                  LayerInfo[] layerInfoArr,
                                  int[] outputNodeIdxArr,
                                  int nodeCount,
                                  int inputNodeCount,
                                  int outputNodeCount)
        {
            // Store refs to network structrue data.
            _nodeActivationFnArr = nodeActivationFnArr;
            _nodeAuxArgsArr = nodeAuxArgsArr;
            _connectionArr = connectionArr;
            _layerInfoArr = layerInfoArr;

            // Create working array for node activation signals.
            _activationArr = new double[nodeCount];

            // Wrap a sub-range of the _activationArr that holds the activation values for the input nodes.
            // Offset is 1 to skip bias neuron (The value at index 1 is the first black box input).
            _inputSignalArrayWrapper = new SignalArray(_activationArr, 1, inputNodeCount);

            // Wrap the output nodes. Nodes have been sorted by depth within the network therefore the output
            // nodes can no longer be guaranteed to be in a contiguous segment at a fixed location. As such their
            // positions are indicated by outputNodeIdxArr, and so we package up this array with the node signal
            // array to abstract away the level of indirection described by outputNodeIdxArr.
            _outputSignalArrayWrapper = new MappingSignalArray(_activationArr, outputNodeIdxArr);

            // Store counts for use during activation.
            _inputNodeCount = inputNodeCount;
            _inputAndBiasNodeCount = inputNodeCount+1;
            _outputNodeCount = outputNodeCount;

            // Initialise the bias neuron's fixed output value.
            _activationArr[0] = 1.0;
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
        public ISignalArray InputSignalArray
        {
            get { return _inputSignalArrayWrapper; }
        }

        /// <summary>
        /// Gets an array of output signals from the network.
        /// </summary>
        public ISignalArray OutputSignalArray
        {
            get { return _outputSignalArrayWrapper; }
        }

        /// <summary>
        /// Gets a value indicating whether the internal state is valid. Always returns true for this class.
        /// </summary>
        public virtual bool IsStateValid
        {
            get { return true; }
        }

        /// <summary>
        /// Activate the network. Activation reads input signals from InputSignalArray and writes output signals
        /// to OutputSignalArray.
        /// </summary>
        public virtual void Activate()
        {   
            // Reset any state from a previous activation.
            for(int i=_inputAndBiasNodeCount; i<_activationArr.Length; i++) {
                _activationArr[i] = 0.0;
            }

            // Process all layers in turn.
            int conIdx=0, nodeIdx=_inputAndBiasNodeCount;
            for(int layerIdx=1; layerIdx < _layerInfoArr.Length; layerIdx++)
            {
                LayerInfo layerInfo = _layerInfoArr[layerIdx-1];

                // Push signals through the previous layer's connections to the current layer's nodes.
                for(; conIdx < layerInfo._endConnectionIdx; conIdx++) {
                    _activationArr[_connectionArr[conIdx]._tgtNeuronIdx] += _activationArr[_connectionArr[conIdx]._srcNeuronIdx] * _connectionArr[conIdx]._weight;
                }

                // Activate current layer's nodes.
                layerInfo = _layerInfoArr[layerIdx];
                for(; nodeIdx < layerInfo._endNodeIdx; nodeIdx++) {
                    _activationArr[nodeIdx] = _nodeActivationFnArr[nodeIdx].Calculate(_activationArr[nodeIdx], _nodeAuxArgsArr[nodeIdx]);
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
