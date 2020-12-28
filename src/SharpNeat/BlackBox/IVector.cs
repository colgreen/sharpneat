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
namespace SharpNeat.BlackBox
{
    // TODO: Consider if this can be replaced by Memory<T> and Span<T> once those types are fully supported, e.g. in dotnet core 3 and dotnet framework 4.8 (?)

    /// <summary>
    /// A vector of element of type T.
    /// </summary>
    /// <typeparam name="T">Vector data type.</typeparam>
    public interface IVector<T> where T : struct
    {
        #region Indexer / Properties

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index of the element to get or set.</param>
        T this[int index] { get; set; }

        /// <summary>
        /// Gets the length of the vector.
        /// </summary>
        int Length { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Copies all elements from the current vector to the specified target array, starting
        /// at the specified target array index.
        /// </summary>
        /// <param name="targetArray">The target array to copy elements in to.</param>
        /// <param name="targetIndex">The index in <paramref name="targetArray"/> at which copying to begins.</param>
        void CopyTo(T[] targetArray, int targetIndex);

        /// <summary>
        /// Copies <paramref name="length"/> elements from the current vector to the specified target
        /// array, starting at the specified target array index.
        /// </summary>
        /// <param name="targetArray">The target array to copy elements in to.</param>
        /// <param name="targetIndex">The index in <paramref name="targetArray"/> at which copying to begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        void CopyTo(T[] targetArray, int targetIndex, int length);

        /// <summary>
        /// Copies <paramref name="length"/> elements from the current vector to the specified target
        /// array, starting from <paramref name="targetIndex"/> on the target array and
        /// <paramref name="sourceIndex"/> in the current vector..
        /// </summary>
        /// <param name="targetArray">The target array to copy elements in to.</param>
        /// <param name="targetIndex">The index in <paramref name="targetArray"/> at which copying to begins.</param>
        /// <param name="sourceIndex">The index into the current vector  at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        void CopyTo(T[] targetArray, int targetIndex, int sourceIndex, int length);

        /// <summary>
        /// Copies all elements from the source array writing them into the current vector starting at
        /// <paramref name="targetIndex"/>.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        void CopyFrom(T[] sourceArray, int targetIndex);

        /// <summary>
        /// Copies <paramref name="length"/> elements from the source array writing them to the current vector
        /// starting at <paramref name="targetIndex"/>.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        void CopyFrom(T[] sourceArray, int targetIndex, int length);

        /// <summary>
        /// Copies <paramref name="length"/> elements starting from <paramref name="sourceIndex"/> in
        /// <paramref name="sourceArray"/>, to the current vector, starting at <paramref name="targetIndex"/>.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="sourceIndex">The sourceArray index at which copying begins.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        void CopyFrom(T[] sourceArray, int sourceIndex, int targetIndex, int length);

        /// <summary>
        /// Reset all array elements to some default value (typically zero).
        /// </summary>
        void Reset();

        #endregion
    }
}