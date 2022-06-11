// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Text;
using SharpNeat.IO.Models;

namespace SharpNeat.IO;

/// <summary>
/// Static methods for loading and saving instances of <see cref="NetFileModel"/> from and to a file.
/// </summary>
public static class NetFile
{
    /// <summary>
    /// Load a 'net' format file.
    /// </summary>
    /// <param name="filename">The name and path of the file to lead.</param>
    /// <returns>A new instance of <see cref="NetFileModel"/>.</returns>
    public static NetFileModel Load(string filename)
    {
        ArgumentNullException.ThrowIfNull(filename);
        using var sr = new StreamReader(filename);
        NetFileModel netFileModel = NetFileLoader.Load(sr);
        return netFileModel;
    }

    /// <summary>
    /// Load a 'net' format file.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <returns>A new instance of <see cref="NetFileModel"/>.</returns>
    public static NetFileModel Load(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        using var sr = new StreamReader(stream, Encoding.UTF8, true, 1024, true);
        NetFileModel netFileModel = NetFileLoader.Load(sr);
        return netFileModel;
    }
}
