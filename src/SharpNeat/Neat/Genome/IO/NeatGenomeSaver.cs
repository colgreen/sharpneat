// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.IO;
using SharpNeat.IO.Models;

namespace SharpNeat.Neat.Genome.IO;

/// <summary>
/// For saving/serializing instances of <see cref="NeatGenome{T}"/> to file, stream, etc.
/// </summary>
public static class NeatGenomeSaver
{
    /// <summary>
    /// Save a genome to the specified file.
    /// </summary>
    /// <param name="genome">The genome to save.</param>
    /// <param name="path">The path of the file to save to.</param>
    /// <typeparam name="T">Connection weight data type.</typeparam>
    public static void Save<T>(
        NeatGenome<T> genome, string path)
        where T : struct
    {
        // Convert the genome to a NetFileModel.
        NetFileModel netFileModel = NeatGenomeConverter.ToNetFileModel(genome);

        // Save the NetFileModel.
        NetFile.Save(netFileModel, path);
    }

    /// <summary>
    /// Save a genome to the given stream.
    /// </summary>
    /// <param name="genome">The genome to save.</param>
    /// <param name="stream">The stream to save the genome to.</param>
    /// <typeparam name="T">Connection weight data type.</typeparam>
    /// <remarks>This method does not close the Stream.</remarks>
    public static void Save<T>(
        NeatGenome<T> genome, Stream stream)
        where T : struct
    {
        // Convert the genome to a NetFileModel.
        NetFileModel netFileModel = NeatGenomeConverter.ToNetFileModel(genome);

        // Save the NetFileModel.
        NetFile.Save(netFileModel, stream);
    }
}
