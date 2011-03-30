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
using System.Threading.Tasks;

namespace SharpNeat.Utility
{
    /// <summary>
    /// Parallel quicksort algorithm.
    /// </summary>
    public static class ParallelSort
    {
        #region Public Static Methods

        /// <summary>
        /// Sequential quicksort.
        /// </summary>
        public static void QuicksortSequential<T>(T[] arr) where T : IComparable<T>
        {
            QuicksortSequential(arr, 0, arr.Length - 1);
        }

        /// <summary>
        /// Parallel quicksort
        /// </summary>
        public static void QuicksortParallel<T>(T[] arr) where T : IComparable<T>
        {
            QuicksortParallel(arr, 0, arr.Length - 1);
        }

        #endregion

        #region Private Static Methods

        private static void QuicksortSequential<T>(T[] arr, int left, int right) 
            where T : IComparable<T>
        {
            if (right > left)
            {
                int pivot = Partition(arr, left, right);
                QuicksortSequential(arr, left, pivot - 1);
                QuicksortSequential(arr, pivot + 1, right);
            }
        }

        private static void QuicksortParallel<T>(T[] arr, int left, int right) 
            where T : IComparable<T>
        {
            const int SequentialThreshold = 2048;
            if (right > left)
            {
                if (right - left < SequentialThreshold)
                {
                    QuicksortSequential(arr, left, right);
                }
                else
                {
                    int pivot = Partition(arr, left, right);
                    Parallel.Invoke(new Action[] { delegate {QuicksortParallel(arr, left, pivot - 1); },
                                                   delegate {QuicksortParallel(arr, pivot + 1, right); }
                    });
                }
            }
        }

        private static void Swap<T>(T[] arr, int i, int j)
        {
            T tmp = arr[i];
            arr[i] = arr[j];
            arr[j] = tmp;
        }

        private static int Partition<T>(T[] arr, int low, int high) 
            where T : IComparable<T>
        {
            // Simple partitioning implementation
            int pivotPos = (high + low) / 2;
            T pivot = arr[pivotPos];
            Swap(arr, low, pivotPos);

            int left = low;
            for (int i = low + 1; i <= high; i++)
            {
                if (arr[i].CompareTo(pivot) < 0)
                {
                    left++;
                    Swap(arr, i, left);
                }
            }

            Swap(arr, low, left);
            return left;
        }

        #endregion
    }
}
