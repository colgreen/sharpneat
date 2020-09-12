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
using System.Text.Json;
using static SharpNeat.IO.JsonReadOptionalUtils;

namespace SharpNeat.Neat.Reproduction.Sexual
{
    /// <summary>
    /// Static utility methods for reading <see cref="NeatReproductionSexualSettings"/> from json.
    /// </summary>
    public static class NeatReproductionSexualSettingsJsonReader
    {
        /// <summary>
        /// Read json into a target instance of <see cref="NeatReproductionSexualSettings"/>.
        /// Settings that are present are read and set on the target settings object; all other settings
        /// remain unchanged on the target object.
        /// </summary>
        /// <param name="target">The target settings object to store the read values on.</param>
        /// <param name="jelem">The json element to read from.</param>
        public static void Read(
            NeatReproductionSexualSettings target,
            JsonElement jelem)
        {
            ReadDoubleOptional(jelem, "secondaryParentGeneProbability", x => target.SecondaryParentGeneProbability = x);
            target.Validate();
        }
    }
}
