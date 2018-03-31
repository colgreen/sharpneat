using System;
using SharpNeat.EA;
using SharpNeat.Evaluation;
using SharpNeat.Network;
using SharpNeat.Network.Acyclic;

namespace SharpNeat.Neat.Genome
{
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
        /// Mapping function for obtaining node a index for a given node ID.
        /// </summary>
        /// <remarks>
        /// Node indexes have a range of 0 to N-1 (for N nodes), i.e. the indexes are zero based and contiguous, as opposed to node IDs that are not contiguous.
        /// </remarks>
        public Func<int,int> NodeIndexByIdFn { get; }

        /// <summary>
        /// The directed graph that the current genome represents.
        /// </summary>
        /// <remarks>
        /// This digraph mirrors the graph described by <see cref="ConnectionGenes"/>; this object represents the
        /// graph structure only, not the weights, and is therefore reused when spawning genomes with the same structure 
        /// (i.e. a child that is the result of a connection weight mutation only).
        /// The DirectedGraph class provides an efficient means of working with graphs and is therefore made available
        /// on this class to provide improved performance for:
        ///  * Decoding to a neural net object.
        ///  * Finding new connections on acyclic graph, i.e. detecting if a random new connection would form a cycle.
        /// </remarks>
        public DirectedGraph DirectedGraph { get; }

        // TODO: Replace cached GraphDepthInfo with a cached IPhenome, since that is ultimately what we want the depth info for.
        // A GraphDepthInfo instance can be re-used for child genomes that have the same graph topology as their parent, i.e. child genomes
        // that are the result of weight mutation; but the IPhenome can probably be re-used in those cases too.
        /// <summary>
        /// Graph depth information. For acyclic graphs only.
        /// If present this has been cached during genome decoding, since the depth info is a structure tied to DirectedGraph
        /// not NeatGenome, in particular it's based on contiguous node IDs used by DirectedGraph and not the non-contiguous 
        /// node innovation IDs used by NeatGenome.
        /// </summary>
        public GraphDepthInfo DepthInfo { get; set; }

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
            Func<int,int> nodeIndexByIdFn,
            DirectedGraph digraph,
            GraphDepthInfo depthInfo)
        {
            this.MetaNeatGenome = metaNeatGenome;
            this.Id = id;
            this.BirthGeneration = birthGeneration;
            this.ConnectionGenes = connGenes;
            this.HiddenNodeIdArray = hiddenNodeIdArr;
            this.NodeIndexByIdFn = nodeIndexByIdFn;
            this.DirectedGraph = digraph;
            this.DepthInfo = depthInfo;
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
