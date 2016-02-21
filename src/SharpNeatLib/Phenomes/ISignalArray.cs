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
namespace SharpNeat.Phenomes
{
    /// <summary>
    /// An abstraction of the SignalArray class. Provided to allow custom implementations of a signal array
    /// if required.
    /// </summary>
    public interface ISignalArray
    {
        #region Indexer / Properties

        /// <summary>
        /// Gets or sets the signal value at the specified index.
        /// </summary>
        double this[int index]
        {
            get; set;
        }
        
        /// <summary>
        /// Gets the length of the signal array.
        /// </summary>
        int Length
        {
            get;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Copies all elements from the current SignalArray to the specified target array starting 
        /// at the specified target Array index. 
        /// </summary>
        /// <param name="targetArray">The array to copy elements to.</param>
        /// <param name="targetIndex">The targetArray index at which copying to begins.</param>
        void CopyTo(double[] targetArray, int targetIndex);
        
        /// <summary>
        /// Copies <paramref name="length"/> elements from the current SignalArray to the specified target
        /// array starting at the specified target Array index. 
        /// </summary>
        /// <param name="targetArray">The array to copy elements to.</param>
        /// <param name="targetIndex">The targetArray index at which storing begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        void CopyTo(double[] targetArray, int targetIndex, int length);

        /// <summary>
        /// Copies <paramref name="length"/> elements from the current SignalArray to the specified target
        /// starting from <paramref name="targetIndex"/> on the target array and <paramref name="sourceIndex"/>
        /// on the current source SignalArray.
        /// </summary>
        /// <param name="targetArray">The array to copy elements to.</param>
        /// <param name="targetIndex">The targetArray index at which copying begins.</param>
        /// <param name="sourceIndex">The index into the current SignalArray at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        void CopyTo(double[] targetArray, int targetIndex, int sourceIndex, int length);

        /// <summary>
        /// Copies all elements from the source array writing them into the current SignalArray starting
        /// at the specified targetIndex.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        void CopyFrom(double[] sourceArray, int targetIndex);

        /// <summary>
        /// Copies <paramref name="length"/> elements from the source array writing them to the current SignalArray 
        /// starting at the specified targetIndex.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        void CopyFrom(double[] sourceArray, int targetIndex, int length);

        /// <summary>
        /// Copies <paramref name="length"/> elements starting from sourceIndex on sourceArray to the current
        /// SignalArray starting at the specified targetIndex.
        /// </summary>
        /// <param name="sourceArray">The array to copy elements from.</param>
        /// <param name="sourceIndex">The sourceArray index at which copying begins.</param>
        /// <param name="targetIndex">The index into the current SignalArray at which copying begins.</param>
        /// <param name="length">The number of elements to copy.</param>
        void CopyFrom(double[] sourceArray, int sourceIndex, int targetIndex, int length);

        /// <summary>
        /// Reset all array elements to zero.
        /// </summary>
        void Reset();

        #endregion
    }
}