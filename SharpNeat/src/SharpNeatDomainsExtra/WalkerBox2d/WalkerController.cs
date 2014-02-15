/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2012 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
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
