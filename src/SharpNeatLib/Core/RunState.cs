/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
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
namespace SharpNeat.Core
{
    /// <summary>
    /// An enumeration of possible execution states for an IEvolutionAlgorithm.
    /// </summary>
    public enum RunState
    {
        /// <summary>
        /// Not yet initialized.
        /// </summary>
        NotReady,
        /// <summary>
        /// Initialized and ready to start.
        /// </summary>
        Ready,
        /// <summary>
        /// The algorithm is running.
        /// </summary>
        Running,
        /// <summary>
        /// The algorithm has been paused, either due to a user request or because a stop condition
        /// has been met. The algorithm can be restarted if the stop condition is no longer true.
        /// </summary>
        Paused,
        /// <summary>
        /// The algorithm thread has terminated. The algorithm cannot be restarted from this state, a new
        /// algorithm object must be created and started afresh.
        /// </summary>
        Terminated
    }
}
