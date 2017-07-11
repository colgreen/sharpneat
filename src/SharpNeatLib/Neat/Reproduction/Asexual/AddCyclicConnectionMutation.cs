using Redzen.Numerics;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;
using SharpNeat.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SharpNeat.Neat.Reproduction.Asexual
{

    internal class AddCyclicConnectionMutation
    {
        NeatPopulation _pop;
        IRandomSource _rng;

        #region Constructor

        public AddCyclicConnectionMutation(NeatPopulation pop, IRandomSource rng)
        {
            _pop = pop;
            _rng = rng;
        }

        #endregion

        #region Public Methods

        public NeatGenome CreateChild(NeatGenome parent)
        { 
            // Notes.
            // Two nodes are selected at random, a source node and a target node. If no connection exists from the source to 
            // the target then a new connection is made between those two nodes.
            var parentConnectivityInfo = parent.ConnectivityInfo;
            ConnectionEndpoints conn;
            if(!TryGetConnection(parentConnectivityInfo, out conn))
            {   // Failed to find a connection that could be added the parent genome.
                return null;
            }

            // If a connection between these two node IDs has been made previously then re-use the innovation ID.
            uint connectionId;
            if(!_pop.AddedConnectionBuffer.TryGetValue(conn, out connectionId))
            {   // No previous ID found; get a new ID.
                connectionId = _pop.InnovationIdSeq.Next();
            }

            // Get a random connection weight.
            double weight = RandomUtils.SampleConnectionWeight(_pop.MetaNeatGenome.ConnectionWeightRange, _rng);

            // Create a new connection gene.
            var cGene = new ConnectionGene(connectionId, conn, weight);





            //var parentGeneList = parent.ConnectionGeneList;
            //if(connectionId > parentGeneList.LastInnovationId)
            //{

            //}

            


















            
            return null;
        }

        #endregion

        #region Private Methods

        private bool TryGetConnection(NetworkConnectivityInfo parentConnectivityInfo, out ConnectionEndpoints conn)
        {
            int nodeCount = parentConnectivityInfo.NodeCount;
            int hiddenNodeCount = parentConnectivityInfo.HiddenNodeCount;

            var metaNeatGenome = _pop.MetaNeatGenome;
            int inputCount = metaNeatGenome.InputNodeCount;
            int outputHiddenCount = metaNeatGenome.OutputNodeCount + hiddenNodeCount;

            // TODO: Consider possible ways this might be improved upon.
            // Make a fixed number of attempts, and give up if none succeed.
            for(int attempts=0; attempts<5; attempts++)
            {
                // Select candidate source and target nodes.
                // Source node can by any node. Target node is any node except input nodes.
                int srcNodeIdx = _rng.Next(nodeCount);
                int tgtNodeIdx = inputCount + _rng.Next(outputHiddenCount);

                // Get/lookup node IDs.
                uint srcNodeId = parentConnectivityInfo.GetNodeId(srcNodeIdx);
                uint tgtNodeId = parentConnectivityInfo.GetNodeId(tgtNodeIdx);

                // Test if the proposed connection already exists.
                if(!parentConnectivityInfo.ContainsConnection(srcNodeId, tgtNodeId))
                {
                    conn = new ConnectionEndpoints(srcNodeId, tgtNodeId);
                    return true;
                }
            }

            // No valid connection to create was found. 
            // Indicate failure.
            conn = default(ConnectionEndpoints);
            return false;
        }

        #endregion
    }
}
