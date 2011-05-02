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
    // TODO: Use faster Ziggurat algorithm (http://en.wikipedia.org/wiki/Ziggurat_algorithm)
    /// <summary>
    /// Source of random values sample from a Gaussian distribution. Uses the polar form of the Box-Muller method.
    /// http://en.wikipedia.org/wiki/Box_Muller_transform
    /// </summary>
    public class GaussianGenerator
    {
        FastRandom _rng = new FastRandom();
        double? _spareValue = null;

        /// <summary>
        /// Get the next sample point from the gaussian distribution.
        /// </summary>
        public double NextDouble()
        {
            if(null != _spareValue)
            {
                double tmp = _spareValue.Value;
                _spareValue = null;
                return tmp;
            }

            // Generate two new gaussian values.
            double x, y, sqr;

            // We need a non-zero random point inside the unit circle.
            do
            {
                x = 2.0 * _rng.NextDouble() - 1.0;
                y = 2.0 * _rng.NextDouble() - 1.0;
                sqr = x * x + y * y;
            }
            while(sqr > 1.0 || sqr == 0);

            // Make the Box-Muller transformation.
            double fac = Math.Sqrt(-2.0 * Math.Log(sqr) / sqr);

            _spareValue = x * fac;
            return y * fac;
        }

        /// <summary>
        /// Get the next sample point from the gaussian distribution.
        /// </summary>
        public double NextDouble(double mu, double sigma)
        {
            return mu + (NextDouble() * sigma);
        }
    }
}
