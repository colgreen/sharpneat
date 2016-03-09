/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
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

namespace SharpNeat.Utility
{
    /// <summary>
    /// General purpose static utility methods.
    /// </summary>
    public static class Utilities
    {
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
