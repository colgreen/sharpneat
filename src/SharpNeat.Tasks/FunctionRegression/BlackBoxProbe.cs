// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;

namespace SharpNeat.Tasks.FunctionRegression;

/// <summary>
/// For probing and recording the responses of instances of <see cref="IBlackBox{T}"/>.
/// </summary>
public sealed class BlackBoxProbe : IBlackBoxProbe
{
    readonly ParamSamplingInfo _paramSamplingInfo;
    readonly double _offset;
    readonly double _scale;

    #region Constructor

    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="paramSamplingInfo">Parameter sampling info.</param>
    /// <param name="offset">Offset to apply to each neural network output response.</param>
    /// <param name="scale">Scaling factor to apply to each neural network output response.</param>
    public BlackBoxProbe(ParamSamplingInfo paramSamplingInfo, double offset, double scale)
    {
        _paramSamplingInfo = paramSamplingInfo;
        _offset = offset;
        _scale = scale;
    }

    #endregion

    #region Public Methods

    /// <inheritdoc/>
    public void Probe(IBlackBox<double> box, double[] responseArr)
    {
        Debug.Assert(responseArr.Length == _paramSamplingInfo.SampleResolution);

        // Get the blackbox input and output spans.
        var inputs = box.Inputs.Span;
        var outputs = box.Outputs.Span;

        double[] xArr = _paramSamplingInfo.XArrNetwork;
        for(int i=0; i < xArr.Length; i++)
        {
            // Reset black box internal state.
            box.Reset();

            // Set bias input, and function input value.
            inputs[0] = 1.0;
            inputs[1] = xArr[i];

            // Activate the black box.
            box.Activate();

            // Get the black box's output value.
            // TODO: Review this scheme. This replicates the behaviour in SharpNEAT 2.x but not sure if it's ideal,
            // for one it depends on the output range of the neural net activation function in use.
            double output = outputs[0];
            Clip(ref output);
            responseArr[i] = ((output - 0.5) * _scale) + _offset;
        }
    }

    #endregion

    #region Private Static Methods

    private static void Clip(ref double x)
    {
        if(x < 0.0) x = 0.0;
        else if(x > 1.0) x = 1.0;
    }

    #endregion
}
