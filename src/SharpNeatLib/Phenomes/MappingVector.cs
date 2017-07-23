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
using System;
using System.Diagnostics;

namespace SharpNeat.Phenomes
{
    /// <summary>
    /// Wraps a native array along with an indirection/mapping array.
    /// </summary>
    public class MappingVector<T> : IVector<T> where T : struct
    {
        readonly T[] _innerArr;
        readonly int[] _map;

        #region Constructor

        /// <summary>
        /// Construct a SignalArray that wraps the provided wrappedArray.
        /// </summary>
        public MappingVector(T[] innerArray, int[] map)
        {
            Debug.Assert(ValidateMapIndexes(innerArray, map));

            _innerArr = innerArray;
            _map = map;
        }

        #endregion

        #region Indexer / Properties

        /// <summary>
        /// Gets or sets the single value at the specified index.
        /// 
        /// We debug assert that the index is within the defined range of the signal array. Throwing
        /// an exception would be more correct but the check would affect performance of problem
        /// domains with large I/O throughput.
        /// </summary>
        public virtual T this[int index]
        {
            get 
            {
                Debug.Assert(index > -1 && index < _map.Length, "Out of bounds MappingSignalArray access.");
                return _innerArr[_map[index]]; 
            }
            set
            {
                Debug.Assert(index > -1 && index < _map.Length, "Out of bounds MappingSignalArray access.");
                _innerArr[_map[index]] = value; 
            }
        }

        /// <summary>
        /// Gets the length of the signal array.
        /// </summary>
        public int Length => _map.Length;

        #endregion

        #region Public Methods

        /// <summary>
        /// Copies all elements from the current MappingSignalArray to the specified target array starting 
        /// at the specified target Array index. 
        /// </summary>
        /// <param name="targetArray">The array to copy elements to.</param>
        /// <param name="targetIndex">The targetArray index at which copying to begins.</param>
        public void CopyTo(T[] targetArray, int targetIndex)
        {
            for(int i=0, tgtIdx=targetIndex; i<_map.Length; i++, tgtIdx++) {
                targetArray[tgtIdx] = _innerArr[_map[i]];
            }
        }

        /// <summary>
        /// Copies <paramref name="length"/> elements from the current MappingSignalArray to the specified target
        /// array starting at the specified target Array index. 
        /// </summary>
        /// <param name="targetArray">The array to copy elements to.</param>
        /// <param name="targetIndex">The targetArray index at which storing begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyTo(T[] targetArray, int targetIndex, int length)
        {
            Debug.Assert(length <= _map.Length);
            for(int i=0, tgtIdx=targetIndex; i<length; i++, tgtIdx++) {
                targetArray[tgtIdx] = _innerArr[_map[i]];
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
        public void CopyTo(T[] targetArray, int targetIndex, int sourceIndex, int length)
        {
            Debug.Assert(sourceIndex + length < _map.Length);
            for(int i=sourceIndex, tgtIdx=targetIndex; i<sourceIndex+length; i++, tgtIdx++) {
                targetArray[tgtIdx] = _innerArr[_map[i]];
            }
        }

        /// <summary>
        /// Copies all elements from the source array writing them into the current MappingSignalArray starting
        /// at the specified targetIndex.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        public void CopyFrom(T[] sourceArray, int targetIndex)
        {
            Debug.Assert(sourceArray.Length <= (_map.Length - targetIndex));
            for(int i=0, tgtIdx=targetIndex; i<sourceArray.Length; i++, tgtIdx++){
                _innerArr[_map[tgtIdx]] = sourceArray[i];
            }
        }

        /// <summary>
        /// Copies <paramref name="length"/> elements from the source array writing them to the current MappingSignalArray 
        /// starting at the specified targetIndex.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyFrom(T[] sourceArray, int targetIndex, int length)
        {
            Debug.Assert(length <= (_map.Length - targetIndex));
            for(int i=0, tgtIdx=targetIndex; i<length; i++, tgtIdx++){
                _innerArr[_map[tgtIdx]] = sourceArray[i];
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
        public void CopyFrom(T[] sourceArray, int sourceIndex, int targetIndex, int length)
        {
            Debug.Assert((sourceIndex + length <= sourceArray.Length) && (targetIndex + length <= _map.Length));
            for(int i=sourceIndex, tgtIdx=targetIndex; i < sourceIndex + length; i++, tgtIdx++) {
                _innerArr[_map[tgtIdx]] = sourceArray[i];
            }
        }

        /// <summary>
        /// Reset all array elements to zero.
        /// </summary>
        public void Reset()
        {
            for(int i=0; i<_map.Length; i++) {
                _innerArr[_map[i]] = default(T);
            }
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Validate the indexes within _map.
        /// Returns true if they are all valid (within the indexable range of _wrappedArray)
        /// </summary>
        /// <returns></returns>
        private static bool ValidateMapIndexes(T[] wrappedArray, int[] map)
        {
            for(int i=0; i < map.Length; i++)
            {
                if(map[i] < 0 || map[i] >= wrappedArray.Length) {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
