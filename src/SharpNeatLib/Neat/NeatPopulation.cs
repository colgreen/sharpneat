using Redzen.Structures;
using SharpNeat.EA;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;
using SharpNeat.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Neat
{
    public class NeatPopulation : Population<NeatGenome>
    {
        const int __defaultInnovationHistoryBufferSize = 0x20000; // = 131,072.

        #region Constructor

        public NeatPopulation(UInt32Sequence genomeIdSeq, UInt32Sequence innovationIdSeq,
                              List<NeatGenome> genomeList, MetaNeatGenome metaNeatGenome)
            : this(genomeIdSeq, innovationIdSeq, genomeList, metaNeatGenome, __defaultInnovationHistoryBufferSize)
        {}

        public NeatPopulation(UInt32Sequence genomeIdSeq, UInt32Sequence innovationIdSeq,
                              List<NeatGenome> genomeList, MetaNeatGenome metaNeatGenome,
                              int innovationHistoryBufferSize)
            : base(genomeIdSeq, innovationIdSeq, genomeList)
        {
            this.MetaNeatGenome = metaNeatGenome;
            this.AddedConnectionBuffer = new KeyedCircularBuffer<ConnectionEndpoints,uint>(innovationHistoryBufferSize);
            this.AddedNodeBuffer = new KeyedCircularBuffer<uint,AddedNodeInfo>(innovationHistoryBufferSize);
        }

        #endregion

        #region Properties

        public MetaNeatGenome MetaNeatGenome { get; }

        /// <summary>
        /// A history buffer of added connections. 
        /// Used when adding new connections to check if an identical connection has been added to a genome elsewhere 
        /// in the population. This allows re-use of the same innovation ID for like connections.
        /// </summary>
        public KeyedCircularBuffer<ConnectionEndpoints,uint> AddedConnectionBuffer { get; }

        /// <summary>
        /// A history buffer of added neurons.
        /// Used when adding new neurons to check if an identical neuron has been added to a genome elsewhere in the 
        /// population. This allows re-use of the same innovation ID for like neurons.
        /// </summary>
        public KeyedCircularBuffer<uint,AddedNodeInfo> AddedNodeBuffer { get; }

        #endregion

        #region Public Methods

        public NetworkConnectivityInfo GetNetworkConnectivityInfo(NeatGenome genome)
        {
            if(null == genome.ConnectivityInfo) {
                genome.ConnectivityInfo = new NetworkConnectivityInfo(MetaNeatGenome.InputNodeCount, MetaNeatGenome.OutputNodeCount, genome.ConnectionGeneList);
            }
            return genome.ConnectivityInfo;
        }

        #endregion
    }
}
