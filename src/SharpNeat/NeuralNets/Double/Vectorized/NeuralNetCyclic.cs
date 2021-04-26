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
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpNeat.BlackBox;
using SharpNeat.Graphs;

namespace SharpNeat.NeuralNets.Double.Vectorized
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
        readonly VecFn2<double> _activationFn;

        // Node pre- and post-activation signal arrays.
        readonly double[] _preActivationArr;
        readonly double[] _postActivationArr;

        // Convenient counts.
        readonly int _inputCount;
        readonly int _outputCount;
        readonly int _totalNodeCount;
        readonly int _cyclesPerActivation;

        // Connection inputs array.
        readonly double[] _conInputArr = new double[Vector<double>.Count];
        volatile bool _isDisposed;

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
            _totalNodeCount = digraph.TotalNodeCount;

            // Get a working arrays for pre and post node activation signals.
            _preActivationArr = ArrayPool<double>.Shared.Rent(_totalNodeCount);
            _postActivationArr = ArrayPool<double>.Shared.Rent(_totalNodeCount);

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

            ReadOnlySpan<int> srcIds = _srcIdArr.AsSpan();
            ReadOnlySpan<int> tgtIds = _tgtIdArr.AsSpan();
            ReadOnlySpan<double> weights = _weightArr.AsSpan();
            Span<double> preActivations = _preActivationArr.AsSpan(0, _totalNodeCount);
            Span<double> postActivations = _postActivationArr.AsSpan(0, _totalNodeCount);

            // Note. Here we skip over the activations corresponding to the input neurons, as these have no
            // incoming connections, and therefore have fixed post-activation values and are never activated.
            int nonInputCount = _totalNodeCount - _inputCount;
            Span<double> preActivationsNonInputs = preActivations.Slice(_inputCount, nonInputCount);
            Span<double> postActivationsNonInputs = postActivations.Slice(_inputCount, nonInputCount);
            Span<double> connInputs = _conInputArr.AsSpan();

            ref int srcIdsRef = ref MemoryMarshal.GetReference(srcIds);
            ref int tgtIdsRef = ref MemoryMarshal.GetReference(tgtIds);
            ref double weightsRef = ref MemoryMarshal.GetReference(weights);
            ref double preActivationsRef = ref MemoryMarshal.GetReference(preActivations);
            ref double postActivationsRef = ref MemoryMarshal.GetReference(postActivations);
            ref double preActivationsNonInputsRef = ref MemoryMarshal.GetReference(preActivationsNonInputs);
            ref double postActivationsNonInputsRef = ref MemoryMarshal.GetReference(postActivationsNonInputs);
            ref double connInputsRef = ref MemoryMarshal.GetReference(connInputs);

            // Activate the network for a fixed number of timesteps.
            for(int i=0; i < _cyclesPerActivation; i++)
            {
                // Loop connections. Get each connection's input signal, apply the weight and add the result to
                // the pre-activation signal of the target neuron.
                int conIdx=0;
                for(; conIdx <= _srcIdArr.Length - Vector<double>.Count; conIdx += Vector<double>.Count)
                {
                    // Load source node output values into a vector.
                    ref int srcIdsRefSeg = ref Unsafe.Add(ref srcIdsRef, conIdx);

                    for(int k = 0; k < Vector<double>.Count; k++)
                    {
                        Unsafe.Add(ref connInputsRef, k) =
                            Unsafe.Add(
                                ref postActivationsRef,
                                Unsafe.Add(ref srcIdsRefSeg, k));
                    }

                    // Note. This obscure pattern is taken from the Vector<T> constructor source code.
                    var conVec = Unsafe.ReadUnaligned<Vector<double>>(
                        ref Unsafe.As<double, byte>(ref connInputsRef));

                    // Load connection weights into a vector.
                    var weightVec = new Vector<double>(_weightArr, conIdx);
                    // TODO: This ought to be faster, but is slower at time of writing (benchmarking on dotnet 6 preview 3).
                    // var weightVec = Unsafe.ReadUnaligned<Vector<double>>(
                    //    ref Unsafe.As<double, byte>(
                    //        ref Unsafe.Add(
                    //            ref weightsRef, conIdx)));

                    // Multiply connection source inputs and connection weights.
                    conVec *= weightVec;

                    // Save/accumulate connection output values onto the connection target nodes.
                    ref int tgtIdsRefSeg = ref Unsafe.Add(ref tgtIdsRef, conIdx);

                    for(int k = 0; k < Vector<double>.Count; k++)
                    {
                        Unsafe.Add(ref preActivationsRef, Unsafe.Add(ref tgtIdsRefSeg, k)) += conVec[k];
                    }
                }

                // Loop remaining connections
                for(; conIdx < _srcIdArr.Length; conIdx++)
                {
                    // Get a reference to the target activation level 'slot' in the activations span.
                    ref double tgtSlot = ref Unsafe.Add(ref preActivationsRef, Unsafe.Add(ref tgtIdsRef, conIdx));

                    tgtSlot = Math.FusedMultiplyAdd(
                                Unsafe.Add(ref postActivationsRef, Unsafe.Add(ref srcIdsRef, conIdx)),
                                Unsafe.Add(ref weightsRef, conIdx),
                                tgtSlot);
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
        public void ResetState()
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
