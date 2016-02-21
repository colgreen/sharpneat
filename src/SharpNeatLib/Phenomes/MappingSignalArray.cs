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
using System.Diagnostics;

namespace SharpNeat.Phenomes
{
    /// <summary>
    /// MappingSignalArray wraps a native array along with an indirection/mapping array.
    /// The resulting MappingSignalArray provides indexed access to the underlying native array via 
    /// a level of indirection/mapping.
    /// 
    /// See SignalArray for more info.
    /// </summary>
    public class MappingSignalArray : ISignalArray
    {
        readonly double[] _wrappedArray;
        readonly int[] _map;

        #region Constructor

        /// <summary>
        /// Construct a SignalArray that wraps the provided wrappedArray.
        /// </summary>
        public MappingSignalArray(double[] wrappedArray, int[] map)
        {
            _wrappedArray = wrappedArray;
            _map = map;
            Debug.Assert(ValidateMapIndexes());
        }

        #endregion

        #region Indexer / Properties

        /// <summary>
        /// Gets or sets the single value at the specified index.
        /// 
        /// We assert that the index is within the defined range of the signal array. Throwing
        /// an exception would be more correct but the check would affect performance of problem
        /// domains with large I/O throughput.
        /// </summary>
        public double this[int index]
        {
            get 
            {
                Debug.Assert(index > -1 && index < _map.Length, "Out of bounds MappingSignalArray access.");
                return _wrappedArray[_map[index]]; 
            }
            set
            {
                Debug.Assert(index > -1 && index < _map.Length, "Out of bounds MappingSignalArray access.");
                _wrappedArray[_map[index]] = value; 
            }
        }

        /// <summary>
        /// Gets the length of the signal array.
        /// </summary>
        public int Length
        {
            get { return _map.Length; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Copies all elements from the current MappingSignalArray to the specified target array starting 
        /// at the specified target Array index. 
        /// </summary>
        /// <param name="targetArray">The array to copy elements to.</param>
        /// <param name="targetIndex">The targetArray index at which copying to begins.</param>
        public void CopyTo(double[] targetArray, int targetIndex)
        {
            for(int i=0, tgtIdx=targetIndex; i<_map.Length; i++, tgtIdx++) {
                targetArray[tgtIdx] = _wrappedArray[_map[i]];
            }
        }

        /// <summary>
        /// Copies <paramref name="length"/> elements from the current MappingSignalArray to the specified target
        /// array starting at the specified target Array index. 
        /// </summary>
        /// <param name="targetArray">The array to copy elements to.</param>
        /// <param name="targetIndex">The targetArray index at which storing begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyTo(double[] targetArray, int targetIndex, int length)
        {
            Debug.Assert(length <= _map.Length);
            for(int i=0, tgtIdx=targetIndex; i<length; i++, tgtIdx++) {
                targetArray[tgtIdx] = _wrappedArray[_map[i]];
            }
        }

        /// <summary>
        /// Copies <paramref name="length"/> elements from the current MappingSignalArray to the specified target
        /// starting from <paramref name="targetIndex"/> on the target array and <paramref name="sourceIndex"/>
        /// on the current source SignalArray.
        /// </summary>
        /// <param name="targetArray">The array to copy elements to.</param>
        /// <param name="targetIndex">The targetArray index at which copying begins.</param>
        /// <param name="sourceIndex">The index into the current SignalArray at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyTo(double[] targetArray, int targetIndex, int sourceIndex, int length)
        {
            Debug.Assert(sourceIndex + length < _map.Length);
            for(int i=sourceIndex, tgtIdx=targetIndex; i<sourceIndex+length; i++, tgtIdx++) {
                targetArray[tgtIdx] = _wrappedArray[_map[i]];
            }
        }

        /// <summary>
        /// Copies all elements from the source array writing them into the current MappingSignalArray starting
        /// at the specified targetIndex.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        public void CopyFrom(double[] sourceArray, int targetIndex)
        {
            Debug.Assert(sourceArray.Length <= (_map.Length - targetIndex));
            for(int i=0, tgtIdx=targetIndex; i<sourceArray.Length; i++, tgtIdx++){
                _wrappedArray[_map[tgtIdx]] = sourceArray[i];
            }
        }

        /// <summary>
        /// Copies <paramref name="length"/> elements from the source array writing them to the current MappingSignalArray 
        /// starting at the specified targetIndex.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyFrom(double[] sourceArray, int targetIndex, int length)
        {
            Debug.Assert(length <= (_map.Length - targetIndex));
            for(int i=0, tgtIdx=targetIndex; i<length; i++, tgtIdx++){
                _wrappedArray[_map[tgtIdx]] = sourceArray[i];
            }
        }

        /// <summary>
        /// Copies <paramref name="length"/> elements starting from sourceIndex on sourceArray to the current
        /// MappingSignalArray starting at the specified targetIndex.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="sourceIndex">The sourceArray index at which copying begins.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyFrom(double[] sourceArray, int sourceIndex, int targetIndex, int length)
        {
            Debug.Assert((sourceIndex + length <= sourceArray.Length) && (targetIndex + length <= _map.Length));
            for(int i=sourceIndex, tgtIdx=targetIndex; i < sourceIndex + length; i++, tgtIdx++) {
                _wrappedArray[_map[tgtIdx]] = sourceArray[i];
            }
        }

        /// <summary>
        /// Reset all array elements to zero.
        /// </summary>
        public void Reset()
        {
            for(int i=0; i<_map.Length; i++) {
                _wrappedArray[_map[i]] = 0.0;
            }
        }

        #endregion

        #region Private Methods [Debug Assert Helper Methods]

        /// <summary>
        /// Validate the indexes within _map.
        /// Returns true if they are all valid (within the indexable range of _wrappedArray)
        /// </summary>
        /// <returns></returns>
        private bool ValidateMapIndexes()
        {
            for(int i=0; i < _map.Length; i++)
            {
                if(_map[i] < 0 || _map[i] >= _wrappedArray.Length) {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
