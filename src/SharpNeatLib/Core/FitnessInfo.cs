/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
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