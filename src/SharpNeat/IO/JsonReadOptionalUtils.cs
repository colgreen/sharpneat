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
using System.Text.Json;

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
        /// <param name="jelem">The json element to read the property from.</param>
        /// <param name="propertyName">The name of the json property to read.</param>
        /// <param name="setter">An action to call the read property value with.</param>
        public static void ReadBoolOptional(
            JsonElement jelem,
            string propertyName,
            Action<bool> setter)
        {
            if(jelem.TryGetProperty(propertyName, out JsonElement propElem)) {
                setter(propElem.GetBoolean());
            }
        }

        /// <summary>
        /// Read an optional integer property; if present then the provided setter action is called with the value.
        /// </summary>
        /// <param name="jelem">The json element to read the property from.</param>
        /// <param name="propertyName">The name of the json property to read.</param>
        /// <param name="setter">An action to call the read property value with.</param>
        public static void ReadIntOptional(
            JsonElement jelem,
            string propertyName,
            Action<int> setter)
        {
            if(jelem.TryGetProperty(propertyName, out JsonElement propElem)) {
                setter(propElem.GetInt32());
            }
        }

        /// <summary>
        /// Read an optional floating point double property; if present then the provided setter action is called with the value.
        /// </summary>
        /// <param name="jelem">The json element to read the property from.</param>
        /// <param name="propertyName">The name of the json property to read.</param>
        /// <param name="setter">An action to call the read property value with.</param>
        public static void ReadDoubleOptional(
            JsonElement jelem,
            string propertyName,
            Action<double> setter)
        {
            if(jelem.TryGetProperty(propertyName, out JsonElement propElem)) {
                setter(propElem.GetDouble());
            }
        }

        /// <summary>
        /// Read an optional string property; if present then the provided setter action is called with the value.
        /// </summary>
        /// <param name="jelem">The json element to read the property from.</param>
        /// <param name="propertyName">The name of the json property to read.</param>
        /// <param name="setter">An action to call the read property value with.</param>
        public static void ReadStringOptional(
            JsonElement jelem,
            string propertyName,
            Action<string> setter)
        {
            if(jelem.TryGetProperty(propertyName, out JsonElement propElem))
            {
                string? str = propElem.GetString();
                if(str is not null) {
                    setter(str);
                }
            }
        }

        #endregion
    }
}
