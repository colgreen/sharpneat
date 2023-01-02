// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Buffers;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpNeat.Graphs.Acyclic;

namespace SharpNeat.NeuralNets.Double.Vectorized;

/// <summary>
/// A version of <see cref="Double.NeuralNetAcyclic"/> that utilises some vectorized operations
/// for improved performance on hardware platforms that support them.
/// </summary>
public sealed class NeuralNetAcyclic : IBlackBox<double>
{
    // Connection arrays.
    readonly ConnectionIds _connIds;
    readonly double[] _weightArr;

    // Array of layer information.
    readonly LayerInfo[] _layerInfoArr;

    // Node activation function.
    readonly VecFn<double> _activationFn;

    // Working array. Used for node activation signals, and a separate output signal segment on the end.
    readonly double[] _workingArr;

    // Node activation signal segment (used for both pre and post activation levels).
    readonly Memory<double> _activationMem;

    // Output signal segment.
    // The output signal values are copied into this array from _activationMem (see notes below).
    readonly Memory<double> _outputMem;

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
    readonly double[] _conInputArr = new double[Vector<double>.Count];
    volatile bool _isDisposed;

    #region Constructors

    /// <summary>
    /// Constructs a AcyclicNeuralNet with the provided neural net definition parameters.
    /// </summary>
    /// <param name="digraph">Network structure definition.</param>
    /// <param name="activationFn">Node activation function.</param>
    public NeuralNetAcyclic(
        WeightedDirectedGraphAcyclic<double> digraph,
        VecFn<double> activationFn)
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
        double[] weightArr,
        VecFn<double> activationFn)
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
        _workingArr = ArrayPool<double>.Shared.Rent(_totalNodeCount + _outputCount);
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
    public Memory<double> Inputs { get; }

    /// <summary>
    /// Gets a memory segment that represents a vector of output values.
    /// </summary>
    public Memory<double> Outputs { get; }

    /// <summary>
    /// Activate the network. Activation reads input signals from InputSignalArray and writes output signals
    /// to OutputSignalArray.
    /// </summary>
    public void Activate()
    {
        ReadOnlySpan<int> srcIds = _connIds.GetSourceIdSpan();
        ReadOnlySpan<int> tgtIds = _connIds.GetTargetIdSpan();
        ReadOnlySpan<double> weights = _weightArr.AsSpan();
        Span<double> activations = _activationMem.Span;
        Span<double> outputs = _outputMem.Span;
        Span<int> outputNodeIdxs = _outputNodeIdxArr.AsSpan();
        Span<double> connInputs = _conInputArr.AsSpan();

        ref int srcIdsRef = ref MemoryMarshal.GetReference(srcIds);
        ref int tgtIdsRef = ref MemoryMarshal.GetReference(tgtIds);
        ref double weightsRef = ref MemoryMarshal.GetReference(weights);
        ref double activationsRef = ref MemoryMarshal.GetReference(activations);
        ref double outputsRef = ref MemoryMarshal.GetReference(outputs);
        ref int outputNodeIdxsRef = ref MemoryMarshal.GetReference(outputNodeIdxs);
        ref double connInputsRef = ref MemoryMarshal.GetReference(connInputs);

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
            for(; conIdx <= layerInfo.EndConnectionIdx - Vector<double>.Count; conIdx += Vector<double>.Count)
            {
                // Load source node output values into a vector.
                ref int srcIdsRefSeg = ref Unsafe.Add(ref srcIdsRef, conIdx);

                for(int i=0; i < Vector<double>.Count; i++)
                {
                    Unsafe.Add(ref connInputsRef, i) =
                        Unsafe.Add(
                            ref activationsRef,
                            Unsafe.Add(ref srcIdsRefSeg, i));
                }

                // Note. This obscure pattern is taken from the Vector<T> constructor source code.
                var conVec = Unsafe.ReadUnaligned<Vector<double>>(
                    ref Unsafe.As<double, byte>(ref connInputsRef));

                // Load connection weights into a vector.
                var weightVec = Unsafe.ReadUnaligned<Vector<double>>(
                    ref Unsafe.As<double, byte>(
                        ref Unsafe.Add(
                            ref weightsRef, conIdx)));

                // Multiply connection source inputs and connection weights.
                conVec *= weightVec;

                // Save/accumulate connection output values onto the connection target nodes.
                ref int tgtIdsRefSeg = ref Unsafe.Add(ref tgtIdsRef, conIdx);

                for(int i=0; i < Vector<double>.Count; i++)
                    Unsafe.Add(ref activationsRef, Unsafe.Add(ref tgtIdsRefSeg, i)) += conVec[i];
            }

            // Loop remaining connections
            for(; conIdx < layerInfo.EndConnectionIdx; conIdx++)
            {
                // Get a reference to the target activation level 'slot' in the activations span.
                ref double tgtSlot = ref Unsafe.Add(ref activationsRef, Unsafe.Add(ref tgtIdsRef, conIdx));

                // Get the connection source signal, multiply it by the connection weight, add the result
                // to the target node's current pre-activation level, and store the result.
                tgtSlot = Math.FusedMultiplyAdd(
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
            ref double outputSlot = ref Unsafe.Add(ref outputsRef, i);
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
            ArrayPool<double>.Shared.Return(_workingArr);
        }
    }

    #endregion
}
