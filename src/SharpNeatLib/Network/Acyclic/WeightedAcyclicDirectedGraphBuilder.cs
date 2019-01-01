/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2019 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpNeat.Network.Acyclic
{
    public static class WeightedAcyclicDirectedGraphBuilder<T>
        where T : struct
    {
        #region Public Static Methods

        public static WeightedAcyclicDirectedGraph<T> Create(
            IList<WeightedDirectedConnection<T>> connectionList,
            int inputCount, int outputCount)
        {
            // Convert the set of connections to a standardised graph representation.
            WeightedDirectedGraph<T> digraph = WeightedDirectedGraphBuilder<T>.Create(connectionList, inputCount, outputCount);

            // Invoke factory logic specific to acyclic graphs.
            return Create(digraph);
        }

        public static WeightedAcyclicDirectedGraph<T> Create(
            WeightedDirectedGraph<T> digraph)
        {
            // Calc the depth of each node in the digraph.
            // ENHANCEMENT: Use a re-usable instance of AcyclicGraphDepthAnalysis.
            GraphDepthInfo depthInfo = new AcyclicGraphDepthAnalysis().CalculateNodeDepths(digraph);

            return CreateInner(digraph, depthInfo);
        }

        public static WeightedAcyclicDirectedGraph<T> Create(
            WeightedDirectedGraph<T> digraph,
            GraphDepthInfo depthInfo)
        {
            // Assert that the passed in depth info is correct.
            // Note. This test is expensive because it invokes a graph traversal algorithm to determine node depths.

            // ENHANCEMENT: Use a re-usable instance of AcyclicGraphDepthAnalysis.
            Debug.Assert(depthInfo.Equals(new AcyclicGraphDepthAnalysis().CalculateNodeDepths(digraph)));

            return CreateInner(digraph, depthInfo);
        }

        #endregion

        #region Private Static Methods [High Level]

        private static WeightedAcyclicDirectedGraph<T> CreateInner(
            WeightedDirectedGraph<T> digraph,
            GraphDepthInfo depthInfo)
        {
            // Create acyclic digraph.
            var acyclicDigraph = AcyclicDirectedGraphBuilderUtils.CreateAcyclicDirectedGraph(
                digraph,
                depthInfo,
                out int[] _,
                out int[] connectionIndexMap);

            // Copy weights into a new array and into their correct position.
            T[] genomeWeightArr = digraph.WeightArray;
            T[] weightArr = new T[genomeWeightArr.Length];

            for(int i=0; i < weightArr.Length; i++) {
                weightArr[i] = genomeWeightArr[connectionIndexMap[i]];
            }

            // Construct a new WeightedAcyclicDirectedGraph.
            return new WeightedAcyclicDirectedGraph<T>(
                acyclicDigraph.ConnectionIdArrays,
                acyclicDigraph.InputCount, 
                acyclicDigraph.OutputCount,
                acyclicDigraph.TotalNodeCount,
                acyclicDigraph.LayerArray,
                acyclicDigraph.OutputNodeIdxArr,
                weightArr);
        }

        #endregion
    }
}
