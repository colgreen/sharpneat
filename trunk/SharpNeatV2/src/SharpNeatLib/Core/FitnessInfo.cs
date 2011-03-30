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

namespace SharpNeat.Core
{
    /// <summary>
    /// Wrapper struct for fitness values.
    /// </summary>
    public struct FitnessInfo
    {
        /// <summary>
        /// Precosntructed FitnessInfo for commen case of representing zero fitness.
        /// </summary>
        public static FitnessInfo Zero = new FitnessInfo(0.0, 0.0);

        /// <summary>
        /// Fitness score.
        /// </summary>
        public double _fitness;
        /// <summary>
        /// Alternative fitness score. This value is provided to allow evaulators to report a number that is 
        /// more meaningful to humans, it is not used by the evolutionary algorithm in any way. The idea here is that fitness
        /// functions often apply complex traformations to one or more underlying fitness values to obtain a value with a number 
        /// of attributes that are desirable in fitenss functions (e.g. smooth fitness landscape). In applying those transformations
        /// the end fitness value can become hard to interpret directly, as such this value can be used to provide some meaningful
        /// underlying fitness value.
        /// </summary>
        public double _alternativeFitness;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FitnessInfo(double fitness, double alternativeFitness)
        {
            _fitness = fitness;
            _alternativeFitness = alternativeFitness;
        }
    }
}