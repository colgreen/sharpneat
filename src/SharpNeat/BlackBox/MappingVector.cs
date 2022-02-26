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

namespace SharpNeat.BlackBox;

/// <summary>
/// Wraps a native array and a second 'mapping' array that maps vector indexes to indexes within the array.
/// </summary>
/// <typeparam name="T">Vector element type.</typeparam>
public sealed class MappingVector<T> : IVector<T> where T : struct
{
    readonly T[] _innerArr;
    readonly int[] _map;

    #region Constructor

    /// <summary>
    /// Construct a mapping vector that wraps the provided array and mapping array.
    /// </summary>
    /// <param name="innerArray">An array that contains the elements of the vector.</param>
    /// <param name="map">An array that maps vector indexes to indexes within <paramref name="innerArray"/>.</param>
    public MappingVector(T[] innerArray, int[] map)
    {
        // Note. This test is a debug assert to allow for checking of code validity in debug builds,
        // and avoiding the cost of the test in release builds.
        Debug.Assert(ValidateMapIndexes(innerArray, map));

        _innerArr = innerArray;
        _map = map;
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
        get
        {
            Debug.Assert(index > -1 && index < _map.Length);
            return _innerArr[_map[index]];
        }
        set
        {
            Debug.Assert(index > -1 && index < _map.Length);
            _innerArr[_map[index]] = value;
        }
    }

    /// <summary>
    /// Gets the length of the vector.
    /// </summary>
    public int Length => _map.Length;

    #endregion

    #region Public Methods

    /// <summary>
    /// Copies all elements from the current vector to the specified target array, starting
    /// at the specified target array index.
    /// </summary>
    /// <param name="targetArray">The target array to copy elements in to.</param>
    /// <param name="targetIndex">The index in <paramref name="targetArray"/> at which copying to begins.</param>
    public void CopyTo(T[] targetArray, int targetIndex)
    {
        if(    targetIndex < 0
            || targetIndex + _map.Length > targetArray.Length)
        {
            throw new ArgumentException("Invalid copy operation.");
        }

        for(int i=0, tgtIdx = targetIndex; i < _map.Length; i++, tgtIdx++)
        {
            targetArray[tgtIdx] = _innerArr[_map[i]];
        }
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
        if(targetIndex < 0
            || length < 0
            || length > _map.Length
            || targetIndex + length > targetArray.Length)
        {
            throw new ArgumentException("Invalid copy operation.");
        }

        for(int i=0, tgtIdx = targetIndex; i < length; i++, tgtIdx++)
        {
            targetArray[tgtIdx] = _innerArr[_map[i]];
        }
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
        if(targetIndex < 0
            || sourceIndex < 0
            || length < 0
            || targetIndex + length > targetArray.Length
            || sourceIndex + length > _map.Length)
        {
            throw new ArgumentException("Invalid copy operation.");
        }

        for(int i = sourceIndex, tgtIdx = targetIndex; i < sourceIndex+length; i++, tgtIdx++)
        {
            targetArray[tgtIdx] = _innerArr[_map[i]];
        }
    }

    /// <summary>
    /// Copies all elements from the source array writing them into the current vector starting at
    /// <paramref name="targetIndex"/>.
    /// </summary>
    /// <param name="sourceArray">The array to copy elements from.</param>
    /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
    public void CopyFrom(T[] sourceArray, int targetIndex)
    {
        if(targetIndex < 0
            || targetIndex + sourceArray.Length > _map.Length)
        {
            throw new ArgumentException("Invalid copy operation.");
        }

        for(int i=0, tgtIdx = targetIndex; i < sourceArray.Length; i++, tgtIdx++)
        {
            _innerArr[_map[tgtIdx]] = sourceArray[i];
        }
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
        if(targetIndex < 0
            || length < 0
            || length > sourceArray.Length
            || targetIndex + length > _map.Length)
        {
            throw new ArgumentException("Invalid copy operation.");
        }

        for(int i=0, tgtIdx = targetIndex; i < length; i++, tgtIdx++)
        {
            _innerArr[_map[tgtIdx]] = sourceArray[i];
        }
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
        if(sourceIndex < 0
            || targetIndex < 0
            || length < 0
            || sourceIndex + length > sourceArray.Length
            || targetIndex + length > _map.Length)
        {
            throw new ArgumentException("Invalid copy operation.");
        }

        for(int i = sourceIndex, tgtIdx = targetIndex; i < sourceIndex + length; i++, tgtIdx++)
        {
            _innerArr[_map[tgtIdx]] = sourceArray[i];
        }
    }

    /// <summary>
    /// Reset all vector elements to zero.
    /// </summary>
    public void Reset()
    {
        for(int i=0; i < _map.Length; i++)
        {
            _innerArr[_map[i]] = default;
        }
    }

    #endregion

    #region Private Static Methods

    /// <summary>
    /// Validate the indexes within <paramref name="map"/>.
    /// </summary>
    /// <returns>True if the mapping indexes are all valid (i.e., within the indexable range of <paramref name="wrappedArray"/>); otherwise false.</returns>
    private static bool ValidateMapIndexes(T[] wrappedArray, int[] map)
    {
        for(int i=0; i < map.Length; i++)
        {
            if(map[i] < 0 || map[i] >= wrappedArray.Length)
                return false;
        }
        return true;
    }

    #endregion
}
