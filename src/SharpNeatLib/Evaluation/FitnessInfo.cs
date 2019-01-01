/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2019 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */

using System.Diagnostics;

namespace SharpNeat.Evaluation
{
    public struct FitnessInfo
    {
        public static FitnessInfo DefaultFitnessInfo = new FitnessInfo(0.0);

        #region Instance Fields

        /// <summary>
        /// An array of fitness scores. Most problem tasks will yield just a single fitness value, here we allow for 
        /// multiple fitness values per evaluation to allow for multiple objectives, or secondary 
        /// fitness scores for reporting only.
        /// </summary>
        double[] _fitnessScores;

        #endregion

        #region Constructors

        public FitnessInfo(double fitness)
        {
            _fitnessScores = new double[] { fitness };
        }

        public FitnessInfo(double[] fitnessScores)
        {
            Debug.Assert(fitnessScores.Length > 0);
            _fitnessScores = fitnessScores;
        }

        #endregion

        #region Properties / Indexer

        /// <summary>
        /// Get/set the i'th fitness score.
        /// </summary>
        public double this[int idx]
        {
            get => _fitnessScores[idx];
            set => _fitnessScores[idx] = value;
        }

        /// <summary>
        /// Gets the primary fitness score; for most evaluation schemes this is the one and only fitness score.
        /// </summary>
        public double PrimaryFitness => _fitnessScores[0];

        #endregion
    }
}
