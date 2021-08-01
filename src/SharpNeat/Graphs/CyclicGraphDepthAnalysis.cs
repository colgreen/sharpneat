using System;
using System.Linq;
using System.Threading;
using Redzen;
using Redzen.Collections;
using Redzen.Structures;
using SharpNeat.Graphs.Acyclic;

namespace SharpNeat.Graphs
{
    /// <summary>
    /// An algorithm for calculating the depth of each node in an cyclic graph.
    ///
    /// Input nodes are defined as being at depth 0, and the depth of all other nodes is a determined as per the
    /// following scheme:
    ///
    /// Multiple separate traversals of the graph are made, one starting at each input node. Each traversal assigns
    /// a depth value to the visited nodes, and where a node is on multiple paths, the highest depth value is recorded.
    ///
    /// Once all traversal are complete, the average of all depths recorded against each node is calculated and rounded
    /// up to the nearest integer. Finally, if the scheme has resulted in empty layers (e.g. a node allocated to depth 2,
    /// but no nodes at depth 1) then the depth values are adjusted to remove the empty layer(s).
    ///
    /// The motivation for this slightly convoluted scheme is to create 'balanced' depth allocations when large cyclic
    /// loops might assign nodes very high depth values that might not be warranted, e.g. if most connections to a node
    /// would assign it a low depth, but a single cycle assigns it a high depth. Use of a mean/average depth is a
    /// compromise on the depth allocation of such a node. Median, min, or max could also be used, or indeed any aggregate
    /// function.
    ///
    /// The graph traversal algorithm uses function recursion. A number of other classes in SharpNEAT perform
    /// graph traversal by using a separate traversal stack (stored on the heap); that approach is faster but
    /// more complex, thus this class has not been converted to the faster approach because it is not directly
    /// used in the evolutionary algorithm. At time of writing this class is used only for graph visualization.
    /// </summary>
    public sealed class CyclicGraphDepthAnalysis
    {
        #region Instance Fields

        /// <summary>
        /// The directed graph being tested.
        /// </summary>
        DirectedGraph? _digraph;

        /// <summary>
        /// A bitmap in which each bit represents a node in the graph.
        /// The set bits represent the set of nodes that are ancestors of the current traversal node.
        /// </summary>
        BoolArray _ancestorNodeBitmap = new(1024);

        /// <summary>
        /// An integer array in which each element represents a node in the graph.
        ///
        /// The array elements are initialised to -1. An element that remains set to -1 after graph traversal indicates a node that
        /// was not visited by the traversal. All other values indicate that a node was visited, and the traversal depth when the node
        /// was visited. If the node is visited multiple times in a single traversal (i.e. there are multiple routes to the node from
        /// a single input node) then the maximum depth value of all the depths is used.
        /// </summary>
        int[]? _nodeDepthByIdx;

        /// <summary>
        /// A matrix of node depths.
        ///
        /// [nodeIdx][] gives a list of node depth values for a single node; one depth per traversal (based on multiple traversals being
        /// performed, one starting from each input node).
        ///
        /// The final depth allocated to a node is some aggregate function over all node depths assigned to it, e.g. mean, median, min,
        /// max, etc.
        /// </summary>
        LightweightList<int>[] _nodeDepthMatrix = CreateNodeDepthMatrix(64, 8);

        #if DEBUG
        /// <summary>
        /// Indicates if a call to IsCyclic() is currently in progress.
        /// For checking for attempts to re-enter that method while a call is in progress.
        /// </summary>
        int _reentranceFlag = 0;
        #endif

        #endregion

        #region Public Methods

