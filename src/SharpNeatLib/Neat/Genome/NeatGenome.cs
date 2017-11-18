using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        
        // TODO / ENHANCEMENT: Split connection weights into a separate array, this would allow us to re-use the directed connection ID arrays
        // when spawning a child genome with the same neural net structure, i.e. during weight mutation based asexual reproduction, which is the 
        // most common form of reproduction (90%+ of reproductions).
        // An array of connection genes. These define both the neural network structure/topology and the 
        // connection weights.
        readonly ConnectionGene<T>[] _connectionGeneArr;

        // An array of indexes into _connectionGeneArr, sorted by connection gene innovation ID.
        // This allows us to efficiently find connection genes using a binary search.
        readonly int[] _connIdxArr;

        double _fitness;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs with the provided ID, birth generation and gene lists.
        /// </summary>
        public NeatGenome(MetaNeatGenome<T> metaNeatGenome,
                          int id, int birthGeneration,
                          ConnectionGene<T>[] connectionGeneArr)
            : this(metaNeatGenome, id, birthGeneration, connectionGeneArr, ConnectionGeneUtils.CreateConnectionIndexArray(connectionGeneArr))
        {}

        /// <summary>
        /// Constructs with the provided ID, birth generation and gene lists.
        /// </summary>
        public NeatGenome(MetaNeatGenome<T> metaNeatGenome,
                          int id, int birthGeneration,
                          ConnectionGene<T>[] connectionGeneArr,
                          int[] connIdxArr)
        {
            Debug.Assert(connectionGeneArr.Length == connIdxArr.Length);
            Debug.Assert(ConnectionGeneUtils.IsSorted(connectionGeneArr));
            Debug.Assert(ConnectionGeneUtils.IsSorted(connIdxArr, connectionGeneArr));
            Debug.Assert(ConnectionGeneUtils.ValidateInnovationIds(connectionGeneArr, metaNeatGenome.InputNodeCount, metaNeatGenome.OutputNodeCount));

            _metaNeatGenome = metaNeatGenome;
            _id = id;
            _birthGeneration = birthGeneration;
            _connectionGeneArr = connectionGeneArr;
            _connIdxArr = connIdxArr;
        }

        #endregion

        #region Properties [NEAT Genome Specific]

        public MetaNeatGenome<T> MetaNeatGenome {
            get { return _metaNeatGenome; }
        }

        /// <summary>
        /// An array of connection genes. These define both the neural network structure/topology and the 
        /// connection weights.
        /// </summary>
        public ConnectionGene<T>[] ConnectionGeneArray => _connectionGeneArr;
        
        /// <summary>
        /// An array of indexes into _connectionGeneArr, sorted by connection gene innovation ID.
        /// This allows us to efficiently find connection genes using a binary search.
        /// </summary>
        public int[] ConnectionIndexArray => _connIdxArr;
        
        #endregion

        #region IGenome

        public int Id => _id;
        public int BirthGeneration => _birthGeneration;
        public double Fitness { get => _fitness; set => _fitness = value; }

        #endregion
    }
}
