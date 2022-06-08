// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Neat.ComplexityRegulation;

/// <summary>
/// A complexity regulation strategy that applies a fixed/absolute complexity ceiling.
/// The strategy transitions from complexifying to simplifying when the fixed ceiling is reached.
/// Transitioning from simplifying to complexifying occurs when complexity is no longer falling
/// *and* complexity is below the ceiling.
/// </summary>
public sealed class AbsoluteComplexityRegulationStrategy : IComplexityRegulationStrategy
{
    #region Instance Fields

    /// <summary>
    /// The minimum number of generations we stay within simplification mode.
    /// </summary>
    readonly int _minSimplifcationGenerations;

    /// <summary>
    /// The fixed/absolute complexity ceiling.
    /// </summary>
    readonly double _complexityCeiling;

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
    /// <param name="complexityCeiling">The absolute complexity ceiling.</param>
    /// <param name="minSimplifcationGenerations">The minimum number of generations we stay within simplification mode.</param>
    public AbsoluteComplexityRegulationStrategy(
        int minSimplifcationGenerations,
        double complexityCeiling)
    {
        _minSimplifcationGenerations = minSimplifcationGenerations;
        _complexityCeiling = complexityCeiling;
        _currentMode = ComplexityRegulationMode.Complexifying;
        _lastTransitionGeneration = 0;

        if(minSimplifcationGenerations < 1) throw new ArgumentException("Must be 1 or above.", nameof(minSimplifcationGenerations));
        if(complexityCeiling < 1) throw new ArgumentException("Must be at 1 or above.", nameof(complexityCeiling));
    }

    #endregion

    #region IComplexityRegulationStrategy

    /// <inheritdoc/>
    public ComplexityRegulationMode CurrentMode => _currentMode;

    /// <inheritdoc/>
    public ComplexityRegulationMode UpdateMode(
        EvolutionAlgorithmStatistics eaStats,
        PopulationStatistics popStats)
    {
        return _currentMode switch
        {
            ComplexityRegulationMode.Complexifying => DetermineMode_WhileComplexifying(eaStats, popStats),
            ComplexityRegulationMode.Simplifying => DetermineMode_WhileSimplifying(eaStats, popStats),
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
        if(
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

        return _currentMode;
    }

    #endregion
}
