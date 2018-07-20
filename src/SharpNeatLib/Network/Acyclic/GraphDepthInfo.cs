using System;
using System.Diagnostics;
using System.Linq;

namespace SharpNeat.Network.Acyclic
{
    /// <summary>
    /// Conveys summary information from a network depth analysis.
    /// </summary>
    public class GraphDepthInfo : IEquatable<GraphDepthInfo>
    {
        /// <summary>
        /// Indicates the total depth of the network.
        /// This is the highest value within _nodeDepths + 1 (because the first layer is layer 0)
        /// </summary>
        public readonly int _networkDepth;
        /// <summary>
        /// An array containing the depth of each node in the digraph.
        /// </summary>
        public readonly int[] _nodeDepthArr;

        /// <summary>
        /// Construct with the provided info.
        /// </summary>
        public GraphDepthInfo(int networkDepth, int[] nodeDepthArr)
        {
            Debug.Assert(networkDepth >= 0);
            Debug.Assert(null != nodeDepthArr);

            _networkDepth = networkDepth;
            _nodeDepthArr = nodeDepthArr;
        }

        #region IEquatable

        // TODO: Check why this is here. Should we also override Equals(object), operator==, GetHashCode(), etc.?
        public bool Equals(GraphDepthInfo other)
        {
            return _networkDepth == other._networkDepth
                && _nodeDepthArr.SequenceEqual(other._nodeDepthArr);
        }

        #endregion
    }
}
