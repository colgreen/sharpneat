// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using System.Numerics;
using SharpNeat.Tasks.FunctionRegression;

namespace SharpNeat.Tasks.GenerativeFunctionRegression;

/// <summary>
/// For probing and recording the responses of instances of <see cref="IBlackBox{T}"/>.
/// </summary>
/// <typeparam name="TScalar">Black box input/output data type.</typeparam>
public sealed class GenerativeBlackBoxProbe<TScalar> : IBlackBoxProbe<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    static readonly TScalar Half = TScalar.CreateSaturating(0.5);

    readonly int _sampleCount;
    readonly TScalar _offset;
    readonly TScalar _scale;

    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="sampleCount">The number of generative samples to take.</param>
    /// <param name="offset">Offset to apply to each black box output response.</param>
    /// <param name="scale">Scaling factor to apply to each black box output response.</param>
    public GenerativeBlackBoxProbe(int sampleCount, TScalar offset, TScalar scale)
    {
        _sampleCount = sampleCount;
        _offset = offset;
        _scale = scale;
    }

    /// <inheritdoc/>
    public void Probe(IBlackBox<TScalar> box, TScalar[] responseArr)
    {
        Debug.Assert(responseArr.Length == _sampleCount);

        // Reset black box internal state.
        box.Reset();

        // Get the blackbox input and output spans.
        var inputs = box.Inputs.Span;
        var outputs = box.Outputs.Span;

        // Set bias input.
        // This will remain set for the lifetime of the below loops.
        inputs[0] = TScalar.One;

        // Perform some warm-up activations of the neural net.
        for(int i = 0; i < 3; i++)
        {
            box.Activate();
        }

        // Take the required number of samples.
        for(int i=0; i < _sampleCount; i++)
        {
            // Activate the black box.
            box.Activate();

            // Get the black box's output value.
            // TODO: Review this scheme. This replicates the behaviour in SharpNEAT 2.x but not sure if it's ideal,
            // for one it depends on the output range of the neural net activation function in use.
            responseArr[i] = ((outputs[0] - Half) * _scale) + _offset;
        }
    }
}
