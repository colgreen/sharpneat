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

namespace SharpNeat.Core
{
    /// <summary>
    /// Wrapper struct for fitness values.
    /// </summary>
    public struct FitnessInfo
    {
        /// <summary>
        /// Preconstructed FitnessInfo for common case of representing zero fitness.
        /// </summary>
        public static FitnessInfo Zero = new FitnessInfo(0.0, 0.0);

        /// <summary>
        /// Fitness score.
        /// </summary>
        public double _fitness;

        /// <summary>
        /// Auxiliary fitness info, i.e. for evaluation metrics other than the
        /// primary fitness metric but that nonetheless we are interested in observing.
        /// </summary>
        public AuxFitnessInfo[] _auxFitnessArr;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FitnessInfo(double fitness, double alternativeFitness)
        {
            _fitness = fitness;
            _auxFitnessArr = new AuxFitnessInfo[] {new AuxFitnessInfo("Alternative Fitness", alternativeFitness)};
        }

        /// <summary>
        /// Construct with the provided fitness value and auxiliary fitness info.
        /// </summary>
        public FitnessInfo(double fitness, AuxFitnessInfo[] auxFitnessArr)
        {
            _fitness = fitness;
            _auxFitnessArr = auxFitnessArr;
        }
    }
}