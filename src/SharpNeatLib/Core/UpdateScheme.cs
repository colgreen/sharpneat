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
using System;

namespace SharpNeat.Core
{
    /// <summary>
    /// Represents an update scheme for an IEvolutionAlgorithm. e.g. update per some time duration or 
    /// some number of generations.
    /// </summary>
    public class UpdateScheme
    {
        readonly UpdateMode _updateMode;
        readonly uint _generations;
        readonly TimeSpan _timespan;

        #region Constructors

        /// <summary>
        /// Constructs a 'per generations' update scheme.
        /// </summary>
        public UpdateScheme(uint generations)
        {
            _updateMode = UpdateMode.Generational;
            _generations = generations;
        }

        /// <summary>
        /// Constructs a 'per timespan' update scheme.
        /// </summary>
        public UpdateScheme(TimeSpan timespan)
        {
            _updateMode = UpdateMode.Timespan;
            _timespan = timespan;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the update scheme's mode.
        /// </summary>
        public UpdateMode UpdateMode
        {
            get { return _updateMode; }
        }

        /// <summary>
        /// Gets the number of generations between updates; Applies only to the generational update scheme.
        /// </summary>
        public uint Generations
        {
            get { return _generations; }
        }

        /// <summary>
        /// Gets the number timespan between updates; Applies only to the timespan update scheme.
        /// </summary>
        public TimeSpan TimeSpan
        {
            get { return _timespan; }
        }

        #endregion
    }
}
