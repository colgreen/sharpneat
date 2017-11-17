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

        /// <summary>
        /// Genome metadata.
        /// </summary>
        readonly MetaNeatGenome<T> _metaNeatGenome;

        readonly int _id;
        // TODO: Consider whether birthGeneration belongs here.
        readonly int _birthGeneration;
        readonly ConnectionGene<T>[] _connectionGeneArr;

        double _fitness;

        //NetworkConnectivityInfo _connectivityInfo;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs with the provided ID, birth generation and gene lists.
        /// </summary>
        public NeatGenome(MetaNeatGenome<T> metaNeatGenome,
                          int id, int birthGeneration,
                          ConnectionGene<T>[] connectionGeneArr)
        {
            Debug.Assert(ConnectionGeneUtils.IsSorted<T>(connectionGeneArr));

            _metaNeatGenome = metaNeatGenome;
            _id = id;
            _birthGeneration = birthGeneration;
            _connectionGeneArr = connectionGeneArr;
        }

        #endregion

        #region Properties [NEAT Genome Specific]

        public MetaNeatGenome<T> MetaNeatGenome {
            get { return _metaNeatGenome; }
        }

        /// <summary>
        /// Gets the genome's list of connection genes.
        /// </summary>
        public ConnectionGene<T>[] ConnectionGeneArray {
            get { return _connectionGeneArr; }
        }

        ///// <summary>
        ///// Connectivity info related to the current genome.
        ///// Built in a just-in-time manner, and cached for re-use.
        ///// </summary>
        //public NetworkConnectivityInfo ConnectivityInfo 
        //{ 
        //    get
        //    {
        //        if(null == _connectivityInfo) {
        //            _connectivityInfo = new NetworkConnectivityInfo(_metaNeatGenome.InputNodeCount, _metaNeatGenome.OutputNodeCount, _connectionGeneList);
        //        }
        //        return _connectivityInfo;
        //    }
        //}

        #endregion

        #region IGenome

        public int Id => _id;
        public int BirthGeneration => _birthGeneration;
        public double Fitness { get => _fitness; set => _fitness = value; }

        #endregion

        //#region Private Methods

        //private HashSet<uint> BuildNodeIdSet()
        //{
        //    // Loop connection genes and build a set of all observed node IDs.
        //    HashSet<uint> nodeIdSet = new HashSet<uint>();
        //    foreach(ConnectionGene cGene in _connectionGeneList)
        //    {
        //        nodeIdSet.Add(cGene.SourceNodeId);
        //        nodeIdSet.Add(cGene.TargetNodeId);
        //    }
        //    return nodeIdSet;
        //}

        //#endregion
    }
}
