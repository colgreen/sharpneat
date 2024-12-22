// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Neat.Genome;

/// <summary>
/// Static factory class for creating instances of <see cref="INeatGenomeBuilder{T}"/>.
/// </summary>
/// <typeparam name="TWeight">Connection weight data type.</typeparam>
public static class NeatGenomeBuilderFactory<TWeight>
    where TWeight : unmanaged
{
    /// <summary>
    /// Create a new instance of <see cref="INeatGenomeBuilder{T}"/>.
    /// </summary>
    /// <param name="metaNeatGenome">Neat genome metadata.</param>
    /// <param name="validateAcyclic">Enable acyclic graph validation.</param>
    /// <returns>A new instance of <see cref="INeatGenomeBuilder{T}"/>.</returns>
    /// <remarks>
    /// If the caller can guarantee that calls to Create() will provide acyclic graphs only when metaNeatGenome.IsAcyclic is true, then
    /// <paramref name="validateAcyclic"/> can be set to false to avoid the cost of the cyclic graph check (which is relatively expensive to perform).
    /// </remarks>
    public static INeatGenomeBuilder<TWeight> Create(
        MetaNeatGenome<TWeight> metaNeatGenome,
        bool validateAcyclic = false)
    {
        if(metaNeatGenome.IsAcyclic)
        {
            return new NeatGenomeBuilderAcyclic<TWeight>(metaNeatGenome, validateAcyclic);
        }

        return new NeatGenomeBuilderCyclic<TWeight>(metaNeatGenome);
    }
}
