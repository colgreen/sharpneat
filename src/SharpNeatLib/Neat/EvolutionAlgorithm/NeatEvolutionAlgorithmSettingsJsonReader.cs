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
using Newtonsoft.Json.Linq;
using static SharpNeat.IO.JsonReadOptionalUtils;

namespace SharpNeat.Neat.EvolutionAlgorithm
{
    /// <summary>
    /// Static utility methods for reading <see cref="NeatEvolutionAlgorithmSettings"/> from json.
    /// </summary>
    public static class NeatEvolutionAlgorithmSettingsJsonReader
    {
        /// <summary>
        /// Read json into a target instance of <see cref="NeatEvolutionAlgorithmSettings"/>.
        /// Settings that are present are read and set on the target settings object; all other settings
        /// remain unchanged on the target object.
        /// </summary>
        /// <param name="target">The target settings object to store the read values on.</param>
        /// <param name="jobj">The json object to read from.</param>
        public static void Read(
            NeatEvolutionAlgorithmSettings target,
            JObject jobj)
        {
            ReadIntOptional(jobj, "speciesCount", x => target.SpeciesCount = x);
            ReadDoubleOptional(jobj, "elitismProportion", x => target.ElitismProportion = x);
            ReadDoubleOptional(jobj, "selectionProportion", x => target.SelectionProportion = x);
            ReadDoubleOptional(jobj, "offspringAsexualProportion", x => target.OffspringAsexualProportion = x);
            ReadDoubleOptional(jobj, "offspringSexualProportion", x => target.OffspringSexualProportion = x);
            ReadDoubleOptional(jobj, "interspeciesMatingProportion", x => target.InterspeciesMatingProportion = x);
            ReadIntOptional(jobj, "statisticsMovingAverageHistoryLength", x => target.StatisticsMovingAverageHistoryLength = x);
        }
    }
}