        /// <summary>
        /// Calculate node depths in a cyclic network.
        /// </summary>
        /// <param name="digraph">The directed graph.</param>
        /// <returns>A new instance of <see cref="GraphDepthInfo"/>.</returns>
        public GraphDepthInfo CalculateNodeDepths(DirectedGraph digraph)
        {
            // TODO/FIXME: Consider storing reusable state in a pool, to allow reentrancy from threads. See ArrayPool<T> as a baseline for how to implement, or Redzen.Random.DefaultRandomSeedSource.

            #if DEBUG
            // Check for attempts to re-enter this method.
            if(Interlocked.CompareExchange(ref _reentranceFlag, 1, 0) == 1) {
                throw new InvalidOperationException("Attempt to re-enter non-reentrant method.");
            }
            #endif

            _digraph = digraph;
            EnsureNodeCapacity(digraph.TotalNodeCount, digraph.InputCount);
            _nodeDepthByIdx = new int[digraph.TotalNodeCount];

            try
            {
                // Loop over input nodes. Take each one in turn as a traversal root node.
                int inputCount = _digraph.InputCount;
                for(int nodeIdx=0; nodeIdx < inputCount; nodeIdx++)
                {
                    // Mark all nodes as not yet visited (within the current traversal).
                    Array.Fill(_nodeDepthByIdx, -1);

                    // Traverse into the current input node, with the depth we wish to assign to it.
                    TraverseNode(nodeIdx, 0);

                    // Record the node depths assigned by the current traversal.
                    RecordTraversalNodeDepths();
                }

                // Determine the final depth for each node by applying an aggregate function to each node's recorded depth values.
                DetermineFinalNodeDepths();

                // Return depth analysis info.
                return new GraphDepthInfo(_nodeDepthByIdx.Max() + 1, _nodeDepthByIdx!);
            }
            finally
            {
                Cleanup();
            }
        }

        #endregion

        #region Private Methods

        private void EnsureNodeCapacity(int requiredCapacity, int inputCount)
        {
            // Grow _ancestorNodeBitmap as necessary.
            if(requiredCapacity > _ancestorNodeBitmap.Length)
            {
                // For the new capacity, select the lowest power of two that is above the required capacity;
                // this limits the number of capacity increases as the required capacity increases.
                int newCapacity = MathUtils.CeilingToPowerOfTwo(requiredCapacity);

                // Allocate new bitmaps with the new capacity.
                _ancestorNodeBitmap = new BoolArray(newCapacity);
            }

            // Grow _nodeDepthMatrix as necessary.
            if(requiredCapacity > _nodeDepthMatrix.Length)
            {
                int newLength = MathUtils.CeilingToPowerOfTwo(requiredCapacity);
                int prevLength = _nodeDepthMatrix.Length;
                Array.Resize(ref _nodeDepthMatrix, newLength);

                for(int i=prevLength; i < newLength; i++) {
                    _nodeDepthMatrix[i] = new LightweightList<int>(inputCount);
                }
            }
        }

        private void TraverseNode(int nodeIdx, int depth)
        {
            // Is the node on the current stack of traversal ancestor nodes?
            if(_ancestorNodeBitmap[nodeIdx])
            {   // Connectivity cycle detected; don't traverse into this pathway.
                return;
            }

            // Have we already traversed this node? And if so, was the depth assigned to it greater than the current traversal depth?
            // If so we can skip traversal into this node, as we could not assign it, or any of its descendants, a greater depth than
            // it already has.
            if(_nodeDepthByIdx![nodeIdx] >= depth) {
                return;
            }

            // Register the node as having been visited, and record the traversal depth.
            _nodeDepthByIdx![nodeIdx] = depth;

            // Traverse into the node's targets / children (if it has any).
            int connIdx = _digraph!.GetFirstConnectionIndex(nodeIdx);
            if(connIdx == -1)
            {   // No target nodes to traverse.
                return;
            }

            // Add node to the set of traversal path nodes.
            _ancestorNodeBitmap[nodeIdx] = true;

            // Traverse into target nodes.
            ReadOnlySpan<int> srcIds = _digraph.ConnectionIdArrays.GetSourceIdSpan();

            for(; connIdx < srcIds.Length && srcIds[connIdx] == nodeIdx; connIdx++)
            {
                TraverseNode(_digraph.GetTargetNodeIdx(connIdx), depth + 1);
            }

            // Remove node from set of traversal path nodes.
            _ancestorNodeBitmap[nodeIdx] = false;
        }

