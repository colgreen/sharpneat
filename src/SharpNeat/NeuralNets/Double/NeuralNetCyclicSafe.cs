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
    /// This class is functionally equivalent to <see cref="NeuralNetCyclic"/>, but doesn't use any of the unsafe
    /// memory pointer techniques used in that class, and therefore this class is much slower.
    ///
    /// This class is intended to be used as a safe reference implementation that can be used when testing or
    /// debugging <see cref="NeuralNetCyclic"/>.
    /// </summary>
    public sealed class NeuralNetCyclicSafe : IBlackBox<double>
    {
        #region Instance Fields

        // Connection arrays.
        readonly ConnectionIds _connIds;
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
        public NeuralNetCyclicSafe(
            WeightedDirectedGraph<double> digraph,
            VecFn2<double> activationFn,
            int cyclesPerActivation)
        : this(
             digraph,
             digraph.WeightArray,
             activationFn,
             cyclesPerActivation)
        {
        }

        /// <summary>
        /// Constructs a cyclic neural network.
        /// </summary>
        /// <param name="digraph">The directed graph that defines the neural network structure.</param>
        /// <param name="weightArr">An array of neural network connection weights.</param>
        /// <param name="activationFn">The neuron activation function to use at all neurons in the network.</param>
        /// <param name="cyclesPerActivation">The number of activation cycles to perform per overall activation of the cyclic network.</param>
        public NeuralNetCyclicSafe(
            DirectedGraph digraph,
            double[] weightArr,
            VecFn2<double> activationFn,
            int cyclesPerActivation)
        {
            Debug.Assert(digraph.ConnectionIds.GetSourceIdSpan().Length == weightArr.Length);

            // Store refs to network structure data.
            _connIds = digraph.ConnectionIds;
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
            ReadOnlySpan<int> srcIds = _connIds.GetSourceIdSpan();
            ReadOnlySpan<int> tgtIds = _connIds.GetTargetIdSpan();
            ReadOnlySpan<double> weights = _weightArr.AsSpan();
            Span<double> preActivations = _preActivationArr.AsSpan();
            Span<double> postActivations = _postActivationArr.AsSpan();

            // Note. Here we skip over the activations corresponding to the input neurons, as these have no
            // incoming connections, and therefore have fixed post-activation values and are never activated.
            int nonInputCount = _preActivationArr.Length - _inputCount;
            Span<double> preActivationsNonInputs = preActivations.Slice(_inputCount, nonInputCount);
            Span<double> postActivationsNonInputs = postActivations.Slice(_inputCount, nonInputCount);

            // Activate the network for a fixed number of timesteps.
            for(int i=0; i < _cyclesPerActivation; i++)
            {
                // Loop connections. Get each connection's input signal, apply the weight and add the result to
                // the pre-activation signal of the target neuron.
                for(int j=0; j < srcIds.Length; j++)
                {
                    preActivations[tgtIds[j]] =
                        Math.FusedMultiplyAdd(
                            postActivations[srcIds[j]],
                            weights[j],
                            preActivations[tgtIds[j]]);
                }

                // Pass the pre-activation levels through the activation function, storing the results in the
                // post-activation span/array.
                _activationFn(
                    ref preActivationsNonInputs[0],
                    ref postActivationsNonInputs[0],
                    nonInputCount);

                // Reset the elements of _preActivationArray that represent the output and hidden nodes.
                Array.Clear(
                    _preActivationArr,
                    _inputCount,
                    nonInputCount);
            }
        }

        /// <summary>
        /// Reset the network's internal state.
        /// </summary>
        public void ResetState()
        {
            // Reset the elements of _preActivationArray and _postActivationArr that represent the
            // output and hidden nodes.
            int nonInputCount = _preActivationArr.Length - _inputCount;
            Array.Clear(_preActivationArr, _inputCount, nonInputCount);
            Array.Clear(_postActivationArr, _inputCount, nonInputCount);
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Releases both managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        #endregion
    }
}
