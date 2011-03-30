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
    /// Represents the layout of a roulette wheel where each sector has different degrees of arc and thus
    /// probability of being selected.
    /// For use by the RouletteWheel class.
    /// </summary>
    public class RouletteWheelLayout
    {
        readonly double[] _probabilities;

        #region Constructor

        /// <summary>
        /// Construct the layout with provided probabilities. The provided probabilites do not have to add 
        /// up to 1.0 as we normalise them so that their total equals 1.0. In that sense they are not true
        /// probabilities until they have been normalised.
        /// </summary>
        public RouletteWheelLayout(double[] probabilities)
        {
            // Normalise provided values so that all probabilities add up to 1.0.
            double total = 0;
            for(int i=0; i<probabilities.Length; i++) {
                total += probabilities[i];
            }

            // Handle special case where all provided probabilities are zero. In that case we evenly 
            // assign probabilities across all choices.
            if(0.0 == total) 
            {
                double prob = 1.0/probabilities.Length;
                for(int i=0; i<probabilities.Length; i++) {
                    probabilities[i] = prob;
                }
            }
            else
            {
                double total2 = 0;
                double factor = 1.0 / total;
                for(int i=0; i<probabilities.Length; i++) 
                {
                    probabilities[i] = probabilities[i] * factor;
                    total2 +=  probabilities[i];
                }

                // Check that the probabilities add up to about 1. If not then we give up and assign
                // the probabilities evenly across all choices. This can happen in some pathological cases
                // as the result of floating point arithmetic .
                if(Math.Abs(1.0 - total2) > 0.02)
                {
                    double prob = 1.0/probabilities.Length;
                    for(int i=0; i<probabilities.Length; i++) {
                        probabilities[i] = prob;
                    }
                }
            }
            _probabilities = probabilities;
        }

        /// <summary>
        /// Copy constructor. Avoids normalisation of probabilities as this has already been done.
        /// </summary>
        public RouletteWheelLayout(RouletteWheelLayout copyFrom)
        {
            _probabilities = copyFrom._probabilities;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the array of probabilities.
        /// </summary>
        public double[] Probabilities
        {
            get { return _probabilities; }
        }

        #endregion

    }
}
