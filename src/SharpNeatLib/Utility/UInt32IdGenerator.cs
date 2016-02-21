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
namespace SharpNeat.Utility
{
    /// <summary>
    /// Conveniently encapsulates a single UInt32, which is incremented to produce new IDs.
    /// </summary>
    public class UInt32IdGenerator
    {
        uint _nextId;

        #region Constructors

        /// <summary>
        /// Construct, setting the initial ID to zero.
        /// </summary>
        public UInt32IdGenerator()
        {
            _nextId = 0;
        }

        /// <summary>
        /// Construct, setting the initial ID to the value provided.
        /// </summary>
        public UInt32IdGenerator(uint nextId)
        {
            _nextId = nextId;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the next ID. IDs wrap around to zero when uint.MaxValue is reached. 
        /// </summary>
        public uint NextId
        {
            get
            {   // ENHANCEMENT: Consider triggering an ID deframentaion here.
                if (_nextId == uint.MaxValue) {
                    _nextId = 0;
                }
                return _nextId++;
            }
        }

        /// <summary>
        /// Get the next ID without incrementing (peek the ID).
        /// </summary>
        public uint Peek
        {
            get { return _nextId; }
        }

        /// <summary>
        /// Resets the next ID back to zero.
        /// </summary>
        public void Reset()
        {
            _nextId = 0;
        }

        /// <summary>
        /// Resets the next ID to a specific value.
        /// </summary>
        public void Reset(uint nextId)
        {
            _nextId = nextId;
        }

        #endregion
    }
}
