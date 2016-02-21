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
using System.Diagnostics;

namespace SharpNeat.Phenomes
{
    /// <summary>
    /// SignalArray wraps a native array along with an offset into that array. The resulting SignalArray
    /// provides offset indexed access to the underlying native array.
    /// 
    /// SignalArray minimizes the amount of value copying required when setting input signal values to, and
    /// reading output values from an IBlackBox. E.g. FastCyclicNetwork requires all input, output and 
    /// hidden node activation values to be stored in a single array. This class allows us to handle direct 
    /// access to the input and output values through their own SignalArray, thus we can set individual values
    /// in the underlying native array directly without having knowledge of that array's structure. An alternative
    /// would be to pass arrays to SetInputs() and SetOutput() methods, requiring us to copy the complete contents
    /// of the arrays into the IBlackBox's working array on each call.
    /// 
    /// This class is effectively a substitute for array pointer manipulation as is possible in C++, e.g. in
    /// C++ you might do something like:
    /// <code>
    /// double[] allSignals = new double[100];
    /// double[] inputSignals = &amp;allSignals + 1;    // Skip bias neuron.
    /// double[] outputSignals = &amp;allSignals + 10;  // Skip bias and input neurons.
    /// </code>
    /// In the above example access to the real items outside of the bounds of the sub-ranges is
    /// possible (e.g. inputSignals[10] yields the first output signal). SignalArray also does not check for
    /// such out-of-bounds accesses, accept when running with a debugger attached in which case assertions will
    /// make these tests.
    /// </summary>
    public class SignalArray : ISignalArray
    {
        readonly double[] _wrappedArray;
        readonly int _offset;
        readonly int _length;

        #region Constructor

        /// <summary>
        /// Construct a SignalArray that wraps the provided wrappedArray.
        /// </summary>
        public SignalArray(double[] wrappedArray, int offset, int length)
        {
            if(offset + length > wrappedArray.Length) {
                throw new SharpNeatException("wrappedArray is not long enough to represent the requested SignalArray.");
            }

            _wrappedArray = wrappedArray;
            _offset = offset;
            _length = length;
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
                Debug.Assert(index > -1 && index < _length, "Out of bounds SignalArray access.");
                return _wrappedArray[_offset + index]; 
            }
            set
            {
                Debug.Assert(index > -1 && index < _length, "Out of bounds SignalArray access.");
                _wrappedArray[_offset + index] = value; 
            }
        }

        /// <summary>
        /// Gets the length of the signal array.
        /// </summary>
        public int Length
        {
            get { return _length; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Copies all elements from the current SignalArray to the specified target array starting 
        /// at the specified target Array index. 
        /// </summary>
        /// <param name="targetArray">The array to copy elements to.</param>
        /// <param name="targetIndex">The targetArray index at which copying to begins.</param>
        public void CopyTo(double[] targetArray, int targetIndex)
        {
            Array.Copy(_wrappedArray, _offset, targetArray, targetIndex, _length);
        }
        
        /// <summary>
        /// Copies <paramref name="length"/> elements from the current SignalArray to the specified target
        /// array starting at the specified target Array index. 
        /// </summary>
        /// <param name="targetArray">The array to copy elements to.</param>
        /// <param name="targetIndex">The targetArray index at which storing begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyTo(double[] targetArray, int targetIndex, int length)
        {
            Debug.Assert(length <= _length);
            Array.Copy(_wrappedArray, _offset, targetArray, targetIndex, length);
        }

        /// <summary>
        /// Copies <paramref name="length"/> elements from the current SignalArray to the specified target
        /// starting from <paramref name="targetIndex"/> on the target array and <paramref name="sourceIndex"/>
        /// on the current source SignalArray.
        /// </summary>
        /// <param name="targetArray">The array to copy elements to.</param>
        /// <param name="targetIndex">The targetArray index at which copying begins.</param>
        /// <param name="sourceIndex">The index into the current SignalArray at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyTo(double[] targetArray, int targetIndex, int sourceIndex, int length)
        {
            Debug.Assert(sourceIndex + length < _length);
            Array.Copy(_wrappedArray, _offset + sourceIndex, targetArray, targetIndex, length);
        }

        /// <summary>
        /// Copies all elements from the source array writing them into the current SignalArray starting
        /// at the specified targetIndex.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        public void CopyFrom(double[] sourceArray, int targetIndex)
        {
            Array.Copy(sourceArray, 0, _wrappedArray, _offset + targetIndex, sourceArray.Length);
        }

        /// <summary>
        /// Copies <paramref name="length"/> elements from the source array writing them to the current SignalArray 
        /// starting at the specified targetIndex.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyFrom(double[] sourceArray, int targetIndex, int length)
        {
            Debug.Assert(targetIndex + length < _length);
            Array.Copy(sourceArray, 0, _wrappedArray, _offset + targetIndex, length);
        }

        /// <summary>
        /// Copies <paramref name="length"/> elements starting from sourceIndex on sourceArray to the current
        /// SignalArray starting at the specified targetIndex.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="sourceIndex">The sourceArray index at which copying begins.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyFrom(double[] sourceArray, int sourceIndex, int targetIndex, int length)
        {
            Debug.Assert(targetIndex + length < _length);
            Array.Copy(sourceArray, sourceIndex, _wrappedArray, _offset + targetIndex, length);
        }

        /// <summary>
        /// Reset all array elements to zero.
        /// </summary>
        public void Reset()
        {
            for(int i=_offset; i < _offset + _length; i++) {
                _wrappedArray[i] = 0.0;
            }
        }

        #endregion
    }
}
