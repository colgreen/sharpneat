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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
        readonly int _totalNodeCount;
        readonly int _cyclesPerActivation;
        volatile bool _isDisposed;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a cyclic neural network.
        /// </summary>
        /// <param name="digraph">The weighted directed graph that defines the neural network structure and connection weights.</param>
        /// <param name="activationFn">The neuron activation function to use at all neurons in the network.</param>
        /// <param name="cyclesPerActivation">The number of activation cycles to perform per overall activation of the cyclic network.</param>
        public NeuralNetCyclic(
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
        public NeuralNetCyclic(
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
            _totalNodeCount = digraph.TotalNodeCount;

            // Get a working arrays for pre and post node activation signals.
            _preActivationArr = ArrayPool<double>.Shared.Rent(_totalNodeCount);
            _postActivationArr = ArrayPool<double>.Shared.Rent(_totalNodeCount);

            // Map the input and output vectors to the corresponding segments of _postActivationArr.
            this.Inputs = new Memory<double>(_postActivationArr, 0, _inputCount);

            // Note. Output neurons follow input neurons in the arrays.
            this.Outputs = new Memory<double>(_postActivationArr, _inputCount, _outputCount);
        }

        #endregion

        #region IBlackBox

        /// <summary>
        /// Gets a memory segment used for passing input signals to the network, i.e., the network input vector.
        /// </summary>
        public Memory<double> Inputs { get; }

        /// <summary>
        /// Gets a memory segment of output signals from the network, i.e., the network output vector.
        /// </summary>
        public Memory<double> Outputs { get; }

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
            int nonInputCount = _totalNodeCount - _inputCount;
            Span<double> preActivationsNonInputs = preActivations.Slice(_inputCount, nonInputCount);
            Span<double> postActivationsNonInputs = postActivations.Slice(_inputCount, nonInputCount);

            ref int srcIdsRef = ref MemoryMarshal.GetReference(srcIds);
            ref int tgtIdsRef = ref MemoryMarshal.GetReference(tgtIds);
            ref double weightsRef = ref MemoryMarshal.GetReference(weights);
            ref double preActivationsRef = ref MemoryMarshal.GetReference(preActivations);
            ref double postActivationsRef = ref MemoryMarshal.GetReference(postActivations);
            ref double preActivationsNonInputsRef = ref MemoryMarshal.GetReference(preActivationsNonInputs);
            ref double postActivationsNonInputsRef = ref MemoryMarshal.GetReference(postActivationsNonInputs);

            // Activate the network for a fixed number of timesteps.
            for(int i=0; i < _cyclesPerActivation; i++)
            {
                // Loop connections. Get each connection's input signal, apply the weight and add the result to
                // the pre-activation signal of the target neuron.
                for(int j=0; j < srcIds.Length; j++)
                {
                    Unsafe.Add(ref preActivationsRef, Unsafe.Add(ref tgtIdsRef, j)) =
                        Math.FusedMultiplyAdd(
                            Unsafe.Add(ref postActivationsRef, Unsafe.Add(ref srcIdsRef, j)),
                            Unsafe.Add(ref weightsRef, j),
                            Unsafe.Add(ref preActivationsRef, Unsafe.Add(ref tgtIdsRef, j)));
                }

                // Pass the pre-activation levels through the activation function, storing the results in the
                // post-activation span/array.
                _activationFn(
                    ref preActivationsNonInputsRef,
                    ref postActivationsNonInputsRef,
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
        public void Reset()
        {
            // Reset the elements of _preActivationArray and _postActivationArr that represent the
            // output and hidden nodes.
            int nonInputCount = _totalNodeCount - _inputCount;
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
            if(!_isDisposed)
            {
                _isDisposed = true;
                ArrayPool<double>.Shared.Return(_preActivationArr);
                ArrayPool<double>.Shared.Return(_postActivationArr);
            }
        }

        #endregion
    }
}
