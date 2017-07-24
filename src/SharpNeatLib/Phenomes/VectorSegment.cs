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
    /// Wraps a native array along with an offset into that array. The resulting VectorSegment
    /// provides offset indexed access to the underlying native array.
    /// 
    /// VectorSegment minimizes the amount of value copying required when setting input signal values to, and
    /// reading output values from an IBlackBox. E.g. CyclicNeuralNet requires all input, output and 
    /// hidden node activation values to be stored in a single array. This class allows us to handle direct 
    /// access to the input and output values through their own VectorSegment, thus we can set individual values
    /// in the underlying native array directly without having knowledge of that array's structure. An alternative
    /// would be to pass arrays to SetInputs() and SetOutput() methods, requiring us to copy the complete contents
    /// of the arrays into the IBlackBox's working array on each call.
    /// 
    /// This class is effectively a substitute for array pointer manipulation as is possible in C++, e.g. in
    /// C++ you might do something like:
    /// <code>
    /// double[] allSignals = new double[100];
    /// double[] inputSignals = &amp;allSignals; 
    /// double[] outputSignals = &amp;allSignals + 10;  // Skip input neurons.
    /// </code>
    /// In the above example access to the real items outside of the bounds of the sub-ranges is
    /// possible (e.g. inputSignals[10] yields the first output signal). VectorSegment also does not check for
    /// such out-of-bounds accesses, accept when running with a debugger attached in which case assertions will
    /// make these tests.
    /// </summary>
    public class VectorSegment<T> : IVector<T> where T : struct
    {
        readonly T[] _innerArr;
        readonly int _offset;
        readonly int _length;

        #region Constructor

        /// <summary>
        /// Construct a VectorSegment that wraps the provided innerArray.
        /// </summary>
        public VectorSegment(T[] innerArray, int offset, int length)
        {
            if(offset < 0 || offset >= innerArray.Length) {
                throw new ArgumentOutOfRangeException("Invalid offset.", "offset");
            }

            if(offset + length > innerArray.Length) {
                throw new ArgumentOutOfRangeException("Invalid length.", "length");
            }

            _innerArr = innerArray;
            _offset = offset;
            _length = length;
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
        /// <remarks>
        /// </remarks>
        public virtual T this[int index]
        {
            get 
            {
                if(index < 0 || index >= _length) {
                    throw new ArgumentOutOfRangeException("index");
                }
                return _innerArr[_offset + index]; 
            }
            set
            {
                if(index < 0 || index >= _length) {
                    throw new ArgumentOutOfRangeException("index");
                }
                _innerArr[_offset + index] = value; 
            }
        }

        /// <summary>
        /// Gets the length of the signal array.
        /// </summary>
        public int Length => _length;

        #endregion

        #region Public Methods [CopyTo*]

        /// <summary>
        /// Copies all elements from the current VectorSegment to the specified target array starting 
        /// at the specified target Array index. 
        /// </summary>
        /// <param name="targetArray">The array to copy elements to.</param>
        /// <param name="targetIndex">The targetArray index at which copying to begins.</param>
        public void CopyTo(T[] targetArray, int targetIndex)
        {
            Array.Copy(_innerArr, _offset, targetArray, targetIndex, _length);
        }
        
        /// <summary>
        /// Copies <paramref name="length"/> elements from the current VectorSegment to the specified target
        /// array starting at the specified target Array index. 
        /// </summary>
        /// <param name="targetArray">The array to copy elements to.</param>
        /// <param name="targetIndex">The targetArray index at which storing begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyTo(T[] targetArray, int targetIndex, int length)
        {
            if(length > _length) {
                throw new ArgumentOutOfRangeException("length");
            }
            Array.Copy(_innerArr, _offset, targetArray, targetIndex, length);
        }

        /// <summary>
        /// Copies <paramref name="length"/> elements from the current VectorSegment to the specified target
        /// starting from <paramref name="targetIndex"/> on the target array and <paramref name="sourceIndex"/>
        /// on the current source VectorSegment.
        /// </summary>
        /// <param name="targetArray">The array to copy elements to.</param>
        /// <param name="targetIndex">The targetArray index at which copying begins.</param>
        /// <param name="sourceIndex">The index into the current VectorSegment at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyTo(T[] targetArray, int targetIndex, int sourceIndex, int length)
        {
            if(sourceIndex < 0 || sourceIndex + length > _length) {
                throw new ArgumentOutOfRangeException("sourceIndex, length");
            }
            Array.Copy(_innerArr, _offset + sourceIndex, targetArray, targetIndex, length);
        }

        #endregion

        #region Public Methods [CopyFrom*]

        /// <summary>
        /// Copies all elements from the source array writing them into the current VectorSegment starting
        /// at the specified targetIndex.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="targetIndex">The index into the current VectorSegment at which copying begins.</param>
        public void CopyFrom(T[] sourceArray, int targetIndex)
        {
            if(targetIndex < 0 || targetIndex + sourceArray.Length > _length) {
                throw new ArgumentOutOfRangeException("targetIndex");
            }
            Array.Copy(sourceArray, 0, _innerArr, _offset + targetIndex, sourceArray.Length);
        }

        /// <summary>
        /// Copies <paramref name="length"/> elements from the source array writing them to the current VectorSegment 
        /// starting at the specified targetIndex.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="targetIndex">The index into the current VectorSegment at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyFrom(T[] sourceArray, int targetIndex, int length)
        {
            if(targetIndex < 0 || targetIndex + length > _length) {
                throw new ArgumentOutOfRangeException("targetIndex");
            }
            Array.Copy(sourceArray, 0, _innerArr, _offset + targetIndex, length);
        }

        /// <summary>
        /// Copies <paramref name="length"/> elements starting from sourceIndex on sourceArray to the current
        /// VectorSegment starting at the specified targetIndex.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="sourceIndex">The sourceArray index at which copying begins.</param>
        /// <param name="targetIndex">The index into the current VectorSegment at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyFrom(T[] sourceArray, int sourceIndex, int targetIndex, int length)
        {
            if(targetIndex < 0 || targetIndex + length > _length) {
                throw new ArgumentOutOfRangeException("targetIndex");
            }
            Array.Copy(sourceArray, sourceIndex, _innerArr, _offset + targetIndex, length);
        }

        /// <summary>
        /// Reset all array elements to zero.
        /// </summary>
        public void Reset()
        {
            for(int i=_offset; i < _offset + _length; i++) {
                _innerArr[i] = default(T);
            }
        }

        #endregion
    }
}
