using System.Diagnostics;
using SharpNeat.Core;
using SharpNeat.Network;

namespace SharpNeat.Neat.Genome
{
    public class NeatGenome<T> : IGenome
        where T : struct
    {
        #region Instance Fields

        // Genome metadata.
        readonly MetaNeatGenome<T> _metaNeatGenome;

        // A unique genome ID.
        readonly int _id;

        // TODO: Consider whether birthGeneration belongs here.
        // The generation that the genome was created in.
        readonly int _birthGeneration;
        
        // Connection genes data structure.
        // These define both the neural network structure/topology and the connection weights.
        readonly ConnectionGenes<T> _connGenes;

        // An array of indexes into _connectionGeneArr, sorted by connection gene innovation ID.
        // This allows us to efficiently find connection genes by innovation ID using a binary search.
        readonly int[] _connIdxArr;

        // An array of hidden node IDs, sorted to allow efficient lookup of an ID with a binary search.
        // Input and output node IDs are not included because these are allocated fixed IDs starting from zero
        // and are therefore always known.
        readonly int[] _hiddenNodeIdArr;

        double _fitness;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs with the provided ID, birth generation and gene lists.
        /// </summary>
        public NeatGenome(MetaNeatGenome<T> metaNeatGenome,
                          int id, int birthGeneration,
                          ConnectionGenes<T> connGenes)
            : this(metaNeatGenome, id, birthGeneration, connGenes,
                  ConnectionGenesUtils.CreateConnectionIndexArray(connGenes),
                  ConnectionGenesUtils.CreateHiddenNodeIdArray(connGenes._connArr, metaNeatGenome.InputOutputNodeCount))
        {}

        /// <summary>
        /// Constructs with the provided ID, birth generation and gene lists.
        /// </summary>
        public NeatGenome(MetaNeatGenome<T> metaNeatGenome,
                          int id, int birthGeneration,
                          ConnectionGenes<T> connGenes,
                          int[] connIdxArr,
                          int[] hiddenNodeIdArr)
        {
            Debug.Assert(connGenes.Length == connIdxArr.Length);
            Debug.Assert(DirectedConnectionUtils.IsSorted(connGenes._connArr));
            Debug.Assert(ConnectionGenesUtils.IsSorted(connIdxArr, connGenes._idArr));
            Debug.Assert(ConnectionGenesUtils.ValidateInnovationIds(connGenes, metaNeatGenome.InputNodeCount, metaNeatGenome.OutputNodeCount));
            Debug.Assert(ConnectionGenesUtils.ValidateHiddenNodeIds(hiddenNodeIdArr, connGenes._connArr, metaNeatGenome.InputOutputNodeCount));

            _metaNeatGenome = metaNeatGenome;
            _id = id;
            _birthGeneration = birthGeneration;
            _connGenes = connGenes;
            _connIdxArr = connIdxArr;
            _hiddenNodeIdArr = hiddenNodeIdArr;
        }

        #endregion

        #region Properties [NEAT Genome Specific]

        /// <summary>
        /// Genome metadata.
        /// </summary>
        public MetaNeatGenome<T> MetaNeatGenome {
            get { return _metaNeatGenome; }
        }

        /// <summary>
        /// Connection genes data structure.
        /// These define both the neural network structure/topology and the connection weights.
        /// </summary>
        public ConnectionGenes<T> ConnectionGenes => _connGenes;

        /// <summary>
        /// An array of connection gene indexes, sorted by connection gene innovation ID.
        /// This allows us to efficiently find connection genes using a binary search.
        /// </summary>
        public int[] ConnectionIndexArray => _connIdxArr;
        
        /// <summary>
        /// An array of hidden node IDs, sorted to allow efficient lookup of an ID with a binary search.
        /// Input and output node IDs are not included because these are allocated fixed IDs starting from zero
        /// and are therefore always known.
        /// </summary>
        public int[] HiddenNodeIdArray => _hiddenNodeIdArr;

        #endregion

        #region IGenome

        /// <summary>
        /// Genome ID.
        /// </summary>
        public int Id => _id;
        /// <summary>
        /// Genome birth generation.
        /// </summary>
        public int BirthGeneration => _birthGeneration;
        /// <summary>
        /// Genome fitness score.
        /// </summary>
        public double Fitness { get => _fitness; set => _fitness = value; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the index of a connection gene with the given innovation ID.
        /// </summary>
        /// <param name="id">The innovation ID to search for.</param>
        /// <returns>An array index if the item is found; otherwise, a negative number that is the bitwise complement
        /// of the index of the next element that is larger than id or, if there is no larger element, the bitwise
        /// complement of ConnectionGenes.Length.</returns>
        public int GetConnectionIndexById(int id)
        {
            return ConnectionGenesUtils.BinarySearchId(_connIdxArr, _connGenes._idArr, id);
        }

        #endregion
    }
}
