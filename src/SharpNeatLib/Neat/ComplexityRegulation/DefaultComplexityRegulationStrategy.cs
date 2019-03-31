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
using System;
using System.Collections.Generic;
using System.Text;
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Neat.ComplexityRegulation
{
    public class DefaultComplexityRegulationStrategy : IComplexityRegulationStrategy
    {
        #region Consts

        // TODO: make this configurable.
        /// <summary>
        /// The minimum number of generations we stay within simplification mode.
        /// </summary>
        const int __MinSimplifcationGenerations = 10;

        #endregion

        #region Instance Fields

        /// <summary>
        /// The ceiling type - absolute or relative.
        /// </summary>
        ComplexityCeilingType _ceilingType;

        /// <summary>
        /// The complexity ceiling value passed into the constructor. 
        /// This is required for <see cref="ComplexityCeilingType.Absolute"/> only.
        /// </summary>
        double _complexityCeiling;

        /// <summary>
        /// The complexity level at which the strategy will switch to simplifying mode. 
        /// This value may be fixed (absolute ceiling) or may be relative to some other value, e.g. the complexity at the
        /// end of the last simplification phase.
        /// </summary>
        double _complexityCeilingCurrent;

        /// <summary>
        /// The current regulation mode - simplifying or complexifying.
        /// </summary>
        ComplexityRegulationMode _currentMode;

        /// <summary>
        /// The evolution algorithm generation at which the last regulation mode transition occurred.
        /// </summary>
        uint _lastTransitionGeneration;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new instance with the provided complexity regulation strategy arguments.
        /// </summary>
        public DefaultComplexityRegulationStrategy(ComplexityCeilingType ceilingType, double ceilingValue)
        {
            _ceilingType = ceilingType;
            _complexityCeiling = ceilingValue;

            // For relative complexity ceiling we await the first call to DetermineMode() before setting the threshold
            // relative to the population mean complexity. Indicate this by assigning a negative value.
            if(ComplexityCeilingType.Relative ==  ceilingType) {
                _complexityCeilingCurrent = -1.0;
            } 
            else {
                _complexityCeilingCurrent = ceilingValue;
            }

            _currentMode = ComplexityRegulationMode.Complexifying;
        }

        #endregion

        #region IComplexityRegulationStrategy

        public ComplexityRegulationMode DetermineMode(EvolutionAlgorithmStatistics eaStats)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
