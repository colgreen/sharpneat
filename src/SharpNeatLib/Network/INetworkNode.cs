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
    /// Represents a single network node.
    /// Part of the INetworkDefinition type hierarchy.
    /// </summary>
    public interface INetworkNode
    {
        /// <summary>
        /// Gets the node's unique ID.
        /// </summary>
        uint Id { get; }
        ///// <summary>
        ///// Gets the node's type.
        ///// </summary>
        //NodeType NodeType { get; }
        /// <summary>
        /// Gets the node's activation function ID. This is an ID into an IActivationFunctionLibrary
        /// associated with the network as a whole.
        /// </summary>
        int ActivationFnId { get; }
    }
}
