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

namespace SharpNeat.Neat.Reproduction.Asexual
{
    /// <summary>
    /// Static utility methods for reading <see cref="NeatReproductionAsexualSettings"/> from json.
    /// </summary>
    public static class NeatReproductionAsexualSettingsJsonReader
    {
        /// <summary>
        /// Read json into a target instance of <see cref="NeatReproductionAsexualSettings"/>.
        /// Settings that are present are read and set on the target settings object; all other settings
        /// remain unchanged on the target object.
        /// </summary>
        /// <param name="target">The target settings object to store the read values on.</param>
        /// <param name="jobj">The json object to read from.</param>
        public static void Read(
            NeatReproductionAsexualSettings target,
            JObject jobj)
        {
            ReadDoubleOptional(jobj, "connectionWeightMutationProbability", x => target.ConnectionWeightMutationProbability = x);
            ReadDoubleOptional(jobj, "addNodeMutationProbability", x => target.AddNodeMutationProbability = x);
            ReadDoubleOptional(jobj, "addConnectionMutationProbability", x => target.AddConnectionMutationProbability = x);
            ReadDoubleOptional(jobj, "deleteConnectionMutationProbability", x => target.DeleteConnectionMutationProbability = x);
        }
    }
}
