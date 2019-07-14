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
    /// Static utility methods for reading mandatory json properties.
    /// </summary>
    public static class JsonReadMandatoryUtils
    {
        #region Public Static Methods [Read Mandatory Properties]

        /// <summary>
        /// Read a mandatory boolean property; if not present then an exception is throw
        /// </summary>
        /// <param name="jobj">The json object to read the property from.</param>
        /// <param name="propertyName">The name of the json property to read.</param>
        /// <returns>The property value</returns>
        public static bool ReadBoolMandatory(
            JObject jobj,
            string propertyName)
        {
            bool? val = (bool?)jobj[propertyName];
            if(val.HasValue) {
                return val.Value;
            }
            throw new Exception($"Missing mandatory property [{propertyName}]");
        }

        /// <summary>
        /// Read a mandatory integer property; if not present then an exception is throw
        /// </summary>
        /// <param name="jobj">The json object to read the property from.</param>
        /// <param name="propertyName">The name of the json property to read.</param>
        /// <returns>The property value</returns>
        public static int ReadIntMandatory(
            JObject jobj,
            string propertyName)
        {
            int? val = (int?)jobj[propertyName];
            if(val.HasValue) {
                return val.Value;
            }
            throw new Exception($"Missing mandatory property [{propertyName}]");
        }

        /// <summary>
        /// Read a mandatory double precision floating point property; if not present then an exception is throw
        /// </summary>
        /// <param name="jobj">The json object to read the property from.</param>
        /// <param name="propertyName">The name of the json property to read.</param>
        /// <returns>The property value</returns>
        public static double ReadDoubleMandatory(
            JObject jobj,
            string propertyName)
        {
            double? val = (double?)jobj[propertyName];
            if(val.HasValue) {
                return val.Value;
            }
            throw new Exception($"Missing mandatory property [{propertyName}]");
        }

        /// <summary>
        /// Read a mandatory string property; if not present then an exception is throw
        /// </summary>
        /// <param name="jobj">The json object to read the property from.</param>
        /// <param name="propertyName">The name of the json property to read.</param>
        /// <returns>The property value</returns>
        public static string ReadStringMandatory(
            JObject jobj,
            string propertyName)
        {
            string val = (string)jobj[propertyName];
            if(val != null) {
                return val;
            }
            throw new Exception($"Missing mandatory property [{propertyName}]");
        }

        #endregion
    }
}
