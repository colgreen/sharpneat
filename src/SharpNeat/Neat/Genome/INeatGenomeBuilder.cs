// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;

namespace SharpNeat.Neat.Genome;

/// <summary>
/// For creating new instances of <see cref="NeatGenome{T}"/>.
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
public interface INeatGenomeBuilder<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    /// <summary>
    /// Create a NeatGenome with the given meta data and connection genes.
    /// </summary>
    /// <param name="id">Genome ID.</param>
    /// <param name="birthGeneration">Birth generation.</param>
    /// <param name="connGenes">Connection genes.</param>
    /// <returns>A new NeatGenome instance.</returns>
    NeatGenome<TScalar> Create(
        int id,
        int birthGeneration,
        ConnectionGenes<TScalar> connGenes);

    /// <summary>
    /// Create a NeatGenome with the given meta data, connection genes and supplementary data.
    /// </summary>
    /// <param name="id">Genome ID.</param>
    /// <param name="birthGeneration">Birth generation.</param>
    /// <param name="connGenes">Connection genes.</param>
    /// <param name="hiddenNodeIdArr">An array of the hidden node IDs in the genome, in ascending order.</param>
    /// <returns>A new NeatGenome instance.</returns>
    NeatGenome<TScalar> Create(
        int id, int birthGeneration,
        ConnectionGenes<TScalar> connGenes,
        int[] hiddenNodeIdArr);

    /// <summary>
    /// Create a NeatGenome with the given meta data, connection genes and supplementary data.
    /// </summary>
    /// <param name="id">Genome ID.</param>
    /// <param name="birthGeneration">Birth generation.</param>
    /// <param name="connGenes">Connection genes.</param>
    /// <param name="hiddenNodeIdArr">An array of the hidden node IDs in the genome, in ascending order.</param>
    /// <param name="nodeIndexByIdMap">Provides a mapping from node ID to node index.</param>
    /// <returns>A new NeatGenome instance.</returns>
    NeatGenome<TScalar> Create(
        int id, int birthGeneration,
        ConnectionGenes<TScalar> connGenes,
        int[] hiddenNodeIdArr,
        INodeIdMap nodeIndexByIdMap);

    /// <summary>
    /// Create a NeatGenome with the given meta data, connection genes and supplementary data.
    /// </summary>
    /// <param name="id">Genome ID.</param>
    /// <param name="birthGeneration">Birth generation.</param>
    /// <param name="connGenes">Connection genes.</param>
    /// <param name="hiddenNodeIdArr">An array of the hidden node IDs in the genome, in ascending order.</param>
    /// <param name="nodeIndexByIdMap">Provides a mapping from node ID to node index.</param>
    /// <param name="digraph">A DirectedGraph that mirrors the structure described by the connection genes.</param>
    /// <param name="connectionIndexMap">Mapping from genome connection indexes (in NeatGenome.ConnectionGenes) to reordered connections, based on depth based
    /// node index allocations.</param>
    /// <returns>A new NeatGenome instance.</returns>
    NeatGenome<TScalar> Create(
        int id, int birthGeneration,
        ConnectionGenes<TScalar> connGenes,
        int[] hiddenNodeIdArr,
        INodeIdMap nodeIndexByIdMap,
        DirectedGraph digraph,
        int[]? connectionIndexMap);
}
