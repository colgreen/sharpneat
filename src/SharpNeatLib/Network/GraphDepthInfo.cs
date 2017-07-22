
namespace SharpNeat.Network.Analysis
{
    /// <summary>
    /// Conveys summary information from a network depth analysis .
    /// </summary>
    public struct GraphDepthInfo
    {
        /// <summary>
        /// Indicates the total depth of the network.
        /// This is the highest value within _nodeDepths + 1 (because the first layer is layer 0)
        /// </summary>
        public readonly int _networkDepth;
        /// <summary>
        /// An array containing the depth of each node in the network 
        /// (indexed by position within the analysed INodeList).
        /// </summary>
        public readonly int[] _nodeDepthArr;

        /// <summary>
        /// Construct with the provided info.
        /// </summary>
        public GraphDepthInfo(int networkDepth, int[] nodeDepthArr)
        {
            _networkDepth = networkDepth;
            _nodeDepthArr = nodeDepthArr;
        }
    }
}
