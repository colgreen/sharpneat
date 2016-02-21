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

namespace SharpNeat.EvolutionAlgorithms.ComplexityRegulation
{
    /// <summary>
    /// Default complexity regulation strategy. 
    /// This strategy provides a choice of hard/absolute comelity ceiling or a ceiling relative to the 
    /// complexity at the end of the most recent simplification phase.
    /// The strategy transitions from complexifying to simplifying when the ceiling is reached. 
    /// Transitioning from simplifying to complexifying occurs when complexity is no longer falling
    /// *and* complexity is below the ceiling. This is determined by tracking a complexity moving average
    /// calculated over the past N generations.
    /// </summary>
    public class DefaultComplexityRegulationStrategy : IComplexityRegulationStrategy
    {
        #region Consts

        /// <summary>
        /// The minimum number of generations we stay within simplifcation mode.
        /// </summary>
        const int MinSimplifcationGenerations = 10;

        #endregion

        #region Instance Fields

        /// <summary>
        /// The ceiling type - absolute or relative.
        /// </summary>
        ComplexityCeilingType _ceilingType;

        /// <summary>
        /// The ceiling value passed into the constructor. Allows the true ceiling to be calculated
        /// if the ceiling type is relative.
        /// </summary>
        double _complexityCeiling;

        /// <summary>
        /// The ceiling point at which we switch to 'simplifying' mode. This value may be fixed 
        /// (absolute ceiling) or may be relative to some other value, e.g. the complexity at the
        /// end of the last simplification phase.
        /// </summary>
        double _complexityCeilingCurrent;

        /// <summary>
        /// The current regulation mode - simplifying or complexifying.
        /// </summary>
        ComplexityRegulationMode _currentMode;

        /// <summary>
        /// The generation at which the last transition occured.
        /// </summary>
        uint _lastTransitionGeneration;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct the complexity regulation strategy with the provided regulation parameters.
        /// </summary>
        public DefaultComplexityRegulationStrategy(ComplexityCeilingType ceilingType, double ceilingValue)
        {
            _ceilingType = ceilingType;
            _complexityCeiling = ceilingValue;

            // For relative complexity ceiling we await the first call to DetermineMode() before setting the threshold
            // relative to the population mean complexity. Indicate this with -1.0.
            if(ComplexityCeilingType.Relative ==  ceilingType) {
                _complexityCeilingCurrent = -1.0;
            } else {
                _complexityCeilingCurrent = ceilingValue;
            }

            _currentMode = ComplexityRegulationMode.Complexifying;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determine which complexity regulation mode the search should be in given the provided
        /// NEAT algorithm stats.
        /// </summary>
        public ComplexityRegulationMode DetermineMode(NeatAlgorithmStats stats)
        {
            if(ComplexityRegulationMode.Complexifying == _currentMode)
            {
                if(-1.0 == _complexityCeilingCurrent)
                {   // First call to DetermineMode(). Continue complexifying and set threshold relative current complexity.
                    _complexityCeilingCurrent = stats._meanComplexity + _complexityCeiling;
                } 
                // Currently complexifying. Test if the complexity ceiling has been reached.
                else if(stats._meanComplexity > _complexityCeilingCurrent)
                {   // Switch to simplifying mode.
                    _currentMode = ComplexityRegulationMode.Simplifying;
                    _lastTransitionGeneration = stats._generation;
                }
            }
            else
            {   // Currently simplifying. Test if simplication (ongoing reduction in complexity) has stalled.
                // We allow simplification to progress for a few generations before testing of it has stalled, this allows
                // a lead in time for the effects of simplification to occur.
                // In addition we do not switch to complexifying if complexity is above the currently defined ceiling.
                if(((stats._generation - _lastTransitionGeneration) > MinSimplifcationGenerations) 
                 && (stats._meanComplexity < _complexityCeilingCurrent)
                 && ((stats._complexityMA.Mean - stats._prevComplexityMA) >= 0.0))
                {   // Simplification has stalled. Switch back to complexification.
                    _currentMode = ComplexityRegulationMode.Complexifying;
                    _lastTransitionGeneration = stats._generation;

                    // Redefine the complexity ceiling (for relative ceiling only).
                    if(ComplexityCeilingType.Relative == _ceilingType) {
                        _complexityCeilingCurrent = stats._meanComplexity + _complexityCeiling;
                    }
                }
            }
            return _currentMode;
        }

        #endregion
    }
}
