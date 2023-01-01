// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Windows.App;

/// <summary>
/// Miscellaneous utility objects and methods.
/// </summary>
internal static class Utils
{
    /// <summary>
    /// Gets a <see cref="Comparer{double}"/> that can be used to sort items in descending order.
    /// </summary>
    public static Comparer<double> ComparerDesc => Comparer<double>.Create(
        delegate (double x, double y)
        {
            if(x > y) { return -1; }
            if(x < y) { return 1; }
            return 0;
        });
}
