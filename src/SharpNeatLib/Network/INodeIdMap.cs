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

namespace SharpNeat.Network
{
    /// <summary>
    /// Represents a mapping of graph node IDs from one ID space to another.
    /// </summary>
    public interface INodeIdMap
    {
        /// <summary>
        /// Gets the number of mapped node IDs.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Map a given node ID
        /// </summary>
        /// <param name="id">A node ID.</param>
        /// <returns>The mapped to ID.</returns>
        int Map(int id);
    }
}
