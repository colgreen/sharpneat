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

namespace SharpNeat.Utility
{
    /// <summary>
    /// This is a generic circular buffer of items of type T.  A circular buffer must be assigned
    /// a capacity at construction time. Items can be enqueued indefintely, but when the buffer's 
    /// capacity is reached the oldest values in the buffer are overwritten, thus the buffer is best
    /// thought of as a circular array or buffer.
    /// </summary>
    public class CircularBuffer<T>
    {
        /// <summary>
        /// Internal array that stores the circular buffer's values.
        /// </summary>
        protected readonly T[] _buff;

        /// <summary>
        /// The index of the previously enqueued item. -1 if buffer is empty.
        /// </summary>
        protected int _headIdx;

        /// <summary>
        /// The index of the next item to be dequeued. -1 if buffer is empty.
        /// </summary>
        protected int _tailIdx;

        #region Constructors

        /// <summary>
        /// Constructs a circular buffer with the specified capacity.
        /// </summary>
        public CircularBuffer(int capacity)
        {
            _buff = new T[capacity];
            _headIdx = _tailIdx = -1;
        }

        #endregion

        #region Properties / Indexer

        /// <summary>
        /// Gets the number of items in the buffer. Returns the buffer's capacity
        /// if it is full.
        /// </summary>
        public int Length
        {
            get
            {
                if(_headIdx == -1) {
                    return 0;
                }

                if(_headIdx > _tailIdx) {
                    return (_headIdx - _tailIdx) + 1;
                }

                if(_tailIdx > _headIdx) {
                    return (_buff.Length - _tailIdx) + _headIdx + 1;
                }

                return 1;
            }
        }

        /// <summary>
        /// Gets or sets an item at the specified index. The setter can only be used to replace an existing item, it
        /// cannot insert a new item at an arbitrary index.
        /// </summary>
        public T this[int idx]
        {
            get
            {
                if(_tailIdx == -1)
                {   // buffer is currently empty.
                    throw new InvalidOperationException("buffer is empty.");
                }

                // Calc index of item to retrieve.
                idx += _tailIdx;
                if(idx >= _buff.Length) {
                    // Wrap around.
                    idx -= _buff.Length;
                    if(idx > _headIdx) {
                        throw new IndexOutOfRangeException("Index is beyond the end of the available items.");
                    }
                }
                return _buff[idx];
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clear the buffer.
        /// </summary>
        public virtual void Clear()
        {
            _headIdx = _tailIdx = -1;
        }

        /// <summary>
        /// Enqueue a new item. This overwrites the oldest item in the buffer if the buffer
        /// has reached capacity.
        /// </summary>
        public virtual void Enqueue(T item)
        {
            if(_headIdx == -1)
            {   // buffer is currently empty.
                _headIdx = _tailIdx = 0;
                _buff[0] = item;
                return;
            }

            // Determine the index to write to.
            if(++_headIdx == _buff.Length)
            {   // Wrap around.
                _headIdx = 0;
            }

            if(_headIdx == _tailIdx)
            {   // Buffer overflow. Increment tailIdx.
                if(++_tailIdx == _buff.Length) 
                {   // Wrap around.
                    _tailIdx=0;
                }
            }

            _buff[_headIdx] = item;
            return;
        }

        /// <summary>
        /// Remove the oldest item from the back end of the buffer and return it.
        /// </summary>
        public virtual T Dequeue()
        {
            if(_tailIdx == -1)
            {   // buffer is currently empty.
                throw new InvalidOperationException("buffer is empty.");
            }

            T item = _buff[_tailIdx];

            if(_tailIdx == _headIdx)
            {   // The buffer is now empty.
                _headIdx = _tailIdx = -1;
                return item;
            }

            if(++_tailIdx == _buff.Length)
            {   // Wrap around.
                _tailIdx = 0;
            }

            return item;
        }

        /// <summary>
        /// Pop the most recently added item from the front end of the buffer and return it.
        /// </summary>
        public virtual T Pop()
        {
            if(_tailIdx == -1)
            {   // buffer is currently empty.
                throw new InvalidOperationException("buffer is empty.");
            }   

            T item = _buff[_headIdx];

            if(_tailIdx == _headIdx)
            {   // The buffer is now empty.
                _headIdx = _tailIdx = -1;
                return item;
            }

            if(--_headIdx == -1)
            {   // Wrap around.
                _headIdx=_buff.Length-1;
            }

            return item;
        }

        #endregion
    }
}
