// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;

namespace SharpNeat.Tasks.FunctionRegression;

/// <summary>
/// Parameter sampling info.
/// Describes the value range to sample, the number of samples within that range, and the increment between samples.
/// </summary>
public readonly struct ParamSamplingInfo
{
    /// <summary>
    /// Sample interval minimum.
    /// </summary>
    public double Min { get; }

    /// <summary>
    /// Sample interval maximum.
    /// </summary>
    public double Max { get; }

    /// <summary>
    /// Intra sample increment.
    /// </summary>
    public double Incr { get; }

    /// <summary>
    /// Sampling resolution, within the defined min-max interval.
    /// </summary>
    public int SampleResolution { get; }

    /// <summary>
    /// X positions of the sample points.
    /// </summary>
    public double[] XArr { get; }

    /// <summary>
    /// X positions of the sample points in the neural net input space (i.e. scaled from 0 to 1).
    /// </summary>
    public double[] XArrNetwork { get; }

    /// <summary>
    /// Construct with the provided parameter sampling info.
    /// </summary>
    /// <param name="min">Sample interval minimum.</param>
    /// <param name="max">Sample interval maximum.</param>
    /// <param name="resolution">Sampling resolution.</param>
    public ParamSamplingInfo(double min, double max, int resolution)
    {
        Debug.Assert(resolution>=3, "Sample count must be >= 3");
        Min = min;
        Max = max;
        double incr = Incr = (max-min) / (resolution - 1);
        SampleResolution = resolution;

        double incrNet = 1.0 / (resolution - 1);
        double x = min;
        double xNet = 0.0;
        XArr = new double[resolution];
        XArrNetwork = new double[resolution];

        for(int i=0; i < resolution; i++, x += incr, xNet += incrNet)
        {
            XArr[i] = x;
            XArrNetwork[i] = xNet;
        }
    }
}
