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
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpNeat.Utility
{
    /// <summary>
    /// General purpose static utility methods.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Randomly shuffles items within a list.
        /// </summary>
        /// <param name="list">The list to shuffle.</param>
        /// <param name="rng">Random number generator.</param>
        public static void Shuffle<T>(IList<T> list, FastRandom rng)
        {
            // This approach was suggested by Jon Skeet in a dotNet newsgroup post and
            // is also the technique used by the OpenJDK. The use of rnd.Next(i+1) introduces
            // the possibility of swapping an item with itself, I suspect the reasoning behind this
            // has to do with ensuring the probability of each possible permutation is approximately equal.
            for (int i=list.Count-1; i>0; i--)
            {
                int swapIndex = rng.Next(i+1);
                T tmp = list[swapIndex];
                list[swapIndex] = list[i];
                list[i] = tmp;
            }
        }

        /// <summary>
        /// Rounds up or down to a whole number by using the fractional part of the input value
        /// as the probability that the value will be rounded up.
        /// 
        /// This is useful if we wish to round values and then sum them without generating a rounding bias.
        /// For monetary rounding this problem is solved with rounding to e.g. the nearest even number which
        /// then causes a bias towards even numbers.
        /// 
        /// This solution is more appropriate for certain types of scientific values.
        /// </summary>
        public static double ProbabilisticRound(double val, FastRandom rng)
        {
            double integerPart = Math.Floor(val);
            double fractionalPart = val - integerPart;
            return rng.NextDouble() < fractionalPart ? integerPart + 1.0  : integerPart;
        }

        /// <summary>
        /// Calculates the median value in a list of sorted values.
        /// </summary>
        public static double CalculateMedian(IList<double> valueList)
        {
            Debug.Assert(valueList.Count != 0 && IsSorted(valueList), "CalculateMedian() requires non-zero length sorted list of values.");

            if(valueList.Count == 1) {
                return valueList[0];
            }

            if(valueList.Count % 2 == 0)
            {   // Even number of values. The values are already sorted so we simply take the
                // mean of the two central values.
                int idx = valueList.Count / 2;
                return (valueList[idx-1] + valueList[idx]) / 2.0;
            }

            // Odd number of values. Return the middle value.
            // (integer division truncates fractional part of result).
            return valueList[valueList.Count/2];
        }

        /// <summary>
        /// Indicates if a list of doubles is sorted into ascending order.
        /// </summary>
        public static bool IsSorted(IList<double> valueList)
        {
            if(0 == valueList.Count) {
                return true;
            }

            double prev = valueList[0];
            int count = valueList.Count;
            for(int i=1; i<count; i++) 
            {
                if(valueList[i] < prev) {
                    return false;
                }
                prev = valueList[i];
            }
            return true;
        }

        /// <summary>
        /// Calculate a frequency distribution for the provided array of values.
        /// 1) The minimum and maximum values are found.
        /// 2) The resulting value range is divided into equal sized sub-ranges (categoryCount).
        /// 3) The number of values that fall into each category is determined.
        /// </summary>
        public static FrequencyDistributionData CalculateDistribution(double[] valArr, int categoryCount)
        {
            // Determine min/max.
            double min = valArr[0];
            double max = min;

            for(int i=1; i<valArr.Length; i++)
            {
                double val = valArr[i];
                if(val < min) {
                    min = val;
                } 
                else if(val > max) {
                    max = val;
                }
            }

            double range = max - min;

            // Handle special case where the data series contains a single value.
            if(0.0 == range) {
                return new FrequencyDistributionData(min, max, 0.0, new int[]{valArr.Length});
            }

            // Loop values and for each one increment the relevant category's frequency count.
            double incr = range / (categoryCount-1);
            int[] frequencyArr = new int[categoryCount];
            for(int i=0; i<valArr.Length; i++) 
            {
                frequencyArr[(int)((valArr[i]-min)/incr)]++;
            }
            return new FrequencyDistributionData(min, max, incr, frequencyArr);
        }



        public static double MagnifyFitnessRange(double x, double metricThreshold, double metricMax, double fitnessThreshold, double fitnessMax)
        {
            if(x < 0.0) {
                x = 0.0;
            }
            else if (x > metricMax) {
                x = metricMax;
            }

            if(x > metricThreshold)
            {   
                return ((x - metricThreshold) / (metricMax - metricThreshold) * (fitnessMax - fitnessThreshold)) + fitnessThreshold;
            }
            // else
            return (x / metricThreshold) * fitnessThreshold;
        }



    }
}
