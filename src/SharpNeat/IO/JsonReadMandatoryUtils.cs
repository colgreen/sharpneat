/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2022 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System.Text.Json;

namespace SharpNeat.IO;

/// <summary>
/// Static utility methods for reading mandatory json properties.
/// </summary>
public static class JsonReadMandatoryUtils
{
    #region Public Static Methods [Read Mandatory Properties]

    /// <summary>
    /// Read a mandatory boolean property; if not present then an exception is thrown.
    /// </summary>
    /// <param name="jelem">The json element to read the property from.</param>
    /// <param name="propertyName">The name of the json property to read.</param>
    /// <returns>The property value.</returns>
    public static bool ReadBoolMandatory(
        JsonElement jelem,
        string propertyName)
    {
        if(jelem.TryGetProperty(propertyName, out JsonElement propElem))
            return propElem.GetBoolean();

        throw new Exception($"Missing mandatory property [{propertyName}]");
    }

    /// <summary>
    /// Read a mandatory integer property; if not present then an exception is thrown.
    /// </summary>
    /// <param name="jelem">The json element to read the property from.</param>
    /// <param name="propertyName">The name of the json property to read.</param>
    /// <returns>The property value.</returns>
    public static int ReadIntMandatory(
        JsonElement jelem,
        string propertyName)
    {
        if(jelem.TryGetProperty(propertyName, out JsonElement propElem))
            return propElem.GetInt32();

        throw new Exception($"Missing mandatory property [{propertyName}]");
    }

    /// <summary>
    /// Read a mandatory double precision floating point property; if not present then an exception is thrown.
    /// </summary>
    /// <param name="jelem">The json element to read the property from.</param>
    /// <param name="propertyName">The name of the json property to read.</param>
    /// <returns>The property value.</returns>
    public static double ReadDoubleMandatory(
        JsonElement jelem,
        string propertyName)
    {
        if(jelem.TryGetProperty(propertyName, out JsonElement propElem))
            return propElem.GetDouble();

        throw new Exception($"Missing mandatory property [{propertyName}]");
    }

    /// <summary>
    /// Read a mandatory string property; if not present then an exception is thrown.
    /// </summary>
    /// <param name="jelem">The json element to read the property from.</param>
    /// <param name="propertyName">The name of the json property to read.</param>
    /// <returns>The property value.</returns>
    public static string ReadStringMandatory(
        JsonElement jelem,
        string propertyName)
    {
        string? str = null;
        if(jelem.TryGetProperty(propertyName, out JsonElement propElem))
            str = propElem.GetString();

        if(str is null) throw new Exception($"Missing mandatory property [{propertyName}]");
        return str;
    }

    #endregion
}
