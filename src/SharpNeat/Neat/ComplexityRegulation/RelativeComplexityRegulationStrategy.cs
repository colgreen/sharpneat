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
using System;
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Neat.ComplexityRegulation
{
    /// <summary>
    /// A complexity regulation strategy that applies a moving complexity ceiling that is relative
    /// to the population mean complexity each at the start of each transition to complexifying mode.
    ///
    /// The strategy transitions from complexifying to simplifying when the relative ceiling is reached.
    /// Transitioning from simplifying to complexifying occurs when complexity is no longer falling
    /// *and* complexity is below the ceiling.
    /// </summary>
    public sealed class RelativeComplexityRegulationStrategy : IComplexityRegulationStrategy
    {
        #region Instance Fields

        /// <summary>
        /// The minimum number of generations we stay within simplification mode.
        /// </summary>
        readonly int _minSimplifcationGenerations;

        /// <summary>
        /// The relative complexity ceiling.
        /// </summary>
        readonly double _relativeComplexityCeiling;

        /// <summary>
        /// The running/moving complexity ceiling.
        /// </summary>
        double _complexityCeiling;

        /// <summary>
        /// The current regulation mode - simplifying or complexifying.
        /// </summary>
        ComplexityRegulationMode _currentMode;

        /// <summary>
        /// The generation at which the last transition occurred.
        /// </summary>
        int _lastTransitionGeneration;

        /// <summary>
        /// Recorded value of popStats.MeanComplexityHistory.Mean from the previous generation.
        /// </summary>
        double _prevMeanMovingAverage;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="relativeComplexityCeiling">The relative complexity ceiling.</param>
        /// <param name="minSimplifcationGenerations">The minimum number of generations we stay within simplification mode.</param>
        public RelativeComplexityRegulationStrategy(
            int minSimplifcationGenerations,
            double relativeComplexityCeiling)
        {
            _minSimplifcationGenerations = minSimplifcationGenerations;
            _relativeComplexityCeiling = relativeComplexityCeiling;
            _complexityCeiling = relativeComplexityCeiling;
            _currentMode = ComplexityRegulationMode.Complexifying;
            _lastTransitionGeneration = 0;

            if(minSimplifcationGenerations < 1) throw new ArgumentException(nameof(minSimplifcationGenerations));
            if(relativeComplexityCeiling < 1) throw new ArgumentException(nameof(relativeComplexityCeiling));
        }

        #endregion

        #region IComplexityRegulationStrategy

        /// <summary>
        /// Gets the current complexity regulation mode.
        /// </summary>
        public ComplexityRegulationMode CurrentMode => _currentMode;

        /// <summary>
        /// Determine the complexity regulation mode that the evolution algorithm search should be in given the
        /// provided evolution algorithm statistics object, and set the current mode to that mode.
        /// </summary>
        /// <param name="eaStats">Evolution algorithm statistics.</param>
        /// <param name="popStats">Population statistics.</param>
        /// <returns>The determined <see cref="ComplexityRegulationMode"/>.</returns>
        public ComplexityRegulationMode UpdateMode(
            EvolutionAlgorithmStatistics eaStats,
            PopulationStatistics popStats)
        {
            return _currentMode switch
            {
                ComplexityRegulationMode.Complexifying => DetermineMode_WhileComplexifying(eaStats,popStats),
                ComplexityRegulationMode.Simplifying => DetermineMode_WhileSimplifying(eaStats,popStats),
                _ => throw new InvalidOperationException("Unexpected complexity regulation mode."),
            };
        }

        #endregion

        #region Private Methods

        private ComplexityRegulationMode DetermineMode_WhileComplexifying(
            EvolutionAlgorithmStatistics eaStats,
            PopulationStatistics popStats)
        {
            // Currently complexifying.
            // Test if the complexity ceiling has been reached.
            if(popStats.MeanComplexity > _complexityCeiling)
            {
                // Switch to simplifying mode.
                _currentMode = ComplexityRegulationMode.Simplifying;
                _lastTransitionGeneration = eaStats.Generation;
                _prevMeanMovingAverage = popStats.MeanComplexityHistory.Mean;
            }

            return _currentMode;
        }

        private ComplexityRegulationMode DetermineMode_WhileSimplifying(
            EvolutionAlgorithmStatistics eaStats,
            PopulationStatistics popStats)
        {
            // Currently simplifying.
            // Test if simplification (ongoing reduction in complexity) has stalled.

            // We allow simplification to progress for a few generations before testing of it has stalled, this allows
            // a lead in time for the effects of simplification to occur.
            // In addition we do not switch to complexifying if complexity is above the currently defined ceiling.
            if (
                ((eaStats.Generation - _lastTransitionGeneration) > _minSimplifcationGenerations)
                && (popStats.MeanComplexity < _complexityCeiling)
                && ((popStats.MeanComplexityHistory.Mean - _prevMeanMovingAverage) >= 0.0))
            {
                // Simplification has stalled; switch back to complexification.
                _currentMode = ComplexityRegulationMode.Complexifying;
                _lastTransitionGeneration = eaStats.Generation;
                _prevMeanMovingAverage = 0.0;
            }
            // else: otherwise remain in simplifying mode.

            // Update previous mean moving average complexity value.
            _prevMeanMovingAverage = popStats.MeanComplexityHistory.Mean;

            // Set a new complexity ceiling, relative to the current population complexity mean.
            _complexityCeiling = popStats.MeanComplexity + _relativeComplexityCeiling;

            return _currentMode;
        }

        #endregion
    }
}
