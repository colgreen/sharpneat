﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.IO.Compression;

namespace SharpNeat.Neat.Genome.IO;

/// <summary>
/// For loading/deserializing a population of <see cref="NeatGenome{T}"/> instances from the local filesystem.
/// </summary>
/// <typeparam name="T">Connection weight data type.</typeparam>
public sealed class NeatPopulationLoader<T> where T : struct
{
    readonly NeatGenomeLoader<T> _genomeLoader;

    #region Constructors

    /// <summary>
    /// Construct with the provided <see cref="NeatGenomeLoader{T}"/>.
    /// </summary>
    /// <param name="genomeLoader">A genome loader.</param>
    public NeatPopulationLoader(NeatGenomeLoader<T> genomeLoader)
    {
        _genomeLoader = genomeLoader ?? throw new ArgumentNullException(nameof(genomeLoader));
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Load a population from a folder containing one or more genome files (with a .genome file extension).
    /// </summary>
    /// <param name="path">Path to the folder to load genomes from.</param>
    /// <returns>A list of the loaded genomes.</returns>
    public List<NeatGenome<T>> LoadFromFolder(string path)
    {
        if(!Directory.Exists(path))
            throw new IOException($"Directory does not exist [{path}]");

        // Determine the set of genome files to load.
        DirectoryInfo dirInfo = new(path);
        FileInfo[] fileInfoArr = dirInfo.GetFiles("*.genome");

        // Alloc genome list with an appropriate capacity.
        List<NeatGenome<T>> genomeList = new(fileInfoArr.Length);

        // Loop the genome files, loading each in turn.
        foreach(FileInfo fileInfo in fileInfoArr)
        {
            NeatGenome<T> genome = _genomeLoader.Load(fileInfo.FullName);
            genomeList.Add(genome);
        }

        return genomeList;
    }

    /// <summary>
    /// Load a population from a zip archive file containing one or more genome file entries (with a .genome file extension).
    /// </summary>
    /// <param name="path">Path to the zip file to load.</param>
    /// <returns>A list of the loaded genomes.</returns>
    public List<NeatGenome<T>> LoadFromZipArchive(string path)
    {
        if(!File.Exists(path))
            throw new IOException($"File does not exist [{path}]");

        using ZipArchive zipArchive = ZipFile.OpenRead(path);

        // Alloc genome list with an appropriate capacity.
        List<NeatGenome<T>> genomeList = new(zipArchive.Entries.Count);

        // Loop the genome file entries, loading each in turn.
        foreach(ZipArchiveEntry zipEntry in zipArchive.Entries)
        {
            // Skip non-genome files.
            if(Path.GetExtension(zipEntry.Name) != ".genome")
                continue;

            using(Stream zipEntryStream = zipEntry.Open())
            {
                NeatGenome<T> genome = _genomeLoader.Load(zipEntryStream);
                genomeList.Add(genome);
            }
        }

        return genomeList;
    }

    #endregion
}
