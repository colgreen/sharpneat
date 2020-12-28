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
using System.Diagnostics;
using System.Linq;

namespace SharpNeat.Graphs.Acyclic
{
    /// <summary>
    /// Conveys summary information from a network depth analysis.
    /// </summary>
    public sealed class GraphDepthInfo : IEquatable<GraphDepthInfo>
    {
        /// <summary>
        /// The total depth of the graph.
        /// This is the highest value within _nodeDepths, + 1 (because the first layer is layer 0).
        /// </summary>
        public readonly int _graphDepth;
        /// <summary>
        /// An array containing the depth of each node in the digraph.
        /// </summary>
        public readonly int[] _nodeDepthArr;

        /// <summary>
        /// Construct with the provided info.
        /// </summary>
        /// <param name="graphDepth">The total depth of the graph.</param>
        /// <param name="nodeDepthArr">An array containing the depth of each node in the digraph.</param>
        public GraphDepthInfo(int graphDepth, int[] nodeDepthArr)
        {
            Debug.Assert(graphDepth >= 0);
            Debug.Assert(nodeDepthArr is object);

            _graphDepth = graphDepth;
            _nodeDepthArr = nodeDepthArr;
        }

        #region IEquatable

        /// <summary>
        /// Determines whether the specified <see cref="GraphDepthInfo" /> is equal to the current <see cref="GraphDepthInfo" />.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the objects are equal; otherwise false.</returns>
        public bool Equals(GraphDepthInfo? other)
        {
            return other is object
                && _graphDepth == other._graphDepth
                && _nodeDepthArr.SequenceEqual(other._nodeDepthArr);
        }

        #endregion
    }
}
