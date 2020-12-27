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

namespace SharpNeat.Graphs
{
    /// <summary>
    /// Overview
    /// --------
    /// Represents a weighted directed graph. The graph is described by an array of connections,
    /// each with a source and target node ID and a weight.
    ///
    /// The node IDs are actually node indexes, i.e. if there are N unique IDs referred to in the
    /// connection array then the indexes run from 0 to N-1. An exception to this is when representing
    /// graphs from a NeatGenome in which input and outputs nodes are given fixed IDs regardless of whether
    /// they are connected to or not, however the use of a contiguous range of node indexes starting at zero
    /// still holds in that case.
    ///
    /// Elsewhere in sharpneat (e.g. in a NeatGenome) graph node IDs are not necessarily contiguous,
    /// and thus any such graph representation must have its non-contiguous node IDs mapped to zero
    /// based node indexes to be represented by this class. Such node ID mapping is outside the scope
    /// of this class.
    ///
    /// This class can represent both cyclic or acyclic graphs, however, SharpNEAT uses it in the
    /// conversion of cyclic NeatGenomes only; a specialized class is used for acyclic graphs that
    /// gives improved runtime performance for acyclic networks.
    ///
    /// Specifics
    /// ---------
    /// The connection array is sorted by sourceID and secondary sorted by targetID. This means that all
    /// connections from a given node are located in a contiguous segment, the start of which can be efficiently
    /// located using a binary search. However to improve efficiency further an array of lookup indexes is compiled
    /// which gives the starting index of a connection span/segment for a given source node index.
    /// </summary>
    /// <typeparam name="T">Connection weight data type.</typeparam>
    public class WeightedDirectedGraph<T> : DirectedGraph
        where T : struct
    {
        /// <summary>
        /// Connection weight array.
        /// </summary>
        public T[] WeightArray { get; }

        #region Constructor

        internal WeightedDirectedGraph(
            int inputCount,
            int outputCount,
            int totalNodeCount,
            in ConnectionIdArrays connIdArrays,
            T[] weightArr)
        : base(inputCount, outputCount, totalNodeCount, connIdArrays)
        {
            this.WeightArray = weightArr;
        }

        #endregion
    }
}
