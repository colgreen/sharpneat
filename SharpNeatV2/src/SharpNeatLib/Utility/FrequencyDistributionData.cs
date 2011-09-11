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
    /// Frequency distribution data from a distribution analysis of some data series.
    /// </summary>
    public class FrequencyDistributionData
    {
        double _min;
        double _max;
        double _incr;
        int[] _frequencyArr;

        #region Constructor

        /// <summary>
        /// Construct with the provided frequency distribution data.
        /// </summary>
        /// <param name="min">The minimum value in the data series the distribution represents.</param>
        /// <param name="max">The maximum value in the data series the distribution represents.</param>
        /// <param name="increment">The range of a single category bucket.</param>
        /// <param name="frequencyArr">The array of category frequency counts.</param>
        public FrequencyDistributionData(double min, double max, double increment, int[] frequencyArr)
        {
            _min = min;
            _max = max;
            _incr = increment;
            _frequencyArr = frequencyArr;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The minimum value in the data series the distribution represents.
        /// </summary>
        public double Min
        {
            get { return _min; }
        }

        /// <summary>
        /// The maximum value in the data series the distribution represents.
        /// </summary>
        public double Max
        {
            get { return _max; }
        }

        /// <summary>
        /// The range of a single category bucket.
        /// </summary>
        public double Increment
        {
            get { return _incr; }
        }

        /// <summary>
        /// The array of category frequency counts.
        /// </summary>
        public int[] FrequencyArray
        {
            get { return _frequencyArr; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the index of the bucket that covers the specified x value. Throws an exception if x is 
        /// outside the range of represented by the distribution buckets.
        /// </summary>
        public int GetBucketIndex(double x)
        {
            if(x < _min || x > _max) {
                throw new ApplicationException("x is outide the range represented by the distribution data.");
            }
            return (int)((x- _min) / _incr);
        }

        #endregion
    }
}
