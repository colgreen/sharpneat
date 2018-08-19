using System;
using System.Diagnostics;
using SharpNeat.EvolutionAlgorithm;
using SharpNeat.Evaluation;
using SharpNeat.Network;
using SharpNeat.Network.Acyclic;

namespace SharpNeat.Neat.Genome
{
    /// <summary>
    /// Represents a NEAT genome, i.e the genetic representation of a neural network.
    /// </summary>
    /// <typeparam name="T">Connection weight and signal data type.</typeparam>
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
        /// Note. When MetaNeatGenome.IsAcyclic is true then the object stored here will be of the subtype AcyclicDirectedGraph.
        /// </remarks>
        public DirectedGraph DirectedGraph { get; }

        /// <summary>
        /// Cached info related to acyclic digraphs only.
        /// 
        /// Represents a mapping between genome connection indexes (in NeatGenome.ConnectionGenes), to reordered connections
        /// based on depth based node index allocations (as utilised in AcyclicDirectedGraph).
        /// 
        /// This allows for mapping of weights from NeatGenome.ConnectionGenes to the re-ordered weight array used by the neural
        /// net implementation (AcyclicNeuralNet).
        /// 
        /// The mapping is in the form of an array of indexes into NeatGenome.ConnectionGenes, i.e. the position in the index
        /// is the 'new' index (the digraph index), and the value stored at that position is the 'old' index (the genome 
        /// connection index).
        /// </summary>
        public int[] ConnectionIndexMap { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs with the provided ID, birth generation and gene arrays.
        /// </summary>
        internal NeatGenome(
            MetaNeatGenome<T> metaNeatGenome,
            int id,
            int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr,
            INodeIdMap nodeIndexByIdMap,
            DirectedGraph digraph,
            int[] connectionIndexMap)
        {
            #if DEBUG

            NeatGenomeAssertions<T>.Assert(
                metaNeatGenome, id, birthGeneration,
                connGenes, hiddenNodeIdArr, nodeIndexByIdMap,
                digraph, connectionIndexMap);

            #endif

            this.MetaNeatGenome = metaNeatGenome;
            this.Id = id;
            this.BirthGeneration = birthGeneration;
            this.ConnectionGenes = connGenes;
            this.HiddenNodeIdArray = hiddenNodeIdArr;
            this.NodeIndexByIdMap = nodeIndexByIdMap;
            this.DirectedGraph = digraph;
            this.ConnectionIndexMap = connectionIndexMap;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tests if the genome contains a connection that refers to the given hidden node ID.
        /// </summary>
        public bool ContainsHiddenNode(int id)
        {
            return Array.BinarySearch(this.HiddenNodeIdArray, id) >= 0;
        }

        #endregion
    }
}
