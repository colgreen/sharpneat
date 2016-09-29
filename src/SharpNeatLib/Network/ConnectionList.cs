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
    /// A concrete implementation of IConnectionList.
    /// Part of the INetworkDefinition type hierarchy.
    /// </summary>
    public class ConnectionList : List<INetworkConnection>, IConnectionList
    {
        #region Constructors

        /// <summary>
        /// Constructs an empty list.
        /// </summary>
        public ConnectionList() : base()
        {
        }

        /// <summary>
        /// Constructs a list with the specified initial capacity.
        /// </summary>
        public ConnectionList(int capacity) : base(capacity)
        {
        }

        #endregion
    }
}
