/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Diagnostics;

namespace SharpNeat.BlackBox
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
    /// <typeparam name="T">Vector data type.</typeparam>
    public sealed class VectorSegment<T> : IVector<T> where T : struct
    {
        readonly T[] _innerArr;
        readonly int _offset;
        readonly int _length;

        #region Constructor

        /// <summary>
        /// Constructs a vector that wraps the provided array.
        /// </summary>
        /// <param name="innerArray">An array that contains the elements of the vector.</param>
        /// <param name="offset">An offset into <paramref name="innerArray"/>, that points to the first element of the vector segment.</param>
        /// <param name="length">Length of the vector segment.</param>
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
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index of the element to get or set.</param>
        /// <remarks>
        /// Debug asserts are used to check the index value, this avoids the check in release builds thus improving performance,
        /// but includes the check in debug builds. Tasks will typically access this indexer heavily, therefore the removal of
        /// the test in release builds was deemed a reasonable choice here.
        /// </remarks>
        public T this[int index]
        {
            // ENHANCEMENT: Consider adding ref keyword to make this return a variable rather than a value (this feature is new to C#7)
            get
            {
                Debug.Assert(index > -1 && index < _length);
                return _innerArr[_offset + index];
            }
            set
            {
                Debug.Assert(index > -1 && index < _length);
                _innerArr[_offset + index] = value;
            }
        }

        /// <summary>
        /// Gets the length of the vector.
        /// </summary>
        public int Length => _length;

        #endregion

        #region Public Methods [CopyTo*]

        /// <summary>
        /// Copies all elements from the current vector to the specified target array, starting
        /// at the specified target array index.
        /// </summary>
        /// <param name="targetArray">The target array to copy elements in to.</param>
        /// <param name="targetIndex">The index in <paramref name="targetArray"/> at which copying to begins.</param>
        public void CopyTo(T[] targetArray, int targetIndex)
        {
            Array.Copy(_innerArr, _offset, targetArray, targetIndex, _length);
        }

        /// <summary>
        /// Copies <paramref name="length"/> elements from the current vector to the specified target
        /// array, starting at the specified target array index.
        /// </summary>
        /// <param name="targetArray">The target array to copy elements in to.</param>
        /// <param name="targetIndex">The index in <paramref name="targetArray"/> at which copying to begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyTo(T[] targetArray, int targetIndex, int length)
        {
            if(length > _length) {
                throw new ArgumentException("Invalid copy operation.");
            }
            Array.Copy(_innerArr, _offset, targetArray, targetIndex, length);
        }

        /// <summary>
        /// Copies <paramref name="length"/> elements from the current vector to the specified target
        /// array, starting from <paramref name="targetIndex"/> on the target array and
        /// <paramref name="sourceIndex"/> in the current vector..
        /// </summary>
        /// <param name="targetArray">The target array to copy elements in to.</param>
        /// <param name="targetIndex">The index in <paramref name="targetArray"/> at which copying to begins.</param>
        /// <param name="sourceIndex">The index into the current vector  at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyTo(T[] targetArray, int targetIndex, int sourceIndex, int length)
        {
            if(    sourceIndex < 0
                || sourceIndex + length > _length) {
                throw new ArgumentException("Invalid copy operation.");
            }
            Array.Copy(_innerArr, _offset + sourceIndex, targetArray, targetIndex, length);
        }

        #endregion

        #region Public Methods [CopyFrom*]

        /// <summary>
        /// Copies all elements from the source array writing them into the current vector starting at
        /// <paramref name="targetIndex"/>.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        public void CopyFrom(T[] sourceArray, int targetIndex)
        {
            if(    targetIndex < 0
                || targetIndex + sourceArray.Length > _length) {
                throw new ArgumentException("Invalid copy operation.");
            }
            Array.Copy(sourceArray, 0, _innerArr, _offset + targetIndex, sourceArray.Length);
        }

        /// <summary>
        /// Copies <paramref name="length"/> elements from the source array writing them to the current vector
        /// starting at <paramref name="targetIndex"/>.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyFrom(T[] sourceArray, int targetIndex, int length)
        {
            if(    targetIndex < 0
                || targetIndex + length > _length) {
                throw new ArgumentException("Invalid copy operation.");
            }
            Array.Copy(sourceArray, 0, _innerArr, _offset + targetIndex, length);
        }

        /// <summary>
        /// Copies <paramref name="length"/> elements starting from <paramref name="sourceIndex"/> in
        /// <paramref name="sourceArray"/>, to the current vector, starting at <paramref name="targetIndex"/>.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="sourceIndex">The sourceArray index at which copying begins.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        public void CopyFrom(T[] sourceArray, int sourceIndex, int targetIndex, int length)
        {
            if(    targetIndex < 0
                || targetIndex + length > _length) {
                throw new ArgumentException("Invalid copy operation.");
            }
            Array.Copy(sourceArray, sourceIndex, _innerArr, _offset + targetIndex, length);
        }

        /// <summary>
        /// Reset all vector elements to zero.
        /// </summary>
        public void Reset()
        {
            Array.Clear(_innerArr, _offset, _length);
        }

        #endregion
    }
}
