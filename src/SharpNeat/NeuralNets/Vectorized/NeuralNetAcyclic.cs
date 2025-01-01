// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Buffers;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpNeat.Graphs.Acyclic;
using SharpNeat.NeuralNets.ActivationFunctions;

namespace SharpNeat.NeuralNets.Vectorized;

/// <summary>
/// A version of <see cref="NeuralNets.NeuralNetAcyclic{T}"/> that utilises some vectorized operations
/// for improved performance on hardware platforms that support them.
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
public sealed class NeuralNetAcyclic<TScalar> : IBlackBox<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    // Connection arrays.
    readonly ConnectionIds _connIds;
    readonly TScalar[] _weightArr;

    // Array of layer information.
    readonly LayerInfo[] _layerInfoArr;

    // Node activation function.
    readonly VecFn<TScalar> _activationFn;

    // Working array. Used for node activation signals, and a separate output signal segment on the end.
    readonly TScalar[] _workingArr;

    // Node activation signal segment (used for both pre and post activation levels).
    readonly Memory<TScalar> _activationMem;

    // Output signal segment.
    // The output signal values are copied into this array from _activationMem (see notes below).
    readonly Memory<TScalar> _outputMem;

    // An array containing output node indexes into _activationArr.
    // Notes. Nodes have been sorted by depth within the network, therefore the output nodes are not guaranteed
    // to be in a contiguous segment at a fixed location within _activationArr. As such, this array maps from
    // output index to node index in _activationArr.
    readonly int[] _outputNodeIdxArr;

    // Convenient counts.
    readonly int _inputCount;
    readonly int _outputCount;
    readonly int _totalNodeCount;

    // Connection inputs array.
    readonly TScalar[] _conInputArr = new TScalar[Vector<TScalar>.Count];
    volatile bool _isDisposed;

    #region Constructors

    /// <summary>
    /// Constructs a AcyclicNeuralNet with the provided neural net definition parameters.
    /// </summary>
    /// <param name="digraph">Network structure definition.</param>
    /// <param name="activationFn">Node activation function.</param>
    public NeuralNetAcyclic(
        WeightedDirectedGraphAcyclic<TScalar> digraph,
        VecFn<TScalar> activationFn)
        : this(digraph, digraph.WeightArray, activationFn)
    {
    }

    /// <summary>
    /// Constructs a AcyclicNeuralNet with the provided neural net definition parameters.
    /// </summary>
    /// <param name="digraph">Network structure definition.</param>
    /// <param name="weightArr">Connection weights array.</param>
    /// <param name="activationFn">Node activation function.</param>
    public NeuralNetAcyclic(
        DirectedGraphAcyclic digraph,
        TScalar[] weightArr,
        VecFn<TScalar> activationFn)
    {
        Debug.Assert(digraph.ConnectionIds.GetSourceIdSpan().Length == weightArr.Length);

        // Store refs to network structure data.
        _connIds = digraph.ConnectionIds;
        _weightArr = weightArr;
        _layerInfoArr = digraph.LayerArray;

        // Store network activation function.
        _activationFn = activationFn;

        // Store input/output node counts.
        _inputCount = digraph.InputCount;
        _outputCount = digraph.OutputCount;
        _totalNodeCount = digraph.TotalNodeCount;

        // Get a working array for node activations signals and a separate output signal segment on the end.
        // And map the memory segments onto the array.
        _workingArr = ArrayPool<TScalar>.Shared.Rent(_totalNodeCount + _outputCount);
        _activationMem = _workingArr.AsMemory(0, _totalNodeCount);
        _outputMem = _workingArr.AsMemory(_totalNodeCount, _outputCount);

        // Map the inputs vector to the corresponding segment of node activation values.
        Inputs = _activationMem.Slice(0, _inputCount);

        // Use the already defined outputs memory segment.
        Outputs = _outputMem;

        // Store the indexes into _activationArr that give the output signals.
        _outputNodeIdxArr = digraph.OutputNodeIdxArr;
    }

    #endregion

    #region IBlackBox

    /// <summary>
    /// Gets a memory segment that represents a vector of input values.
    /// </summary>
    public Memory<TScalar> Inputs { get; }

    /// <summary>
    /// Gets a memory segment that represents a vector of output values.
    /// </summary>
    public Memory<TScalar> Outputs { get; }

    /// <summary>
    /// Activate the network. Activation reads input signals from InputSignalArray and writes output signals
    /// to OutputSignalArray.
    /// </summary>
    public void Activate()
    {
        ReadOnlySpan<int> srcIds = _connIds.GetSourceIdSpan();
        ReadOnlySpan<int> tgtIds = _connIds.GetTargetIdSpan();
        ReadOnlySpan<TScalar> weights = _weightArr.AsSpan();
        Span<TScalar> activations = _activationMem.Span;
        Span<TScalar> outputs = _outputMem.Span;
        Span<int> outputNodeIdxs = _outputNodeIdxArr.AsSpan();
        Span<TScalar> connInputs = _conInputArr.AsSpan();

        ref int srcIdsRef = ref MemoryMarshal.GetReference(srcIds);
        ref int tgtIdsRef = ref MemoryMarshal.GetReference(tgtIds);
        ref TScalar weightsRef = ref MemoryMarshal.GetReference(weights);
        ref TScalar activationsRef = ref MemoryMarshal.GetReference(activations);
        ref TScalar outputsRef = ref MemoryMarshal.GetReference(outputs);
        ref int outputNodeIdxsRef = ref MemoryMarshal.GetReference(outputNodeIdxs);
        ref TScalar connInputsRef = ref MemoryMarshal.GetReference(connInputs);

        // Reset hidden and output node activation levels.
        // Notes.
        // The reset is performed here because the activations memory may not be initialized/zeroed as it was
        // obtained via ArrayPool.Rent(). Furthermore, the output node signals must maintain their values
        // after exiting this method, to allow the caller to read the output signal values.
        activations.Slice(_inputCount).Clear();

        // Process all layers in turn.
        int conIdx = 0;
        int nodeIdx = _inputCount;

        // Loop through network layers.
        for(int layerIdx=0; layerIdx < _layerInfoArr.Length - 1; layerIdx++)
        {
            LayerInfo layerInfo = _layerInfoArr[layerIdx];

            // Push signals through the current layer's connections to the target nodes (that are all in 'downstream' layers).
            for(; conIdx <= layerInfo.EndConnectionIdx - Vector<TScalar>.Count; conIdx += Vector<TScalar>.Count)
            {
                // Load source node output values into a vector.
                ref int srcIdsRefSeg = ref Unsafe.Add(ref srcIdsRef, conIdx);

                for(int i=0; i < Vector<TScalar>.Count; i++)
                {
                    Unsafe.Add(ref connInputsRef, i) =
                        Unsafe.Add(
                            ref activationsRef,
                            Unsafe.Add(ref srcIdsRefSeg, i));
                }

                // Note. This obscure pattern is taken from the Vector<T> constructor source code.
                var conVec = Unsafe.ReadUnaligned<Vector<TScalar>>(
                    ref Unsafe.As<TScalar, byte>(ref connInputsRef));

                // Load connection weights into a vector.
                var weightVec = Unsafe.ReadUnaligned<Vector<TScalar>>(
                    ref Unsafe.As<TScalar, byte>(
                        ref Unsafe.Add(
                            ref weightsRef, conIdx)));

                // Multiply connection source inputs and connection weights.
                conVec *= weightVec;

                // Save/accumulate connection output values onto the connection target nodes.
                ref int tgtIdsRefSeg = ref Unsafe.Add(ref tgtIdsRef, conIdx);

                for(int i=0; i < Vector<TScalar>.Count; i++)
                    Unsafe.Add(ref activationsRef, Unsafe.Add(ref tgtIdsRefSeg, i)) += conVec[i];
            }

            // Loop remaining connections
            for(; conIdx < layerInfo.EndConnectionIdx; conIdx++)
            {
                // Get a reference to the target activation level 'slot' in the activations span.
                ref TScalar tgtSlot = ref Unsafe.Add(ref activationsRef, Unsafe.Add(ref tgtIdsRef, conIdx));

                // Get the connection source signal, multiply it by the connection weight, add the result
                // to the target node's current pre-activation level, and store the result.
                tgtSlot = TScalar.FusedMultiplyAdd(
                    Unsafe.Add(ref activationsRef, Unsafe.Add(ref srcIdsRef, conIdx)),
                    Unsafe.Add(ref weightsRef, conIdx),
                    tgtSlot);
            }

            // Activate the next layer's nodes. This is possible because we know that all connections that
            // target these nodes have been processed, either during processing on the current layer's
            // connections, or earlier layers. This means that the final output value/signal (i.e post
            // activation function output) is available for all connections and nodes in the lower/downstream
            // layers.
            //
            // Pass the pre-activation levels through the activation function.
            // Note. The resulting post-activation levels are stored in _activationArr.
            layerInfo = _layerInfoArr[layerIdx + 1];
            _activationFn(
                ref Unsafe.Add(ref activationsRef, nodeIdx),
                layerInfo.EndNodeIdx - nodeIdx);

            // Update nodeIdx to point at first node in the next layer.
            nodeIdx = layerInfo.EndNodeIdx;
        }

        // Copy the output signals from _activationMem into _outputMem.
        // These signals are scattered through _activationMem, and here we bring them together into a
        // contiguous segment of memory that is indexable by output index.
        for(int i=0; i < outputNodeIdxs.Length; i++)
        {
            ref TScalar outputSlot = ref Unsafe.Add(ref outputsRef, i);
            outputSlot = Unsafe.Add(ref activationsRef, Unsafe.Add(ref outputNodeIdxsRef, i));
        }
    }

    /// <summary>
    /// Reset the network's internal state.
    /// </summary>
    public void Reset()
    {
        // Unnecessary for this implementation. The node activation signal state is completely overwritten on each activation.
    }

    #endregion

    #region IDisposable

    /// <inheritdoc/>
    public void Dispose()
    {
        if(!_isDisposed)
        {
            _isDisposed = true;
            ArrayPool<TScalar>.Shared.Return(_workingArr);
        }
    }

    #endregion
}
