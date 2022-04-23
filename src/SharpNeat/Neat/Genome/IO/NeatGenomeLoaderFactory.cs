// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Globalization;

namespace SharpNeat.Neat.Genome.IO;

/// <summary>
/// <see cref="NeatGenomeLoader{Double}"/> factory.
/// </summary>
public static class NeatGenomeLoaderFactory
{
    #region Public Static Methods

    /// <summary>
    /// Create a new instance of <see cref="NeatGenomeLoader{Double}"/>.
    /// </summary>
    /// <param name="metaNeatGenome">Meta neat genome.</param>
    /// <returns>A new instance of <see cref="NeatGenomeLoader{Double}"/>.</returns>
    public static NeatGenomeLoader<double> CreateLoaderDouble(
        MetaNeatGenome<double> metaNeatGenome)
    {
        return new NeatGenomeLoader<double>(metaNeatGenome, TryParseWeight);
    }

    #endregion

    #region Private Static Methods

    private static (double,bool) TryParseWeight(string str)
    {
        (double, bool) result;
        result.Item2 = double.TryParse(
            str,
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
            CultureInfo.InvariantCulture,
            out result.Item1);

        return result;
    }

    #endregion
}
