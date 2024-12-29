// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using System.Numerics;

namespace SharpNeat.Tasks.FunctionRegression;

/// <summary>
/// For probing and recording the responses of instances of <see cref="IBlackBox{T}"/>.
/// </summary>
/// <typeparam name="TScalar">Black box input/output data type.</typeparam>
public sealed class BlackBoxProbe<TScalar> : IBlackBoxProbe<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    static readonly TScalar Half = TScalar.CreateChecked(0.5);

    readonly ParamSamplingInfo<TScalar> _paramSamplingInfo;
    readonly TScalar _offset;
    readonly TScalar _scale;

    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="paramSamplingInfo">Parameter sampling info.</param>
    /// <param name="offset">Offset to apply to each neural network output response.</param>
    /// <param name="scale">Scaling factor to apply to each neural network output response.</param>
    public BlackBoxProbe(ParamSamplingInfo<TScalar> paramSamplingInfo, TScalar offset, TScalar scale)
    {
        _paramSamplingInfo = paramSamplingInfo;
        _offset = offset;
        _scale = scale;
    }

    /// <inheritdoc/>
    public void Probe(IBlackBox<TScalar> box, TScalar[] responseArr)
    {
        Debug.Assert(responseArr.Length == _paramSamplingInfo.SampleResolution);

        // Get the blackbox input and output spans.
        var inputs = box.Inputs.Span;
        var outputs = box.Outputs.Span;

        TScalar[] xArr = _paramSamplingInfo.XArrNetwork;
        for(int i=0; i < xArr.Length; i++)
        {
            // Reset black box internal state.
            box.Reset();

            // Set bias input, and function input value.
            inputs[0] = TScalar.One;
            inputs[1] = xArr[i];

            // Activate the black box.
            box.Activate();

            // Get the black box's output value.
            // TODO: Review this scheme. This replicates the behaviour in SharpNEAT 2.x but not sure if it's ideal,
            // for one it depends on the output range of the neural net activation function in use.
            TScalar output = outputs[0];
            Clip(ref output);
            responseArr[i] = ((output - Half) * _scale) + _offset;
        }
    }

    private static void Clip(ref TScalar x)
    {
        if(x < TScalar.Zero) x = TScalar.Zero;
        else if(x > TScalar.One) x = TScalar.One;
    }
}
