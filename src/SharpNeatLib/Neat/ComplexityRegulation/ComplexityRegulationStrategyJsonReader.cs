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
using Newtonsoft.Json.Linq;
using static SharpNeat.IO.JsonReadMandatoryUtils;

namespace SharpNeat.Neat.ComplexityRegulation
{
    /// <summary>
    /// Static utility methods for creating insstances of <see cref="IComplexityRegulationStrategy"/> from json configuration.
    /// </summary>
    public static class ComplexityRegulationStrategyJsonReader
    {
        #region Public Static Methods

        /// <summary>
        /// Create a new instance of <see cref="IComplexityRegulationStrategy"/> based on the provided json configuration.
        /// </summary>
        /// <param name="jobj">The json object to read from.</param>
        /// <returns>A new instance of <see cref="IComplexityRegulationStrategy"/>.</returns>
        public static IComplexityRegulationStrategy Read(JObject jobj)
        {
            string strategyName = (string)jobj["strategyName"];
            switch(strategyName)
            {
                case "null":
                    // The 'null' strategy has been explicitly defined.
                    return new NullComplexityRegulationStrategy();

                case "absolute":
                    return ReadAbsoluteComplexityRegulationStrategy(jobj);

                case "relative":
                    return ReadRelativeComplexityRegulationStrategy(jobj);

                default:
                    throw new Exception($"Unsupported complexity regulation strategyName [{strategyName}]");
            }
        }

        #endregion

        #region Private Static Methods

        private static AbsoluteComplexityRegulationStrategy ReadAbsoluteComplexityRegulationStrategy(JObject jobj)
        {
            int complexityCeiling = ReadIntMandatory(jobj, "complexityCeiling");
            int minSimplifcationGenerations = ReadIntMandatory(jobj, "minSimplifcationGenerations");
            return new AbsoluteComplexityRegulationStrategy(minSimplifcationGenerations, complexityCeiling);
        }

        private static RelativeComplexityRegulationStrategy ReadRelativeComplexityRegulationStrategy(JObject jobj)
        {
            int relativeComplexityCeiling = ReadIntMandatory(jobj, "relativeComplexityCeiling");
            int minSimplifcationGenerations = ReadIntMandatory(jobj, "minSimplifcationGenerations");
            return new RelativeComplexityRegulationStrategy(minSimplifcationGenerations, relativeComplexityCeiling);
        }

        #endregion
    }
}
