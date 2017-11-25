using System.Diagnostics;
using Redzen;
using Redzen.Sorting;
using SharpNeat.Network;

namespace SharpNeat.Neat.Genome
{
    /// <summary>
    /// Connections genes represented as a structure of arrays (see https://en.wikipedia.org/wiki/AOS_and_SOA).
    /// Element i of each array represents a value relating to the i'th gene.
    /// </summary>
    /// <typeparam name="T">Connection weight type.</typeparam>
    public class ConnectionGenes<T>
        where T : struct
    {        
        /// <summary>
        /// Array of directed connections; this described the network structure.
        /// </summary>
        public readonly DirectedConnection[] _connArr;
        /// <summary>
        /// Array of connection weights.
        /// </summary>
        public readonly T[] _weightArr;
        /// <summary>
        /// Array of connection innovation IDs.
        /// </summary>
        public readonly int[] _idArr;

        #region Constructors

        /// <summary>
        /// Construct uninitialised arrays with the given length.
        /// </summary>
        /// <param name="length">Number of connection genes to allocate space for.</param>
        public ConnectionGenes(int length)
        {
            _connArr = new DirectedConnection[length];
            _weightArr = new T[length];
            _idArr = new int[length];
        }

        /// <summary>
        /// Construct with the pre-built arrays.
        /// </summary>
        /// <param name="connArr">Array of directed connections; this described the network structure.</param>
        /// <param name="weightArr">Array of connection weights.</param>
        /// <param name="idArr">Array of connection innovation IDs.</param>
        public ConnectionGenes(
            DirectedConnection[] connArr,
            T[] weightArr,
            int[] idArr)
        {
            Debug.Assert(null != connArr);
            Debug.Assert(null != weightArr);
            Debug.Assert(null != idArr);
            Debug.Assert(connArr.Length == weightArr.Length);
            Debug.Assert(connArr.Length == idArr.Length);

            _connArr = connArr;
            _weightArr = weightArr;
            _idArr = idArr;
        }

        #endregion

        #region Properties / Indexers

        /// <summary>
        /// Gets the number of connection genes.
        /// </summary>
        public int Length => _connArr.Length;

        /// <summary>
        /// Connection gene indexer.
        /// </summary>
        /// <param name="idx">Index of the gene to get or set.</param>
        public (int srcIdx, int tgtIdx, T weight, int id) this[int idx]
        {
            get
            {
                return (_connArr[idx].SourceId, _connArr[idx].TargetId, _weightArr[idx], _idArr[idx]);
            }
            set
            {
                _connArr[idx] = new DirectedConnection(value.srcIdx, value.tgtIdx);
                _weightArr[idx] = value.weight;
                _idArr[idx] = value.id;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sort the connection genes by sourceID then targetID.
        /// </summary>
        public void Sort()
        {
            IntroSort<DirectedConnection,T,int>.Sort(_connArr, _weightArr, _idArr);
        }

        #endregion
    }
}
