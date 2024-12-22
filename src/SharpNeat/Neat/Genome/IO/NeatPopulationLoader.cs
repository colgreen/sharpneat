// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.IO.Compression;
using System.Numerics;

namespace SharpNeat.Neat.Genome.IO;

/// <summary>
/// For loading/deserializing a population of <see cref="NeatGenome{T}"/> instances from the local filesystem.
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
public sealed class NeatPopulationLoader<TScalar>
    where TScalar : unmanaged, INumberBase<TScalar>
{
    readonly MetaNeatGenome<TScalar> _metaNeatGenome;
    int _genomeId;

    /// <summary>
    /// Construct with the provided <see cref="MetaNeatGenome{T}"/>.
    /// </summary>
    /// <param name="metaNeatGenome">Neat genome metadata object.</param>
    public NeatPopulationLoader(
        MetaNeatGenome<TScalar> metaNeatGenome)
    {
        _metaNeatGenome = metaNeatGenome ?? throw new ArgumentNullException(nameof(metaNeatGenome));
    }

    /// <summary>
    /// Load a population from a folder containing one or more genome files (with a .net file extension).
    /// </summary>
    /// <param name="path">Path to the folder to load genomes from.</param>
    /// <returns>A list of the loaded genomes.</returns>
    public List<NeatGenome<TScalar>> LoadFromFolder(string path)
    {
        if(!Directory.Exists(path))
            throw new IOException($"Directory does not exist [{path}]");

        // Determine the set of genome files to load.
        DirectoryInfo dirInfo = new(path);
        FileInfo[] fileInfoArr = dirInfo.GetFiles("*.net");

        // Alloc genome list with an appropriate capacity.
        List<NeatGenome<TScalar>> genomeList = new(fileInfoArr.Length);

        // Loop the genome files, loading each in turn.
        foreach(FileInfo fileInfo in fileInfoArr)
        {
            NeatGenome<TScalar> genome = NeatGenomeLoader.Load(
                fileInfo.FullName, _metaNeatGenome, _genomeId++);

            genomeList.Add(genome);
        }

        return genomeList;
    }

    /// <summary>
    /// Load a population from a zip archive file containing one or more genome file entries (with a .net file extension).
    /// </summary>
    /// <param name="path">Path to the zip file to load.</param>
    /// <returns>A list of the loaded genomes.</returns>
    public List<NeatGenome<TScalar>> LoadFromZipArchive(string path)
    {
        if(!File.Exists(path))
            throw new IOException($"File does not exist [{path}]");

        using ZipArchive zipArchive = ZipFile.OpenRead(path);

        // Alloc genome list with an appropriate capacity.
        List<NeatGenome<TScalar>> genomeList = new(zipArchive.Entries.Count);

        // Loop the genome file entries, loading each in turn.
        foreach(ZipArchiveEntry zipEntry in zipArchive.Entries)
        {
            // Skip non-net files.
            if(Path.GetExtension(zipEntry.Name) != ".net")
                continue;

            using(Stream zipEntryStream = zipEntry.Open())
            {
                NeatGenome<TScalar> genome = NeatGenomeLoader.Load(
                    zipEntryStream, _metaNeatGenome, _genomeId++);

                genomeList.Add(genome);
            }
        }

        return genomeList;
    }
}
