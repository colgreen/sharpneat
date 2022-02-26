using System;
using System.Diagnostics;
using System.Linq;
using SharpNeat.Graphs.Acyclic;

namespace SharpNeat.Graphs.Tests;

/// <summary>
/// An alternative implementation of AcyclicGraphDepthAnalysis that uses function recursion.
///
/// This version is used for testing only. The main implementation uses a number of optimizations, one of
/// which is to use it own graph traversal stack instead of using function recursion. However, those
/// optimizations make the code harder to read/understand, and therefore that code has a higher
/// chance of containing defects.
///
/// As part of the unit testing of AcyclicGraphDepthAnalysis we use this implementation in parallel with
/// the optimized version, and check that both versions give the same results for any given graph.
/// </summary>
///
public class AcyclicGraphDepthAnalysisByRecursion
{
    #region Instance Fields

    /// <summary>
    /// The directed graph being analysed.
    /// </summary>
    readonly DirectedGraph _digraph;

    /// <summary>
    /// Working array of node depths.
    /// </summary>
    readonly int[] _nodeDepthByIdx;

#if DEBUG
    readonly CyclicGraphCheck _cyclicGraphCheck = new();
#endif

    #endregion

    #region Constructor

    /// <summary>
    /// Private constructor. Prevents construction from outside of this class.
    /// </summary>
    private AcyclicGraphDepthAnalysisByRecursion(DirectedGraph digraph)
    {
        _digraph = digraph;
        _nodeDepthByIdx = new int[digraph.TotalNodeCount];
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Calculate node depths in an acyclic network.
    /// </summary>
    private GraphDepthInfo CalculateNodeDepthsInner()
    {
#if DEBUG
        // Debug assert the graph is acyclic.
        // Note. In a release build this test is not performed because we expect this method to be called from
        // code handling acyclic graphs only. If digraph is cyclic then the graph traversal implemented here will
        // cause a stack overflow, so at the very least there isn't a silent error.
        Debug.Assert(!_cyclicGraphCheck.IsCyclic(_digraph));
#endif

        // Loop over all connections exiting from input nodes, and perform a depth first traversal of each in turn.
        int inputCount = _digraph.InputCount;
        ReadOnlySpan<int> srcIds = _digraph.ConnectionIds.GetSourceIdSpan();
        ReadOnlySpan<int> tgtIds = _digraph.ConnectionIds.GetTargetIdSpan();

        for(int connIdx=0; connIdx < srcIds.Length && srcIds[connIdx] < inputCount; connIdx++)
        {
            // Traverse into the target node.
            TraverseNode(tgtIds[connIdx], 1);
        }

        // Determine the maximum depth of the graph.
        int maxDepth = (0 == _nodeDepthByIdx.Length) ? 0 : _nodeDepthByIdx.Max();

        // Return depth analysis info.
        return new GraphDepthInfo(maxDepth+1, _nodeDepthByIdx);
    }

    #endregion

    #region Private Methods

    private void TraverseNode(int nodeIdx, int depth)
    {
        // Check if the node has been visited before.
        if(_nodeDepthByIdx[nodeIdx] >= depth)
        {   // The node already has already been visited via a path that assigned it an equal or greater depth
            // than the current path. Stop traversing this path.
            return;
        }

        // Either this is the first visit to the node *or* the node has been visited, but via a shorter path.
        // Either way we assign it the current depth value and traverse into its targets to update/set their depth.
        _nodeDepthByIdx[nodeIdx] = depth;

        // Traverse into the current node's target nodes.
        int connIdx = _digraph.GetFirstConnectionIndex(nodeIdx);
        if(-1 == connIdx)
        {   // No target nodes to traverse.
            return;
        }

        ReadOnlySpan<int> srcIds = _digraph.ConnectionIds.GetSourceIdSpan();
        for(; connIdx < srcIds.Length && srcIds[connIdx] == nodeIdx; connIdx++)
        {
            TraverseNode(_digraph.GetTargetNodeIdx(connIdx), depth + 1);
        }
    }

    #endregion

    #region Public Static Methods

    /// <summary>
    /// Calculate node depths in an acyclic network.
    /// </summary>
    public static GraphDepthInfo CalculateNodeDepths(DirectedGraph digraph)
    {
        return new AcyclicGraphDepthAnalysisByRecursion(digraph).CalculateNodeDepthsInner();
    }

    #endregion
}
