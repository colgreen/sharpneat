/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
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
    /// <summary>
    /// Conveys fitness information for a genome.
    /// </summary>
    public struct FitnessInfo
    {
        /// <summary>
        /// Default singleton instance.
        /// </summary>
        public static FitnessInfo DefaultFitnessInfo = new FitnessInfo(0.0);

        #region Instance Fields

        readonly double _primaryFitness;

        /// <summary>
        /// An array of auxiliary fitness scores. Most problem tasks will yield just a single fitness value,
        /// here we allow for multiple fitness values per evaluation to allow for multiple objectives, or
        /// secondary fitness scores for reporting only.
        /// </summary>
        readonly double[]? _auxFitnessScores;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct with a single fitness score.
        /// </summary>
        /// <param name="fitness">Genome fitness score.</param>
        public FitnessInfo(double fitness)
        {
            _primaryFitness = fitness;
            _auxFitnessScores = null;
        }

        /// <summary>
        /// Construct with a compound fitness score.
        /// </summary>
        /// <param name="primaryFitness">Primary fitness.</param>
        /// <param name="auxFitnessScores">Auxiliary fitness scores.</param>
        public FitnessInfo(double primaryFitness, double[] auxFitnessScores)
        {
            Debug.Assert(auxFitnessScores.Length > 0);
            _primaryFitness = primaryFitness;
            _auxFitnessScores = auxFitnessScores;
        }

        #endregion

        #region Properties / Indexer

        /// <summary>
        /// Gets the primary fitness score; for most evaluation schemes this is the one and only fitness score.
        /// </summary>
        public double PrimaryFitness => _primaryFitness;

        /// <summary>
        /// Gets an array of auxiliary fitness scores.
        /// </summary>
        public double[]? AuxFitnessScores => _auxFitnessScores;

        #endregion
    }
}
