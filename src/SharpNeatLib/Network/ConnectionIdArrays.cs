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
using System.Diagnostics;

namespace SharpNeat.Network
{
    /// <summary>
    /// Represents the connections in a directed graph.
    /// </summary>
    public readonly struct ConnectionIdArrays
    {
        /// <summary>
        /// Array of connection source node IDs.
        /// </summary>
        public readonly int[] _sourceIdArr;
        /// <summary>
        /// Array of connection target node IDs.
        /// </summary>
        public readonly int[] _targetIdArr;

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="srcIdArr">Array of connection source node IDs.</param>
        /// <param name="tgtIdArr">Array of connection target node IDs.</param>
        public ConnectionIdArrays(int[] srcIdArr, int[] tgtIdArr)
        {
            Debug.Assert(srcIdArr.Length == tgtIdArr.Length);
            _sourceIdArr = srcIdArr;
            _targetIdArr = tgtIdArr;
        }

        /// <summary>
        /// Gets the number of connections.
        /// </summary>
        public int Length => _sourceIdArr.Length;
    }
}
