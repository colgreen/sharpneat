// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpNeat.Graphs;

namespace SharpNeat.NeuralNets.Double;

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
    // Connection arrays.
    readonly ConnectionIds _connIds;
    readonly double[] _weightArr;

    // Activation function.
    readonly VecFn2<double> _activationFn;

    // Node activation signals array, and two sub-segments representing pre and post activation signals, respectively.
    readonly double[] _activationsArr;
    readonly Memory<double> _preActivationMem;
    readonly Memory<double> _postActivationMem;

    // Convenient counts.
    readonly int _inputCount;
    readonly int _outputCount;
    readonly int _totalNodeCount;
    readonly int _cyclesPerActivation;
    volatile bool _isDisposed;

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

        // Get a working array for both pre and post node activation signals, and map memory segments to pre
        // and post signal segments.
        // Rent an array that has length of at least _totalNodeCount * 2.
        _activationsArr = ArrayPool<double>.Shared.Rent(_totalNodeCount << 1);
        _preActivationMem = _activationsArr.AsMemory(0, _totalNodeCount);
        _postActivationMem = _activationsArr.AsMemory(_totalNodeCount, _totalNodeCount);

        // Map the input and output vectors to the corresponding segments of _postActivationMem.
        this.Inputs = _postActivationMem.Slice(0, _inputCount);

        // Note. Output neurons follow input neurons in the arrays.
        this.Outputs = _postActivationMem.Slice(_inputCount, _outputCount);
    }

    #endregion

    #region IBlackBox

    /// <summary>
    /// Gets a memory segment that represents a vector of input values.
    /// </summary>
    public Memory<double> Inputs { get; }

    /// <summary>
    /// Gets a memory segment that represents a vector of output values.
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
        Span<double> preActivations = _preActivationMem.Span;
        Span<double> postActivations = _postActivationMem.Span;

        // Note. Here we skip over the activations corresponding to the input neurons, as these have no
        // incoming connections, and therefore have fixed post-activation values and are never activated.
        int nonInputCount = _totalNodeCount - _inputCount;
        Span<double> preActivationsNonInputs = preActivations.Slice(_inputCount);
        Span<double> postActivationsNonInputs = postActivations.Slice(_inputCount);

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
            preActivationsNonInputs.Clear();
        }
    }

    /// <summary>
    /// Reset the network's internal state.
    /// </summary>
    public void Reset()
    {
        // Reset the elements of _preActivationArray and _postActivationArr that represent the
        // output and hidden nodes.
        _preActivationMem.Span.Slice(_inputCount).Clear();
        _postActivationMem.Span.Slice(_inputCount).Clear();
    }

    #endregion

    #region IDisposable

    /// <inheritdoc/>
    public void Dispose()
    {
        if(!_isDisposed)
        {
            _isDisposed = true;
            ArrayPool<double>.Shared.Return(_activationsArr);
        }
    }

    #endregion
}
