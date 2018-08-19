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
using System.Diagnostics;
using SharpNeat.Network;
using SharpNeat.BlackBox;
using SharpNeat.BlackBox.Double;

namespace SharpNeat.NeuralNet.Double
{
    /// <summary>
    /// A neural network class that represents a network with recurrent (cyclic) connections. 
    /// 
    /// This class contains performance improvements described in the following report/post:
    /// 
    ///     http://sharpneat.sourceforge.net/research/network-optimisations.html
    /// 
    /// A speed-up over a previous 'naive' implementation was achieved by compactly storing all required data in arrays
    /// in a way that maximizes in-order memory accesses; this allows for good utilisation of CPU caches. 
    /// 
    /// Algorithm Overview.
    /// 1) Loop connections. Each connection gets its input signal from its source node, multiplies this signal by its
    /// weight, and adds the result to its target node's pre-activation variable. Connections are ordered by source node 
    /// index, thus all memory reads here are sequential, but the memory writes to node pre-activation variables are 
    /// non-sequential.
    /// 
    /// 2) Loop nodes. Pass each node's pre-activation signal through the activation function and set its 
    /// post-activation signal value. 
    /// 
    /// The activation loop is now complete and we can go back to (1) or stop.
    /// </summary>
    public class CyclicNeuralNet : IBlackBox<double>
    {
        #region Instance Fields

        // Connection arrays.
        readonly int[] _srcIdArr;
        readonly int[] _tgtIdArr;
        readonly double[] _weightArr;
        
        // Activation function.
        readonly VecFnSegment2<double> _activationFn;

        // Node pre- and post-activation signal arrays.
        readonly double[] _preActivationArr;
        readonly double[] _postActivationArr;

        // Wrappers over _postActivationArr that map between input/output vectors and the
        // corresponding underlying network nodes.
        readonly VectorSegment<double> _inputVector;
        readonly IVector<double> _outputVector;

        // Convenient counts.
        readonly int _inputCount;
        readonly int _outputCount;
        readonly int _activationCount;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a CyclicNetwork with the provided neural net definition.
        /// </summary>
        public CyclicNeuralNet (
            WeightedDirectedGraph<double> digraph,
            VecFnSegment2<double> activationFn,
            int activationCount,
            bool boundedOutput)
        :this(
             digraph,
             digraph.WeightArray,
             activationFn,
             activationCount,
             boundedOutput)
        {}

        /// <summary>
        /// Constructs a CyclicNetwork with the provided neural net definition.
        /// </summary>
        public CyclicNeuralNet(
            DirectedGraph digraph,
            double[] weightArr,
            VecFnSegment2<double> activationFn,
            int activationCount,
            bool boundedOutput)
        {
            Debug.Assert(digraph.ConnectionIdArrays._sourceIdArr.Length == weightArr.Length);

            // Store refs to network structure data.
            _srcIdArr = digraph.ConnectionIdArrays._sourceIdArr;
            _tgtIdArr = digraph.ConnectionIdArrays._targetIdArr;
            _weightArr = weightArr;

            // Store network activation function and parameters.
            _activationFn = activationFn;
            _activationCount = activationCount;

            // Store input/output node counts.
            _inputCount = digraph.InputCount;
            _outputCount = digraph.OutputCount;

            // Create node pre- and post-activation signal arrays.
            int nodeCount = digraph.TotalNodeCount;
            _preActivationArr = new double[nodeCount];
            _postActivationArr = new double[nodeCount];

            // Wrap sub-ranges of the neuron signal arrays as input and output vectors.
            _inputVector = new VectorSegment<double>(_postActivationArr, 0, _inputCount);

            // Note. Output neurons follow input neurons in the arrays.
            var outputVec = new VectorSegment<double>(_postActivationArr, _inputCount, _outputCount);

            if(boundedOutput) {
                _outputVector = new BoundedVector(outputVec);
            } else {
                _outputVector = outputVec;
            }
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
        /// Activate the network for a fixed number of iterations defined by the 'maxIterations' parameter
        /// at construction time. Activation reads input signals from InputSignalArray and writes output signals
        /// to OutputSignalArray.
        /// </summary>
        public void Activate()
        {
            // Activate the network for a fixed number of timesteps.
            for(int i=0; i < _activationCount; i++)
            {
                // Loop connections. Get each connection's input signal, apply the weight and add the result to 
                // the pre-activation signal of the target neuron.
                for(int j=0; j < _srcIdArr.Length; j++) {
                    _preActivationArr[_tgtIdArr[j]] += _postActivationArr[_srcIdArr[j]] * _weightArr[j];
                }

                // Pass the pre-activation levels through the activation function.
                // Note. the post-activation levels are stored in _postActivationArray.
                // Note. Skip over input neurons as these have no incoming connections and therefore have fixed
                // post-activation values and are never activated. 
                _activationFn(_preActivationArr, _postActivationArr, _inputCount, _preActivationArr.Length);

                // Reset the elements of _preActivationArray
                Array.Clear(_preActivationArr, _inputCount, _preActivationArr.Length-_inputCount);
            }
        }

        /// <summary>
        /// Reset the network's internal state.
        /// </summary>
        public void ResetState()
        {
            // Reset the output signal for all output and hidden neurons.
            // Ignore connection signal state as this gets overwritten on each iteration.
            for(int i=_inputCount; i < _postActivationArr.Length; i++) {
                _preActivationArr[i] = 0.0;
            }
        }

        #endregion
    }
}
