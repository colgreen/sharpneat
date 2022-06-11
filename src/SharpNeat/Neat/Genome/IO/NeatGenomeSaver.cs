// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.IO;
using SharpNeat.IO.Models;

namespace SharpNeat.Neat.Genome.IO;

/// <summary>
/// For saving/serializing instances of <see cref="NeatGenome{T}"/> to file, stream, etc.
/// </summary>
/// <typeparam name="T">Connection weight data type.</typeparam>
public static class NeatGenomeSaver<T>
    where T : struct
{
    /// <summary>
    /// Save a genome to the specified file.
    /// </summary>
    /// <param name="genome">The genome to save.</param>
    /// <param name="path">The path of the file to save to.</param>
    public static void Save(NeatGenome<T> genome, string path)
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
    /// <remarks>This method does not close the Stream.</remarks>
    public static void Save(NeatGenome<T> genome, Stream stream)
    {
        // Convert the genome to a NetFileModel.
        NetFileModel netFileModel = NeatGenomeConverter.ToNetFileModel(genome);

        // Save the NetFileModel.
        NetFile.Save(netFileModel, stream);
    }
}
