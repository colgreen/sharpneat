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

        public NeatPopulation(Uint32Sequence genomeIdSeq, List<NeatGenome> genomeList, MetaNeatGenome metaNeatGenome)
            : this(genomeIdSeq, genomeList, metaNeatGenome, __defaultInnovationHistoryBufferSize)
        {}

        public NeatPopulation(Uint32Sequence genomeIdSeq, List<NeatGenome> genomeList, MetaNeatGenome metaNeatGenome, int innovationHistoryBufferSize)
            : base(genomeIdSeq, genomeList)
        {
            this.MetaNeatGenome = metaNeatGenome;
            this.AddedConnectionBuffer = new KeyedCircularBuffer<ConnectionEndpointsInfo,uint>(innovationHistoryBufferSize);
            this.AddedNodeBuffer = new KeyedCircularBuffer<uint,AddedNodeInfo>(innovationHistoryBufferSize);
        }

        #endregion

        #region Properties

        public MetaNeatGenome MetaNeatGenome { get; private set; }

        /// <summary>
        /// A history buffer of added connections. 
        /// Used when adding new connections to check if an identical connection has been added to a genome elsewhere 
        /// in the population. This allows re-use of the same innovation ID for like connections.
        /// </summary>
        public KeyedCircularBuffer<ConnectionEndpointsInfo,uint> AddedConnectionBuffer { get; set; }

        /// <summary>
        /// A history buffer of added neurons.
        /// Used when adding new neurons to check if an identical neuron has been added to a genome elsewhere in the 
        /// population. This allows re-use of the same innovation ID for like neurons.
        /// </summary>
        public KeyedCircularBuffer<uint,AddedNodeInfo> AddedNodeBuffer { get; set; }

        #endregion
    }
}
