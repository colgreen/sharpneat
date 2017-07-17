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
    /// Represents a single network connection.
    /// Part of the INetworkDefinition type hierarchy.
    /// </summary>
    public interface INetworkConnection
    {
        /// <summary>
        /// Gets the ID of the connection's source node.
        /// </summary>
        uint SourceNodeId { get; }
        /// <summary>
        /// Gets the ID of the connection's target node.
        /// </summary>
        uint TargetNodeId { get; }
        /// <summary>
        /// Gets the connection's weight.
        /// </summary>
        double Weight { get; }
    }
}