        private void RecordTraversalNodeDepths()
        {
            for(int i=0; i < _nodeDepthByIdx!.Length; i++)
            {
                if(_nodeDepthByIdx[i] != -1) {
                    _nodeDepthMatrix[i].Add(_nodeDepthByIdx[i]);
                }
            }
        }

        private void DetermineFinalNodeDepths()
        {
            // Assign a node depth of zero as a default. Thus, any unconnected nodes will be assigned to depth zero, including output nodes.
            Array.Clear(_nodeDepthByIdx!, 0, _nodeDepthByIdx!.Length);

            // Determine a depth for all nodes with one or more recorded depths, by applying an aggregate function over those depths.
            for(int i=0; i < _nodeDepthByIdx!.Length; i++)
            {
                if(_nodeDepthMatrix[i].Count != 0)
                {
                    // Take the average node depth, and round up to the nearest integer.
                    // Other aggregate schemes are possible, this is merely one I thought might work OK.
                    _nodeDepthByIdx[i] = (int)MathF.Ceiling((MathSpan.Sum(_nodeDepthMatrix[i].AsSpan()) / (float)_nodeDepthMatrix[i].Count));
                }
            }

            // The scheme for assigning node depths in a cyclic graph may result in some layers being empty, e.g. a node assigned to
            // layer 2, even though there are none in layer 1. As such, as a final step we adjust node depths to eliminate any empty
            // layers.

            // Calc how many nodes there are in each layer.
            Span<int> nodeCountByLayer = stackalloc int[_nodeDepthByIdx.Max() + 1];
            foreach(int depth in _nodeDepthByIdx) {
                nodeCountByLayer[depth]++;
            }

            // Create a mapping from old to new layer indexes, and init with the identity mapping.
            Span<int> layerIdxMap = stackalloc int[nodeCountByLayer.Length];
            for(int i=0; i < layerIdxMap.Length; i++) {
                layerIdxMap[i] = i;
            }

            // Loop through nodeCountByLayer backwards, testing for empty layers.
            int layerCount = nodeCountByLayer.Length;

            for(int layerIdx = nodeCountByLayer.Length-1; layerIdx > -1; layerIdx--)
            {
                if(nodeCountByLayer[layerIdx] == 0)
                {
                    // Empty layer detected. Decrement all higher layer indexes to fill the gap.
                    for(int i=layerIdx+1; i < layerIdxMap.Length; i++) {
                        layerIdxMap[i]--;
                    }

                    // Set the empty layer's layer index to -1, primarily to mark it as not a valid ID (although we don't actually use this
                    // anywhere, except maybe for debugging purposes).
                    layerIdxMap[layerIdx] = -1;

                    // Update/track the number of layers with nodes.
                    layerCount--;
                }
            }

            // Apply the node layer index mappings we have just constructed.
            for(int i=0; i < _nodeDepthByIdx.Length; i++) {
                _nodeDepthByIdx[i] = layerIdxMap[_nodeDepthByIdx[i]];
            }
        }

        private void Cleanup()
        {
            _digraph = null;
            _ancestorNodeBitmap.Reset(false);

            foreach(var list in _nodeDepthMatrix) {
                list.Clear();
            }

            #if DEBUG
            // Reset reentrancy test flag.
            Interlocked.Exchange(ref _reentranceFlag, 0);
            #endif
        }

        #endregion

        #region Private Static Methods

        private static LightweightList<int>[] CreateNodeDepthMatrix(int nodeCount, int inputCount)
        {
            var matrix = new LightweightList<int>[nodeCount];
            for(int i=0; i < nodeCount; i++) {
                matrix[i] = new LightweightList<int>(inputCount);
            }
            return matrix;
        }

        #endregion
    }
}
