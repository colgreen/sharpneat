// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Evaluation;
using SharpNeat.EvolutionAlgorithm;
using SharpNeat.Graphs;

namespace SharpNeat.Neat.Genome;

/// <summary>
/// Represents a NEAT genome, i.e the genetic representation of a neural network.
/// </summary>
/// <typeparam name="T">Neural net numeric data type.</typeparam>
public class NeatGenome<T> : IGenome
    where T : struct
{
    #region Auto Properties [IGenome]

    /// <summary>
    /// Genome ID.
    /// </summary>
    public int Id { get; }

    // TODO: Consider whether birthGeneration belongs here.
    /// <summary>
    /// Genome birth generation.
    /// </summary>
    public int BirthGeneration { get; }

    /// <summary>
    /// Genome fitness info.
    /// </summary>
    public FitnessInfo FitnessInfo { get; set; }

    /// <summary>
    /// Gets a value that is representative of the genome's complexity.
    /// </summary>
    /// <remarks>
    /// For a NEAT genome we take the number of connections as representative of genome complexity.
    /// </remarks>
    public double Complexity { get => ConnectionGenes._connArr.Length; }

    #endregion

    #region Auto Properties [NEAT Genome]

    /// <summary>
    /// Genome metadata.
    /// </summary>
    public MetaNeatGenome<T> MetaNeatGenome { get; }

    /// <summary>
    /// Connection genes data structure.
    /// These define both the neural network structure/topology and the connection weights.
    /// </summary>
    public ConnectionGenes<T> ConnectionGenes { get; }

    #endregion

    #region Auto Properties [Supplemental/Cached Data Structures]

    /// <summary>
    /// An array of hidden node IDs, sorted to allow efficient lookup of an ID with a binary search.
    /// Input and output node IDs are not included because these are allocated fixed IDs starting from zero
    /// and are therefore always known.
    /// </summary>
    public int[] HiddenNodeIdArray { get; }

    /// <summary>
    /// Mapping function for obtaining a node index for a given node ID.
    /// </summary>
    /// <remarks>
    /// Node indexes have a range of 0 to N-1 (for N nodes), i.e. the indexes are zero based and contiguous; as opposed to node IDs, which are not contiguous.
    /// </remarks>
    public INodeIdMap NodeIndexByIdMap { get; }

    /// <summary>
    /// The directed graph that the current genome represents.
    /// </summary>
    /// <remarks>
    /// This digraph mirrors the graph described by <see cref="ConnectionGenes"/>; this object represents the
    /// graph structure only, not the weights, and is therefore re-used when spawning genomes with the same structure
    /// (i.e. a child that is the result of connection weight mutations only).
    /// The DirectedGraph class provides an efficient means of working with graphs and is therefore made available
    /// on this class to provide improved performance for:
    ///  * Decoding to a neural net object.
    ///  * Finding new connections on acyclic graphs, i.e. detecting if a random new connection would form a cycle.
    ///
    /// Note. When MetaNeatGenome.IsAcyclic is true then the object stored here will be of the subtype <see cref="Graphs.Acyclic.DirectedGraphAcyclic"/> .
    /// </remarks>
    public DirectedGraph DirectedGraph { get; }

    /// <summary>
    /// Cached info related to acyclic digraphs only.
    ///
    /// Represents a mapping between genome connection indexes (in NeatGenome.ConnectionGenes), to reordered connections
    /// based on depth based node index allocations (as utilised in DirectedGraphAcyclic).
    ///
    /// This allows for mapping of weights from NeatGenome.ConnectionGenes to the re-ordered weight array used by the neural
    /// net implementation (AcyclicNeuralNet).
    ///
    /// The mapping is in the form of an array of indexes into NeatGenome.ConnectionGenes, i.e. the position in the index
    /// is the 'new' index (the digraph index), and the value stored at that position is the 'old' index (the genome
    /// connection index).
    /// </summary>
    public int[]? ConnectionIndexMap { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructs with the provided ID, birth generation and gene arrays.
    /// </summary>
    /// <param name="metaNeatGenome">Meta NEAT genome.</param>
    /// <param name="id">Genome ID.</param>
    /// <param name="birthGeneration">Genome birth generation.</param>
    /// <param name="connGenes">Genome connection genes.</param>
    /// <param name="hiddenNodeIdArr">Array of hidden node IDs.</param>
    /// <param name="nodeIndexByIdMap">A mapping from node IDs to node indexes.</param>
    /// <param name="digraph">A directed graph that represents the neural network structure.</param>
    /// <param name="connectionIndexMap">A mapping between genome connection indexes (in <paramref name="connGenes"/>),
    /// to reordered connections based on depth based node index allocations (optional, acyclic genomes only).</param>
    internal NeatGenome(
        MetaNeatGenome<T> metaNeatGenome,
        int id,
        int birthGeneration,
        ConnectionGenes<T> connGenes,
        int[] hiddenNodeIdArr,
        INodeIdMap nodeIndexByIdMap,
        DirectedGraph digraph,
        int[]? connectionIndexMap)
    {
#if DEBUG

        NeatGenomeAssertions<T>.AssertIsValid(
            metaNeatGenome, id, birthGeneration,
            connGenes, hiddenNodeIdArr, nodeIndexByIdMap,
            digraph, connectionIndexMap);

#endif

        MetaNeatGenome = metaNeatGenome;
        Id = id;
        BirthGeneration = birthGeneration;
        ConnectionGenes = connGenes;
        HiddenNodeIdArray = hiddenNodeIdArr;
        NodeIndexByIdMap = nodeIndexByIdMap;
        DirectedGraph = digraph;
        ConnectionIndexMap = connectionIndexMap;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Tests if the genome contains a connection that refers to the given hidden node ID.
    /// </summary>
    /// <param name="id">Node ID.</param>
    /// <returns>True if the genome contains a hidden node with the specified ID; otherwise false.</returns>
    public bool ContainsHiddenNode(int id)
    {
        return Array.BinarySearch(this.HiddenNodeIdArray, id) >= 0;
    }

    /// <summary>
    /// Gets an array of connection weights, ordered to match the connections of <see cref="DirectedGraph"/>.
    /// </summary>
    /// <returns>An array of connection weights.</returns>
    /// <remarks>
    /// For cyclic genomes this is simply the genome's weight array, but for acyclic genomes the digraph and genome
    /// represent connections in a different order, thus for acyclic genomes/digraphs this method will return a new
    /// array with the weights in the digraph order.
    /// </remarks>
    public T[] GetDigraphWeightArray()
    {
        // If the genome represents a cyclic graph then the genome connections are in the same order as the digraph
        // connections, and thus the weights are in the same order too, therefore we can just return the genome weight
        // array as is. We can do this because these arrays are treated as being immutable, i.e., a given genome's weight
        // array will never be changed. E.g. Weight mutation occurs on child genomes that have a copy of the parent
        // genome's weight array.
        if(!this.MetaNeatGenome.IsAcyclic)
        {
            return this.ConnectionGenes._weightArr;
        }

        // For acyclic genomes the digraph connections are ordered by depth of the source node in graph, and thus the
        // connection weights are in a different order, therefore we create a new array, copy the weights into their
        // digraph positions, and return the new array.

        // Create a new weight array, and copy in the weights from the genome into their correct positions.
        T[] genomeWeightArr = this.ConnectionGenes._weightArr;
        T[] digraphWeightArr = new T[genomeWeightArr.Length];
        int[] connIdxMap = this.ConnectionIndexMap!;

        for(int i=0; i < connIdxMap.Length; i++)
            digraphWeightArr[i] = genomeWeightArr[connIdxMap[i]];

        return digraphWeightArr;
    }

    #endregion
}
