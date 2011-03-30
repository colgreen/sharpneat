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
    /// This is a circular buffer of double precision floating point numbers. 
    /// A circular buffer must be assigned a capacity at construction time. Values
    /// can be enqueued indefintely, but when the buffer's capacity is reached the oldest values
    /// in the buffer are overwritten, thus the buffer is best thought of as a circular array 
    /// or buffer.
    ///
    /// In addition to normal circular buffer behaviour this class has a 'total' variable that
    /// maintains the sum total of all values currently in the buffer. Therefore when the buffer 
    /// reaches capacity and new values overwrite old ones the total is reduced by the value being
    /// overwritten and increased by the new value. This allows us to cheaply (in computational terms)
    /// maintain a sum total and mean for all values in the buffer.
    /// 
    /// Note that this class isn't made generic because of the lack of operator contraints required 
    /// to maintain the sum total of contained items. There are ways around this limitation but they
    /// affect performance or result in ugly code.
    /// </summary>
    public class DoubleCircularBufferWithStats
    {
        /// <summary>
        /// Internal array that stores the circular buffer's values.
        /// </summary>
        readonly double[] _buff;

        /// <summary>
        /// The sum total of all valid values within the buffer. 
        /// </summary>
        double _total = 0.0;

        /// <summary>
        /// The index of the previously enqueued item. -1 if buffer is empty.
        /// </summary>
        int _headIdx;

        /// <summary>
        /// The index of the next item to be dequeued. -1 if buffer is empty.
        /// </summary>
        int _tailIdx;

        #region Constructors

        /// <summary>
        /// Constructs a circular buffer with the specified capacity.
        /// </summary>
        public DoubleCircularBufferWithStats(int capacity)
        {
            _buff = new double[capacity];
            _headIdx = _tailIdx = -1;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of values in the buffer. Returns the buffer's capacity if it is full.
        /// </summary>
        public int Length
        {
            get
            {
                if(_headIdx == -1) {
                    return 0;
                }

                if(_headIdx > _tailIdx) {
                    return (_headIdx-_tailIdx)+1;
                }

                if(_tailIdx > _headIdx) {
                    return (_buff.Length - _tailIdx) + _headIdx + 1;
                }

                return 1;
            }
        }

        /// <summary>
        /// Gets the sum total of all values on in the buffer.
        /// </summary>
        public double Total
        {
            get { return _total; }
        }

        /// <summary>
        /// Gets the arithmetic mean of all values in the buffer.
        /// </summary>
        public double Mean
        {
            get 
            {
                if(-1 == _headIdx) {
                    return 0.0;
                }
                return _total / Length; 
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clear the buffer and reset the total.
        /// </summary>
        public void Clear()
        {
            _headIdx = _tailIdx = -1;
            _total = 0.0;
        }

        /// <summary>
        /// Enqueue a new value. This overwrites the oldest value in the buffer if the buffer
        /// has reached capacity.
        /// </summary>
        public void Enqueue(double item)
        {
            if(_headIdx == -1)
            {   // buffer is currently empty.
                _headIdx = _tailIdx = 0;
                _buff[0] = item;
                _total += item;
                return;
            }

            // Determine the index to write to.
            if(++_headIdx == _buff.Length)
            {   // Wrap around.
                _headIdx=0;
            }

            if(_headIdx == _tailIdx)
            {   // Buffer overflow. Increment tailIdx.
                _total -= _buff[_headIdx];
                if(++_tailIdx == _buff.Length) 
                {   // Wrap around.
                    _tailIdx=0;
                }
            }

            _buff[_headIdx] = item;
            _total += item;
            return;
        }

        /// <summary>
        /// Remove the oldest value from the back end of the buffer and return it.
        /// </summary>
        public double Dequeue()
        {
            if(_tailIdx == -1)
            {   // buffer is currently empty.
                throw new InvalidOperationException("buffer is empty.");
            }

            double d = _buff[_tailIdx];
            _total -= d;

            if(_tailIdx == _headIdx)
            {   // The buffer is now empty.
                _headIdx = _tailIdx = -1;
                return d;
            }

            if(++_tailIdx == _buff.Length)
            {   // Wrap around.
                _tailIdx = 0;
            }

            return d;
        }

        /// <summary>
        /// Pop the most recently added value from the front end of the buffer and return it.
        /// </summary>
        public double Pop()
        {
            if(_tailIdx == -1)
            {   // buffer is currently empty.
                throw new InvalidOperationException("buffer is empty.");
            }   

            double d = _buff[_headIdx];
            _total -= d;

            if(_tailIdx == _headIdx)
            {   // The buffer is now empty.
                _headIdx = _tailIdx = -1;
                return d;
            }

            if(--_headIdx == -1)
            {   // Wrap around.
                _headIdx = _buff.Length - 1;
            }

            return d;
        }

        #endregion
    }
}
