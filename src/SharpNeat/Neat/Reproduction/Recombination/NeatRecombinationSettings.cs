// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Neat.Reproduction.Recombination;

/// <summary>
/// Settings related to <see cref="NeatRecombination{T}"/>.
/// </summary>
public class NeatRecombinationSettings
{
    /// <summary>
    /// The probability that a gene that exists only on the secondary parent is copied into the child genome.
    /// </summary>
    public double SecondaryParentGeneProbability { get; set; } = 0.1;

    /// <summary>
    /// Validate the settings, and throw an exception if not valid.
    /// </summary>
    /// <remarks>
    /// As a 'simple' collection of properties there is no construction time check that can be performed, therefore this method is supplied to
    /// allow consumers of a settings object to validate it before using it.
    /// </remarks>
    public void Validate()
    {
        if(!IsProbability(SecondaryParentGeneProbability))
            throw new InvalidOperationException("SecondaryParentGeneProbability must be in the interval [0,1].");

        static bool IsProbability(double p) => p >= 0 && p <= 1.0;
    }
}
