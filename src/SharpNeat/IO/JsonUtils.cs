// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Text.Json;

namespace SharpNeat.IO;

/// <summary>
/// Static utility methods for loading JSON.
/// </summary>
public static class JsonUtils
{
    private static readonly JsonSerializerOptions __jsonOptions =
        new()
        {
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

    /// <summary>
    /// Read JSON from the provided file, and deserialize into type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="filename">The name of the filename to read the JSOn from.</param>
    /// <returns>A new instance of T.</returns>
    public static T DeserializeFromFile<T>(string filename)
        where T : class
    {
        using FileStream fs = File.OpenRead(filename);
        T? obj = JsonSerializer.Deserialize<T>(fs, __jsonOptions);
        if(obj is null) throw new IOException($"The file '{filename}' does not contain a JSON object.");
        return obj;
    }

    /// <summary>
    /// Read JSON from the provided stream, and deserialize into type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="utf8Json">An input stream from which json can be read from.</param>
    /// <returns>A new instance of T.</returns>
    public static T Deserialize<T>(Stream utf8Json)
        where T : class
    {
        T? obj = JsonSerializer.Deserialize<T>(utf8Json, __jsonOptions);
        if(obj is null) throw new IOException($"The stream does not contain a JSON object.");
        return obj;
    }

    /// <summary>
    /// Read JSON from the provided string, and deserialize into type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="json">A string containing json.</param>
    /// <returns>A new instance of T.</returns>
    public static T Deserialize<T>(string json)
        where T : class
    {
        T? obj = JsonSerializer.Deserialize<T>(json, __jsonOptions);
        if(obj is null) throw new IOException($"The string does not contain a JSON object.");
        return obj;
    }
}
