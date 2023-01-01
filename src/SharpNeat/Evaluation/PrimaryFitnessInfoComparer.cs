// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Evaluation;

/// <summary>
/// For comparing the primary fitness of two <see cref="FitnessInfo"/> instances.
/// </summary>
public sealed class PrimaryFitnessInfoComparer : IComparer<FitnessInfo>
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static readonly PrimaryFitnessInfoComparer Singleton = new();

    /// <summary>
    /// Compares two instances of <see cref="FitnessInfo"/> and returns a value indicating
    /// whether one is less than, equal to, or greater than the other.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />.</returns>
    public int Compare(FitnessInfo x, FitnessInfo y)
    {
        return x.PrimaryFitness.CompareTo(y.PrimaryFitness);
    }
}
