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
using SharpNeat.BlackBox;
using SharpNeat.Graphs;

namespace SharpNeat.NeuralNets.Double
{
    /// <summary>
    /// A neural network class that represents a network with recurrent (cyclic) connections.
    ///
    /// Algorithm Overview.
    /// 1) Loop connections.
    /// Each connection gets its input signal from its source node, multiplies the signal by its weight, and adds
    /// the result to its target node's pre-activation variable. Connections are ordered by source node index,
    /// thus all memory reads are sequential, but the memory writes to node pre-activation variables are
    /// non-sequential.
    ///
    /// 2) Loop nodes.
    /// Pass each node's pre-activation signal through the activation function, storing the result in a separate
    /// post-activation signals array.
    ///
    /// 3) Completion.
    /// Copy the post-activation signals into the pre-activations signals array.
    ///
    /// The activation loop is run a fixed number of times/cycles to allow signals to gradually propagate through
    /// the network, one timestep/cycle/loop at a time.
    /// </summary>
    public sealed class NeuralNetCyclic : IBlackBox<double>
    {
        #region Instance Fields

        // Connection arrays.
        readonly int[] _srcIdArr;
        readonly int[] _tgtIdArr;
        readonly double[] _weightArr;

        // Activation function.
        readonly VecFn2<double> _activationFn;

        // Node pre- and post-activation signal arrays.
        readonly double[] _preActivationArr;
        readonly double[] _postActivationArr;

        // Convenient counts.
        readonly int _inputCount;
        readonly int _outputCount;
        readonly int _cyclesPerActivation;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a cyclic neural network.
        /// </summary>
        /// <param name="digraph">The weighted directed graph that defines the neural network structure and connection weights.</param>
        /// <param name="activationFn">The neuron activation function to use at all neurons in the network.</param>
        /// <param name="cyclesPerActivation">The number of activation cycles to perform per overall activation of the cyclic network.</param>
        public NeuralNetCyclic (
            WeightedDirectedGraph<double> digraph,
            VecFn2<double> activationFn,
            int cyclesPerActivation)
        : this(
             digraph,
             digraph.WeightArray,
             activationFn,
             cyclesPerActivation)
        {}

        /// <summary>
        /// Constructs a cyclic neural network.
        /// </summary>
        /// <param name="digraph">The directed graph that defines the neural network structure.</param>
        /// <param name="weightArr">An array of neural network connection weights.</param>
        /// <param name="activationFn">The neuron activation function to use at all neurons in the network.</param>
        /// <param name="cyclesPerActivation">The number of activation cycles to perform per overall activation of the cyclic network.</param>
        public NeuralNetCyclic(
            DirectedGraph digraph,
            double[] weightArr,
            VecFn2<double> activationFn,
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
        public IVector<double> OutputVector { get; }

        /// <summary>
        /// Activate the network for a fixed number of iterations defined by the 'maxIterations' parameter
        /// at construction time. Activation reads input signals from InputSignalArray and writes output signals
        /// to OutputSignalArray.
        /// </summary>
        public void Activate()
        {
            // TODO: This code needs to be properly spanified; it's still making heavy use of array indexers.


            // TODO: ENHANCEMENT: Consider defining Memory<T> over the required ranges, as class fields.
            // Note. Skip over input neurons as these have no incoming connections and therefore have fixed
            // post-activation values and are never activated.
            var activationRange = new Range(_inputCount, _preActivationArr.Length);
            var preActivationSpan = _preActivationArr.AsSpan(activationRange);
            var postActivationSpan = _postActivationArr.AsSpan(activationRange);

            // Activate the network for a fixed number of timesteps.
            for(int i=0; i < _cyclesPerActivation; i++)
            {
                // Loop connections. Get each connection's input signal, apply the weight and add the result to
                // the pre-activation signal of the target neuron.
                for(int j=0; j < _srcIdArr.Length; j++)
                {
                    _preActivationArr[_tgtIdArr[j]] =
                        Math.FusedMultiplyAdd(
                            _postActivationArr[_srcIdArr[j]],
                            _weightArr[j],
                            _preActivationArr[_tgtIdArr[j]]);
                }

                // Pass the pre-activation levels through the activation function, storing the results in the
                // post-activation span/array.
                _activationFn(preActivationSpan, postActivationSpan);

                // TODO: ENHANCEMENT: Consider using Span.Clear().
                // Reset the elements of _preActivationArray that represent the output and hidden nodes.
                Array.Clear(
                    _preActivationArr,
                    _inputCount,
                    _preActivationArr.Length - _inputCount);
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
            Array.Clear(
                _preActivationArr,
                _inputCount,
                _preActivationArr.Length - _inputCount);

            Array.Clear(
                _postActivationArr,
                _inputCount,
                _postActivationArr.Length - _inputCount);
        }

        #endregion
    }
}
