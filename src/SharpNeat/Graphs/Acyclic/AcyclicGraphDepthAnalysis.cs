/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Linq;
using System.Threading;
using Redzen.Collections;

namespace SharpNeat.Graphs.Acyclic
{
    /// <summary>
    /// An algorithm for calculating the depth of each node in an acyclic graph.
    ///
    /// Input nodes are defined as being at depth 0, the depth of all other nodes is defined as the maximum number
    /// of hops to each node from an input node. I.e. where multiple paths exist to a node (potentially each with
    /// a different numbers of hops), the node's depth is defined by the path with the most number of hops.
    /// </summary>
    /// <remarks>
    /// The algorithm utilises a depth first traversal of the graph, but using its own traversal stack data
    /// structure instead of relying on function recursion and the call stack. This is an optimisation, for more
    /// details see the comments on: <see cref="Neat.Reproduction.Sexual.Strategy.UniformCrossover.CyclicConnectionCheck"/>.
    /// Also see:
    /// <see cref="CyclicConnectionCheck"/>
    /// <see cref="CyclicGraphCheck"/>.
    /// </remarks>
    public sealed class AcyclicGraphDepthAnalysis
    {
        #region Instance Fields

        /// <summary>
        /// The graph traversal stack, as required by a depth first graph traversal algorithm.
        /// Each stack entry is an index into a connection list, representing both the current node being traversed
        /// (the connections's source ID), and the current position in that node's outgoing connections.
        /// </summary>
        readonly LightweightStack<StackFrame> _traversalStack = new(16);

        /// <summary>
        /// Working array of node depths.
        /// </summary>
        int[]? _nodeDepthByIdx;

        readonly CyclicGraphCheck? _cyclicGraphCheck;

        #if DEBUG
        /// <summary>
        /// Indicates if a call to IsConnectionCyclic() is currently in progress.
        /// For checking for attempts to re-enter that method while a call is in progress.
        /// </summary>
        int _reentranceFlag = 0;
        #endif

        #endregion

        #region Construction

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AcyclicGraphDepthAnalysis()
        {}

