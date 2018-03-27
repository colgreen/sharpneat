using System;
using System.Diagnostics;
using SharpNeat.Core;
using SharpNeat.Evaluation;
using SharpNeat.Network;

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

        #region Auto Properties [NEAT Genome Specific]

        /// <summary>
        /// Genome metadata.
        /// </summary>
        public MetaNeatGenome<T> MetaNeatGenome { get; }

        /// <summary>
        /// Connection genes data structure.
        /// These define both the neural network structure/topology and the connection weights.
        /// </summary>
        public ConnectionGenes<T> ConnectionGenes { get; }

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

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs with the provided ID, birth generation and gene arrays.
        /// </summary>
        private NeatGenome(
            MetaNeatGenome<T> metaNeatGenome,
            int id,
            int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr,
            Func<int,int> nodeIndexByIdFn,
            GraphDepthInfo depthInfo)
        {
            this.MetaNeatGenome = metaNeatGenome;
            this.Id = id;
            this.BirthGeneration = birthGeneration;
            this.ConnectionGenes = connGenes;
            this.HiddenNodeIdArray = hiddenNodeIdArr;
            this.NodeIndexByIdFn = nodeIndexByIdFn;
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

        #region Public Static Factory Methods

        public static NeatGenome<T> Create(
            MetaNeatGenome<T> metaNeatGenome,
            int id, 
            int birthGeneration,
            ConnectionGenes<T> connGenes)
        {
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));

            // Determine the set of node IDs, and create a mapping from node IDs to node indexes.
            int[] hiddenNodeIdArr = ConnectionGenesUtils.CreateHiddenNodeIdArray(connGenes._connArr, metaNeatGenome.InputOutputNodeCount);
            Func<int,int> nodeIndexByIdFn = DirectedGraphUtils.CompileNodeIdMap(hiddenNodeIdArr, metaNeatGenome.InputOutputNodeCount);

            return new NeatGenome<T>(metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdFn, null);
        }

        public static NeatGenome<T> Create(
            MetaNeatGenome<T> metaNeatGenome,
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr)
        {
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));
            Debug.Assert(ConnectionGenesUtils.ValidateHiddenNodeIds(hiddenNodeIdArr, connGenes._connArr, metaNeatGenome.InputOutputNodeCount));

            // Create a mapping from node IDs to node indexes.
            Func<int,int> nodeIndexByIdFn = DirectedGraphUtils.CompileNodeIdMap(hiddenNodeIdArr, metaNeatGenome.InputOutputNodeCount);

            return new NeatGenome<T>(metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdFn, null);
        }

        public static NeatGenome<T> Create(
            MetaNeatGenome<T> metaNeatGenome,
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr,
            Func<int,int> nodeIndexByIdFn)
        {
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));
            Debug.Assert(ConnectionGenesUtils.ValidateHiddenNodeIds(hiddenNodeIdArr, connGenes._connArr, metaNeatGenome.InputOutputNodeCount));

            return new NeatGenome<T>(metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdFn, null);
        }

        public static NeatGenome<T> Create(
            MetaNeatGenome<T> metaNeatGenome,
            int id, int birthGeneration,
            ConnectionGenes<T> connGenes,
            int[] hiddenNodeIdArr,
            Func<int,int> nodeIndexByIdFn,
            GraphDepthInfo depthInfo)
        {
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));
            Debug.Assert(ConnectionGenesUtils.ValidateHiddenNodeIds(hiddenNodeIdArr, connGenes._connArr, metaNeatGenome.InputOutputNodeCount));

            // Notes.
            // 1) If calling this overload of Create() then we require depthInfo to be provided.
            // 2) GraphDepthInfo relates to, and is used for, acyclic graphs only.
            // 3) GraphDepthInfo is optional for acyclic graphs, but if not supplied then one of the other overloads of Create() should be used.
            Debug.Assert(null != depthInfo && metaNeatGenome.IsAcyclic);

            return new NeatGenome<T>(metaNeatGenome, id, birthGeneration, connGenes, hiddenNodeIdArr, nodeIndexByIdFn, depthInfo);
        }

        #endregion
    }
}
