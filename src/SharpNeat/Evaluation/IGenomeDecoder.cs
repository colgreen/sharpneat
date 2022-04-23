// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Evaluation;

/// <summary>
/// Represents types that decode genomes into phenomes.
/// </summary>
/// <typeparam name="TGenome">The genome type to be decoded.</typeparam>
/// <typeparam name="TPhenome">The phenome type that is decoded to.</typeparam>
public interface IGenomeDecoder<TGenome,TPhenome>
    where TPhenome : IDisposable
{
    /// <summary>
    /// Decodes a genome into a phenome.
    /// </summary>
    /// <param name="genome">The genome to decode.</param>
    /// <returns>A phenome if the genome was valid and therefore decoded successfully; otherwise null.</returns>
    /// <remarks>
    /// Note that not all genomes have to decode successfully. That is, we support genetic representations
    /// that may produce non-viable offspring. In such cases this method is allowed to return a null.
    /// </remarks>
    TPhenome Decode(TGenome genome);
}
