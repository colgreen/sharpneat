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
using System.Collections.Generic;

namespace SharpNeat.Network
{
    /// <summary>
    /// Represents a list of INetworkConnection's.
    /// Part of the INetworkDefinition type hierarchy.
    /// </summary>
    public interface IConnectionList : IEnumerable<INetworkConnection>
    {
        /// <summary>
        /// Gets the INetworkConnection at the specified index.
        /// </summary>
        INetworkConnection this[int index] { get; }

        /// <summary>
        /// Gets the count of connections in the list.
        /// </summary>
        int Count { get; }
    }
}
