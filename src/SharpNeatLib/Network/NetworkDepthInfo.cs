/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */

namespace SharpNeat.Network
{
    /// <summary>
    /// Conveys summary information from a network depth analysis .
    /// </summary>
    public struct NetworkDepthInfo
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
        public NetworkDepthInfo(int networkDepth, int[] nodeDepthArr)
        {
            _networkDepth = networkDepth;
            _nodeDepthArr = nodeDepthArr;
        }
    }
}
