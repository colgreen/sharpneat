// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using System.Numerics;

namespace SharpNeat.Tasks.FunctionRegression;

/// <summary>
/// Parameter sampling info.
/// Describes the value range to sample, the number of samples within that range, and the increment between samples.
/// </summary>
public readonly struct ParamSamplingInfo<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    /// <summary>
    /// Sample interval minimum.
    /// </summary>
    public TScalar Min { get; }

    /// <summary>
    /// Sample interval maximum.
    /// </summary>
    public TScalar Max { get; }

    /// <summary>
    /// Intra sample increment.
    /// </summary>
    public TScalar Incr { get; }

    /// <summary>
    /// Sampling resolution, within the defined min-max interval.
    /// </summary>
    public int SampleResolution { get; }

    /// <summary>
    /// X positions of the sample points.
    /// </summary>
    public TScalar[] XArr { get; }

    /// <summary>
    /// X positions of the sample points in the neural net input space (i.e. scaled from 0 to 1).
    /// </summary>
    public TScalar[] XArrNetwork { get; }

    /// <summary>
    /// Construct with the provided parameter sampling info.
    /// </summary>
    /// <param name="min">Sample interval minimum.</param>
    /// <param name="max">Sample interval maximum.</param>
    /// <param name="resolution">Sampling resolution.</param>
    public ParamSamplingInfo(TScalar min, TScalar max, int resolution)
    {
        Debug.Assert(resolution>=3, "Sample count must be >= 3");
        Min = min;
        Max = max;
        TScalar incr = Incr = (max-min) / TScalar.CreateChecked((resolution - 1));
        SampleResolution = resolution;

        TScalar incrNet = TScalar.Zero / TScalar.CreateChecked(resolution - 1);
        TScalar x = min;
        TScalar xNet = TScalar.Zero;
        XArr = new TScalar[resolution];
        XArrNetwork = new TScalar[resolution];

        for(int i=0; i < resolution; i++, x += incr, xNet += incrNet)
        {
            XArr[i] = x;
            XArrNetwork[i] = xNet;
        }
    }
}
