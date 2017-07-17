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
    public class NeatGenome : IGenome
    {
        #region Instance Fields

        /// <summary>
        /// Genome metadata.
        /// </summary>
        readonly MetaNeatGenome _metaNeatGenome;

        readonly uint _id;
        // TODO: Consider whether birthGeneration belongs here.
        readonly uint _birthGeneration;
        // TODO: Order genes by sourceID then targetID, and use a separate structure to track the order of innovation IDs.
        readonly ConnectionGeneList _connectionGeneList;

        double _fitness;

        //NetworkConnectivityInfo _connectivityInfo;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs with the provided ID, birth generation and gene lists.
        /// </summary>
        public NeatGenome(MetaNeatGenome metaNeatGenome,
                          uint id, uint birthGeneration,
                          ConnectionGeneList connectionGeneList)
        {
            _metaNeatGenome = metaNeatGenome;
            _id = id;
            _birthGeneration = birthGeneration;
            _connectionGeneList = connectionGeneList;
        }

        #endregion

        #region Properties [NEAT Genome Specific]

        public MetaNeatGenome MetaNeatGenome {
            get { return _metaNeatGenome; }
        }

        /// <summary>
        /// Gets the genome's list of connection genes.
        /// </summary>
        public ConnectionGeneList ConnectionGeneList {
            get { return _connectionGeneList; }
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

        public uint Id => _id;
        public uint BirthGeneration => _birthGeneration;
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
