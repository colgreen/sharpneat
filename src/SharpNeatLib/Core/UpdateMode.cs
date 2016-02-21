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
    /// An enumeration of update schemes, e.g. Fire an update event the per some time duration or some number of generations.
    /// </summary>
    public enum UpdateMode
    {
        /// <summary>
        /// Raise an update event at regular time intervals.
        /// </summary>
        Timespan,
        /// <summary>
        /// Raise an update event at regular generation intervals. (Every N generations).
        /// </summary>
        Generational
    }
}
