// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.BlackBox;

namespace SharpNeat.Tasks.FunctionRegression;

/// <summary>
/// For probing and recording the responses of instances of <see cref="IBlackBox{T}"/>.
/// </summary>
public interface IBlackBoxProbe
{
    // TODO: Convert responseArr to Span<double>
    /// <summary>
    /// Probe the given black box, and record the responses in <paramref name="responseArr"/>.
    /// </summary>
    /// <param name="box">The black box to probe.</param>
    /// <param name="responseArr">Response array.</param>
    void Probe(IBlackBox<double> box, double[] responseArr);
}
