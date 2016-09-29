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
namespace SharpNeat.DomainsExtra.WalkerBox2d
{
    /// <summary>
    /// Base/abstract class for walker controllers.
    /// </summary>
    public abstract class WalkerController
    {
        /// <summary>
        /// The walker interface to be used for controlling the walker.
        /// </summary>
        protected WalkerInterface _iface;

        /// <summary>
        /// Construct with the provided player interface.
        /// </summary>
        public WalkerController(WalkerInterface iface)
        {
            _iface = iface;
        }

        /// <summary>
        /// Perform one controller step. Typically consisting of reading world and walker state
        /// from the walker interface and updating the torques applied at each leg joint.
        /// </summary>
        public abstract void Step();
    }
}
