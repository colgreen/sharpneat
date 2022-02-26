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

namespace SharpNeat.IO;

/// <summary>
/// Static utility methods for JSON parsing.
/// </summary>
public static class JsonUtils
{
    /// <summary>
    /// Load a UTF8 encoded json file.
    /// </summary>
    /// <param name="filename">The name of the file to load.</param>
    /// <returns>A new instance of <see cref="JsonDocument"/>.</returns>
    public static JsonDocument LoadUtf8(string filename)
    {
        using FileStream fs = File.OpenRead(filename);
        return JsonDocument.Parse(fs);
    }
}
