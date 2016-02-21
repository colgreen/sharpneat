/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;

namespace SharpNeat.Decoders.HyperNeat
{
    /// <summary>
    /// Defines a mapping between two node sets based on mapping all source nodes to all target nodes, but with an option to 
    /// omit mappings where the distance between source and target node is over some threshold. 
    /// 
    /// In addition the same nodeset can be passed to the GenerateConnections() method as both source and targt. This allows 
    /// for creating connections between nodes within a layer. The optional max distance still applies and an additional boolean
    /// option indicates if the local recurrent conenction for each node (from its output back to its input) should be generated.
    /// </summary>
    public class DefaultNodeSetMappingFunction : INodeSetMappingFunction
    {
        /// <summary>
        /// Maximum distance between connected nodes.
        /// If the distance between two nodes is less than this value then a mapping/connection is generated.
        /// </summary>
        double? _maximumConnectionDistance;

        /// <summary>
        /// _maximumConnectionDistance value squared. This allows us to test connection lengths without having to take the 
        /// square root, which is much faster.
        /// </summary>
        double? _maximumConnectionDistanceSquared;

        /// <summary>
        /// If set this fag indicates that connections from a node's output back to its input are generated if the same
        /// nodeset is passed into the GenerateConnections() method as both the source and target nodeset.
        /// </summary>
        bool _allowLocalRecurrentConnections;

        /// <summary>
        /// Delegate for testing whether a mapping/connection should be generated between the specified nodes.
        /// </summary>
        TestNodePair _testNodePair;

        /// <summary>
        /// Definition of a delegate for testing whether a mapping/connection should be generated between the specified nodes.
        /// </summary>
        delegate bool TestNodePair(SubstrateNode srcNode, SubstrateNode tgtNode);
        
        #region Constructors

        /// <summary>
        /// Construct with the specified maximum connection distance (optional/nullable) and flag indicating if local recurrent 
        /// connections should be generated when generating connections within a single node set (same source and target nodeset).
        /// </summary>
        public DefaultNodeSetMappingFunction(double? maximumConnectionDistance,  bool allowLocalRecurrentConnections)
        {
            _maximumConnectionDistance = maximumConnectionDistance;
            if(null != maximumConnectionDistance) {
                _maximumConnectionDistanceSquared = maximumConnectionDistance * maximumConnectionDistance;
            }
            _allowLocalRecurrentConnections = allowLocalRecurrentConnections;

            // Determine node test delegate to use. Using a delegate avoids having to do this test for each possible connection.
            if(null == maximumConnectionDistance) {
                _testNodePair = TestNodePair_NullTest;
            } else {
                _testNodePair = TestNodePair_MaxDistance;
            }
        }

        #endregion

        #region INodeSetMappingFunction

        /// <summary>
        /// Returns an IEnumerable that yields the mappings/connections defined by the mapping function (from the source nodes to
        /// the target nodes) as a sequence. The alternative of returning a list would require a very long list in extreme scenarios; 
        /// this approach minimizes down memory usage.
        /// </summary>
        public IEnumerable<SubstrateConnection> GenerateConnections(SubstrateNodeSet srcNodeSet, SubstrateNodeSet tgtNodeSet)
        {
            // Test for scenario where srcNodeSet and tgtNodeSet are the same node set, that is, we are creating 
            // connections within a nodeset and therefore we need to test (and honour) _allowLocalRecurrentConnections.
            if(srcNodeSet == tgtNodeSet)
            {
                // Mapping between nodes within a single node set. 

                // Break the inner loop into two. This avoids having to make the local recurrent test connection
                // for every node pair - we only test when we actually have the same node as source and target.
                IList<SubstrateNode> nodeList = srcNodeSet.NodeList;
                int count = nodeList.Count;
                for(int i=0; i<count; i++)
                {
                    // Loop over all nodes prior to the i'th.
                    SubstrateNode srcNode = nodeList[i];
                    for(int j=0; j<i; j++)
                    {
                        if(_testNodePair(srcNode, nodeList[j])) {
                            yield return new SubstrateConnection(srcNode, nodeList[j]);
                        }
                    }

                    //  Test for local recurrent connection.
                    if(_allowLocalRecurrentConnections && _testNodePair(srcNode, srcNode)) {
                        yield return new SubstrateConnection(srcNode, srcNode);
                    }

                    // Loop over all nodes after the i'th.
                    for(int j=i+1; j<count; j++)
                    {
                        if(_testNodePair(srcNode, nodeList[j])) {
                            yield return new SubstrateConnection(srcNode, nodeList[j]);
                        }
                    }
                }
            }
            else
            {   
                // Mapping between nodes in two distinct node sets.
                foreach(SubstrateNode srcNode in srcNodeSet.NodeList)
                {
                    foreach(SubstrateNode tgtNode in tgtNodeSet.NodeList)
                    {
                        if(_testNodePair(srcNode, tgtNode)) {
                            yield return new SubstrateConnection(srcNode, tgtNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns an estimate/hint for the number of connections that would be created between the provided source and target node sets.
        /// </summary>
        public int GetConnectionCountHint(SubstrateNodeSet srcNodeSet, SubstrateNodeSet tgtNodeSet)
        {
            if(null == _maximumConnectionDistance) {
                return srcNodeSet.NodeList.Count * tgtNodeSet.NodeList.Count;
            }
            
            int count = 0;
            foreach(SubstrateConnection conn in GenerateConnections(srcNodeSet, tgtNodeSet)) {
                count++;
            }
            return count;
        }

        #endregion

        #region Private Methods

        private bool TestNodePair_NullTest(SubstrateNode srcNode,SubstrateNode tgtNode)
        {
            return true;
        }

        private bool TestNodePair_MaxDistance(SubstrateNode srcNode,SubstrateNode tgtNode)
        {
            return CalcDistanceSquared(srcNode._position, tgtNode._position) < _maximumConnectionDistanceSquared;
        }

        /// <summary>
        /// Calulcate the Euclidean distance between two points in n-dimensions.
        /// </summary>
        private double CalcDistanceSquared(double[] srcPos, double[] tgtPos)
        {
            double acc = 0.0;
            for(int i=0; i<srcPos.Length; i++)
            {
                double delta = (srcPos[i]-tgtPos[i]);
                acc += delta*delta;
            }
            // We return the square of the distance, hence no need to take the square root here;
            // this improves performance.
            return acc;
        }

        #endregion
    }
}
