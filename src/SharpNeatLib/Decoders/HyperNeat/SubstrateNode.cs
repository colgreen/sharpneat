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
namespace SharpNeat.Decoders.HyperNeat
{
    /// <summary>
    /// Represents a node within a HyperNEAT substrate.
    /// </summary>
    public struct SubstrateNode
    {
        /// <summary>
        /// Node ID.
        /// </summary>
        public readonly uint _id;
        /// <summary>
        /// The node's position coordinates on the substrate. The substrate dimensionality is not restricted.
        /// </summary>
        public readonly double[] _position;
        
        /// <summary>
        /// Construct with the specified node ID and position coordinates.
        /// </summary>
        public SubstrateNode(uint id, double[] position)
        {
            _id = id;
            _position = position;
        }
    }
}
