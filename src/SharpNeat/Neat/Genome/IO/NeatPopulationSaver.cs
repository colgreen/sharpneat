﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Globalization;
using System.IO.Compression;
using System.Numerics;
using static System.FormattableString;

namespace SharpNeat.Neat.Genome.IO;

/// <summary>
/// For saving/serializing a population of <see cref="NeatGenome{T}"/> instances to the local filesystem.
/// </summary>
/// <remarks>
/// A population is a collection of genomes. The genomes are each serialized individually, as when saving a single genome.
/// The genome files are then either created in a new folder that contains all genomes for the population, or in a single
/// zip archive file.
/// </remarks>
public static class NeatPopulationSaver
{
    /// <summary>
    /// Save the given population of genomes to the specified path on the local filesystem.
    /// </summary>
    /// <param name="genomeList">The population of genomes to save.</param>
    /// <param name="parentPath">Path to an existing folder to create the new population folder.</param>
    /// <param name="name">The name to assign to the population folder.</param>
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    /// <remarks>
    /// A population is a collection of genomes. The genomes each serialized individually as when saving a single genome.
    /// The genome files are then either created in a new folder that contains all genomes for the population, or in a single
    /// zip archive file.
    /// </remarks>
    public static void SaveToFolder<TScalar>(
        IList<NeatGenome<TScalar>> genomeList,
        string parentPath,
        string name)
        where TScalar : struct, INumberBase<TScalar>
    {
        ArgumentNullException.ThrowIfNull(parentPath);
        ArgumentNullException.ThrowIfNull(name);

        // Check if the specified parent folder exists.
        if(!Directory.Exists(parentPath))
            throw new IOException($"parentPath does not exist [{parentPath}]");

        // Check if the specified population folder name exists.
        string popFolderPath = Path.Combine(parentPath, name);
        if(Directory.Exists(popFolderPath))
            throw new IOException($"The specified population folder already exists [{popFolderPath}]");

        // Create the population folder.
        DirectoryInfo popDirInfo = Directory.CreateDirectory(popFolderPath);
        string popDirPath = popDirInfo.FullName;

        // Loop the genomes; save each one in turn.
        foreach(var genome in genomeList)
        {
            // Build the genome's filepath.
            string genomePath = Path.Combine(popDirPath, genome.Id.ToString("D6", CultureInfo.InvariantCulture) + ".net");

            // Save the genome.
            NeatGenomeSaver.Save(genome, genomePath);
        }
    }

    /// <summary>
    /// Save the given population of genomes to a single zip archive file.
    /// </summary>
    /// <param name="genomeList">The population of genomes to save.</param>
    /// <param name="filepath">The name of the zip archive (without the .zip extension, which will be appended by default).</param>
    /// <param name="compressionLevel">ZIP archive compression level.</param>
    /// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
    public static void SaveToZipArchive<TScalar>(
        IList<NeatGenome<TScalar>> genomeList,
        string filepath,
        CompressionLevel compressionLevel)
        where TScalar : struct, INumberBase<TScalar>
    {
        ArgumentNullException.ThrowIfNull(filepath);

        // Append zip extension for filenames that do not yet have it.
        // For all other extensions we leave them in place. If it isn't .zip then we assume the caller does this with good reason.
        string extension = Path.GetExtension(filepath);
        if(extension == string.Empty)
            filepath += ".zip";

        // Check if the specified population zip archive name exists.
        if(File.Exists(filepath))
            File.Delete(filepath);

        // Create a new zip archive.
        using FileStream fs = new(filepath, FileMode.CreateNew);
        using ZipArchive zipArchive = new(fs, ZipArchiveMode.Create);

        // Loop the genomes; add each one in turn to the zip archive.
        foreach(var genome in genomeList)
        {
            // Build the genome's entry name.
            string entryName = Invariant($"{genome.Id:D6}.net");

            // Create an new zip entry.
            ZipArchiveEntry zipEntry = zipArchive.CreateEntry(entryName, compressionLevel);
            using(Stream zipEntryStream = zipEntry.Open())
            {
                // Serialize the genome into the zip entry.
                NeatGenomeSaver.Save(genome, zipEntryStream);
            }
        }
    }
}
