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

namespace SharpNeat.IO
{
    /// <summary>
    /// Static utility methods for JSON parsing.
    /// </summary>
    public static class JsonUtils
    {
        /// <summary>
        /// Load a Newtonsoft.Json.Linq.JObject from a string that contains JSON.
        /// </summary>
        /// <param name="jsonString">The JSON string to parse.</param>
        /// <returns>A <see cref="JObject"/> that conveys the parsed json data; or null if <paramref name="jsonString"/> was null, the empty string or a whitespace only string.</returns>
        public static JObject Parse(string jsonString)
        {
            if(string.IsNullOrWhiteSpace(jsonString)) {
                return null;
            }
            return JObject.Parse(jsonString);
        }
    }
}
