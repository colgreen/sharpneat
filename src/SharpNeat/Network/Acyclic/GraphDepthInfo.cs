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
            Debug.Assert(nodeDepthArr is object);

            _networkDepth = networkDepth;
            _nodeDepthArr = nodeDepthArr;
        }

        #region IEquatable

        /// <summary>
        /// Determines whether the specified <see cref="GraphDepthInfo" /> is equal to the current <see cref="GraphDepthInfo" />.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the objects are equal; otherwise false.</returns>
        public bool Equals(GraphDepthInfo other)
        {
            return _networkDepth == other._networkDepth
                && _nodeDepthArr.SequenceEqual(other._nodeDepthArr);
        }

        #endregion
    }
}