        /// <summary>
        /// Construct with a <paramref name="validateAcyclic"/> flag.
        /// </summary>
        /// <param name="validateAcyclic">If true then each call to <see cref="CalculateNodeDepths"/> will test if
        /// the graph is cyclic before calculating the acyclic node depths. This is computationally expensive, and
        /// as such is intended to be used in debugging and testing scenarios only.</param>
        /// <remarks>
        /// If the caller can guarantee that calls to CalculateNodeDepths() will provide acyclic graphs only, then
        /// <paramref name="validateAcyclic"/> can be set to false to avoid the cost of the cyclic graph check (which is relatively expensive to perform).
        /// </remarks>
        public AcyclicGraphDepthAnalysis(bool validateAcyclic)
        {
            if(validateAcyclic) {
                _cyclicGraphCheck = new CyclicGraphCheck();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Calculate node depths in an acyclic directed graph.
        /// </summary>
        /// <param name="digraph">The directed graph.</param>
        /// <returns>A new instance of <see cref="GraphDepthInfo"/>.</returns>
        /// <remarks>
        /// If <paramref name="digraph"/> represents a cyclic graph, then this method will either
        /// (a) If validateAcyclic=true passed in at construction time, then an exception is thrown; otherwise
        /// (b) this method is non-completing, and will ultimately cause an out-of-memory exception.
        ///
        /// The cyclic test is expensive, therefore it should be avoided in normal use if possible, thus relying
        /// on the correctness of the caller, i.e., the caller should only ever call this method with acyclic
        /// graphs.
        /// </remarks>
        public GraphDepthInfo CalculateNodeDepths(DirectedGraph digraph)
        {
            #if DEBUG
            // Check for attempts to re-enter this method.
            if(Interlocked.CompareExchange(ref _reentranceFlag, 1, 0) == 1) {
                throw new InvalidOperationException("Attempt to re-enter non-reentrant method.");
            }

            #endif

            // Test that the graph is acyclic; if digraph is cyclic then the graph traversal implemented here will
            // cause _traversalStack to grow indefinitely, ultimately causing an out-of-memory exception.
            // This test is relatively expensive to compute, therefore it can be disabled by callers that can guarantee the
            // graph is acyclic.
            if(_cyclicGraphCheck is not null && _cyclicGraphCheck.IsCyclic(digraph)) {
                throw new ArgumentException("Directed graph is not acyclic.", nameof(digraph));
            }

            _nodeDepthByIdx = new int[digraph.TotalNodeCount];

            try
            {
                CalculateNodeDepthsInner(digraph);

                // Determine the maximum depth of the graph.
                int maxDepth = (_nodeDepthByIdx.Length == 0) ? 0 : _nodeDepthByIdx.Max();

                // Return depth analysis info.
                return new GraphDepthInfo(maxDepth+1, _nodeDepthByIdx);
            }
            finally
            {
                Cleanup();
            }
        }

        #endregion

        #region Private Methods

        private void CalculateNodeDepthsInner(DirectedGraph digraph)
        {
            // Push all input nodes onto the traversal stack, except input nodes with no
            // exit connections to traverse.
            int inputCount = digraph.InputCount;
            for(int nodeIdx=0; nodeIdx < inputCount; nodeIdx++)
            {
                // Lookup the first connection that exits the current input node (if any).
                int connIdx = digraph.GetFirstConnectionIndex(nodeIdx);
                if(connIdx != -1) {
                    _traversalStack.Push(new StackFrame(connIdx, 1));
                }
            }

            // Run the graph traversal algorithm.
            TraverseGraph(digraph);
        }

        /// <summary>
        /// The graph traversal algorithm.
        /// </summary>
        /// <param name="digraph">The directed acyclic graph to traverse.</param>
        private void TraverseGraph(DirectedGraph digraph)
        {
            ReadOnlySpan<int> srcIds = digraph.ConnectionIdArrays.GetSourceIdSpan();
            ReadOnlySpan<int> tgtIds = digraph.ConnectionIdArrays.GetTargetIdSpan();

            // While there are entries on the stack.
            while (_traversalStack.Count != 0)
            {
                // Get the connection index from the top of stack; this indicates next connection to be traversed.
                StackFrame currStackFrame = _traversalStack.Peek();

                // Notes.
                // Before we traverse the current connection, update the stack state to point to the next connection to be
                // traversed, either from the current node or a parent node. I.e. we modify the stack state ready for when
                // the traversal down into the current connection completes and returns back to the current node.
                //
                // This approach results in tail call optimisation and thus will result in a shallower stack on average. It
                // also has the side effect that we can no longer examine the stack to observe the traversal path at a given
                // point in time, since some of the path may no longer be on the stack.
                MoveForward(srcIds, tgtIds, currStackFrame);

                // Skip nodes that have already been visited via a path that assigned them an equal or greater
                // depth than the current path.
                int childNodeId = tgtIds[currStackFrame.ConnectionIdx];
                if(_nodeDepthByIdx![childNodeId] >= currStackFrame.Depth) {
                    continue;
                }

                // We're about to traverse into childNodeId, so mark it as visited with the current traversal depth.
                _nodeDepthByIdx[childNodeId] = currStackFrame.Depth;

                // Search for outgoing connections from childNodeId.
                int connIdx = digraph.GetFirstConnectionIndex(childNodeId);
                if (connIdx >= 0)
                {   // childNodeId has outgoing connections; push the first connection onto the stack to mark it for traversal.
                    _traversalStack.Push(new StackFrame(connIdx, currStackFrame.Depth + 1));
                }
            }

            // The stack is empty. Graph traversal completed.
        }

        /// <summary>
        /// Update the stack state to point to the next connection to traverse down.
        /// </summary>
        private void MoveForward(ReadOnlySpan<int> srcIds, ReadOnlySpan<int> tgtIds, in StackFrame currStackFrame)
        {
            // If the current node has at least one more visitable outgoing connection then update the node's entry
            // on the top of the stack to point to said connection.
            int currConnIdx = currStackFrame.ConnectionIdx;
            int depth = currStackFrame.Depth;

            for(int i=currConnIdx + 1; i < srcIds.Length && (srcIds[currConnIdx] == srcIds[i]); i++)
            {
                // Skip nodes that have already been visited via a path that assigned them an equal or greater
                // depth than the current path.
                if(_nodeDepthByIdx![tgtIds[i]] < depth)
                {
                    _traversalStack.Poke(new StackFrame(i, depth));
                    return;
                }
            }

            // No more connections for the current node; pop/remove the current node from the top of the stack.
            // Traversal will thus continue from its traversal parent node's current position, or will terminate
            // if the stack is now empty.
            _traversalStack.Pop();
        }

        private void Cleanup()
        {
            #if DEBUG
            // Reset reentrancy test flag.
            Interlocked.Exchange(ref _reentranceFlag, 0);
            #endif
        }

        #endregion

        #region Inner Struct

        readonly struct StackFrame
        {
            public readonly int ConnectionIdx;
            public readonly int Depth;

            public StackFrame(int connIdx, int depth)
            {
                this.ConnectionIdx = connIdx;
                this.Depth = depth;
            }
        }

        #endregion
    }
}
