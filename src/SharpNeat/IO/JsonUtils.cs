// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
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
