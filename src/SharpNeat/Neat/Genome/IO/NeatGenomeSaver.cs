// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
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
    /// <typeparam name="TScalar">Connection weight data type.</typeparam>
    /// <param name="genome">The genome to save.</param>
    /// <param name="path">The path of the file to save to.</param>
    public static void Save<TScalar>(
        NeatGenome<TScalar> genome, string path)
        where TScalar : unmanaged, INumberBase<TScalar>
    {
        // Convert the genome to a NetFileModel.
        NetFileModel netFileModel = NeatGenomeConverter.ToNetFileModel(genome);

        // Save the NetFileModel.
        NetFile.Save(netFileModel, path);
    }

    /// <summary>
    /// Save a genome to the given stream.
    /// </summary>
    /// <typeparam name="TScalar">Connection weight data type.</typeparam>
    /// <param name="genome">The genome to save.</param>
    /// <param name="stream">The stream to save the genome to.</param>
    /// <remarks>This method does not close the Stream.</remarks>
    public static void Save<TScalar>(
        NeatGenome<TScalar> genome, Stream stream)
        where TScalar : unmanaged, INumberBase<TScalar>
    {
        // Convert the genome to a NetFileModel.
        NetFileModel netFileModel = NeatGenomeConverter.ToNetFileModel(genome);

        // Save the NetFileModel.
        NetFile.Save(netFileModel, stream);
    }
}
