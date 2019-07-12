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

namespace SharpNeat.IO
{
    /// <summary>
    /// Static utility methods for reading optional json properties.
    /// </summary>
    public static class JsonReadOptionalUtils
    {
        #region Public Static Methods [Read Optional Properties with Setter Action]

        /// <summary>
        /// Read an optional boolean property; if present then the provided setter action is called with the value.
        /// </summary>
        /// <param name="jobj">The json object to read the property from.</param>
        /// <param name="propertyName">The name of the json property to read.</param>
        /// <param name="setter">An action to call the read property value with.</param>
        public static void ReadBoolOptional(
            JObject jobj,
            string propertyName,
            Action<bool> setter)
        {
            bool? val = (bool?)jobj[propertyName];
            if(val.HasValue) {
                setter(val.Value);
            }
        }

        /// <summary>
        /// Read an optional integer property; if present then the provided setter action is called with the value.
        /// </summary>
        /// <param name="jobj">The json object to read the property from.</param>
        /// <param name="propertyName">The name of the json property to read.</param>
        /// <param name="setter">An action to call the read property value with.</param>
        public static void ReadIntOptional(
            JObject jobj,
            string propertyName,
            Action<int> setter)
        {
            int? val = (int?)jobj[propertyName];
            if(val.HasValue) {
                setter(val.Value);
            }
        }

        /// <summary>
        /// Read an optional floating point double property; if present then the provided setter action is called with the value.
        /// </summary>
        /// <param name="jobj">The json object to read the property from.</param>
        /// <param name="propertyName">The name of the json property to read.</param>
        /// <param name="setter">An action to call the read property value with.</param>
        public static void ReadDoubleOptional(
            JObject jobj,
            string propertyName,
            Action<double> setter)
        {
            double? val = (double?)jobj[propertyName];
            if(val.HasValue) {
                setter(val.Value);
            }
        }

        /// <summary>
        /// Read an optional string property; if present then the provided setter action is called with the value.
        /// </summary>
        /// <param name="jobj">The json object to read the property from.</param>
        /// <param name="propertyName">The name of the json property to read.</param>
        /// <param name="setter">An action to call the read property value with.</param>
        public static void ReadStringOptional(
            JObject jobj,
            string propertyName,
            Action<string> setter)
        {
            string val = (string)jobj[propertyName];
            if(!string.IsNullOrEmpty(val)) {
                setter(val);
            }
        }

        #endregion
    }
}
