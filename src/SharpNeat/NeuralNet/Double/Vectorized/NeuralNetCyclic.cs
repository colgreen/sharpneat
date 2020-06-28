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
using System.Diagnostics;
using System.Numerics;
using SharpNeat.BlackBox;
using SharpNeat.Network;

namespace SharpNeat.NeuralNet.Double.Vectorized
{
    /// <summary>
    /// A version of <see cref="Double.NeuralNetCyclic"/> that utilises some vectorized operations
    /// for improved performance on hardware platforms that support them.
    /// </summary>
    public sealed class NeuralNetCyclic : IBlackBox<double>
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

        // Convenient counts.
        readonly int _inputCount;
        readonly int _outputCount;
        readonly int _cyclesPerActivation;

        // Connection inputs array.
        readonly double[] _conInputArr = new double[Vector<double>.Count];

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a CyclicNetwork with the provided neural net definition.
        /// </summary>
        public NeuralNetCyclic (
            WeightedDirectedGraph<double> digraph,
            VecFnSegment2<double> activationFn,
            int cyclesPerActivation)
        :this(
             digraph,
             digraph.WeightArray,
             activationFn,
             cyclesPerActivation)
        {}

        /// <summary>
        /// Constructs a CyclicNetwork with the provided neural net definition.
        /// </summary>
        public NeuralNetCyclic(
            DirectedGraph digraph,
            double[] weightArr,
            VecFnSegment2<double> activationFn,
            int cyclesPerActivation)
        {
            Debug.Assert(digraph.ConnectionIdArrays._sourceIdArr.Length == weightArr.Length);

            // Store refs to network structure data.
            _srcIdArr = digraph.ConnectionIdArrays._sourceIdArr;
            _tgtIdArr = digraph.ConnectionIdArrays._targetIdArr;
            _weightArr = weightArr;

            // Store network activation function and parameters.
            _activationFn = activationFn;
            _cyclesPerActivation = cyclesPerActivation;

            // Store input/output node counts.
            _inputCount = digraph.InputCount;
            _outputCount = digraph.OutputCount;

            // Create node pre- and post-activation signal arrays.
            int nodeCount = digraph.TotalNodeCount;
            _preActivationArr = new double[nodeCount];
            _postActivationArr = new double[nodeCount];

            // Wrap sub-ranges of the neuron signal arrays as input and output vectors.
            this.InputVector = new VectorSegment<double>(_postActivationArr, 0, _inputCount);

            // Note. Output neurons follow input neurons in the arrays.
            this.OutputVector = new VectorSegment<double>(_postActivationArr, _inputCount, _outputCount);
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
        public IVector<double> InputVector { get; }

        /// <summary>
        /// Gets an array of output signals from the network, i.e. the network output vector.
        /// </summary>
        public IVector<double> OutputVector { get ; }

        /// <summary>
        /// Activate the network for a fixed number of iterations defined by the 'maxIterations' parameter
        /// at construction time. Activation reads input signals from InputSignalArray and writes output signals
        /// to OutputSignalArray.
        /// </summary>
        public void Activate()
        {
            // Init vector related variables.
            int width = Vector<double>.Count;
            double[] conInputArr = _conInputArr;

            // Activate the network for a fixed number of timesteps.
            for(int i=0; i < _cyclesPerActivation; i++)
            {
                // Loop connections. Get each connection's input signal, apply the weight and add the result to 
                // the pre-activation signal of the target neuron.
                int conIdx=0;
                for(; conIdx <= _srcIdArr.Length - width; conIdx += width) 
                {
                    // Load source node output values into a vector.
                    for(int k=0; k<width; k++) {
                        conInputArr[k] = _postActivationArr[_srcIdArr[conIdx + k]];
                    }
                    var conInputVec = new Vector<double>(conInputArr);

                    // Load connection weights into a vector.
                    var weightVec = new Vector<double>(_weightArr, conIdx);

                    // Multiply connection source inputs and connection weights.
                    var conOutputVec = conInputVec * weightVec;

                    // Save/accumulate connection output values onto the connection target nodes.
                    for(int k=0; k < width; k++) {
                        _preActivationArr[_tgtIdArr[conIdx+k]] += conOutputVec[k];
                    }
                }

                // Loop remaining connections
                for(; conIdx < _srcIdArr.Length; conIdx++) {
                    _preActivationArr[_tgtIdArr[conIdx]] = Math.FusedMultiplyAdd(_postActivationArr[_srcIdArr[conIdx]], _weightArr[conIdx], _preActivationArr[_tgtIdArr[conIdx]]);
                }

                // Pass the pre-activation levels through the activation function.
                // Note. the post-activation levels are stored in _postActivationArray.
                // Note. Skip over input neurons as these have no incoming connections and therefore have fixed
                // post-activation values and are never activated. 
                _activationFn(_preActivationArr, _postActivationArr, _inputCount, _preActivationArr.Length);

                // Reset the elements of _preActivationArray that represent the output and hidden nodes.
                Array.Clear(_preActivationArr, _inputCount, _preActivationArr.Length - _inputCount);
            }
        }

        /// <summary>
        /// Reset the network's internal state.
        /// </summary>
        public void ResetState()
        {
            // Reset the elements of _preActivationArray and _postActivationArr that represent the 
            // output and hidden nodes.
            // Note. Connection signal state is not reset as this gets overwritten on each iteration.   
            Array.Clear(_preActivationArr, _inputCount, _preActivationArr.Length - _inputCount);
            Array.Clear(_postActivationArr, _inputCount, _postActivationArr.Length - _inputCount);
        }

        #endregion
    }
}
