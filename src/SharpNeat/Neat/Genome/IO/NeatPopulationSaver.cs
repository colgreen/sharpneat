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
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace SharpNeat.Neat.Genome.IO
{
    /// <summary>
    /// For saving/serializing a population of <see cref="NeatGenome{T}"/> instances to the local filesystem.
    /// </summary>
    /// <remarks>
    /// A population is a collection of genomes. The genomes are each serialized individually, as when saving a single genome.
    /// The genome files are then either created in a new folder that contains all genomes for the population, or in a single
    /// zip archive file.
    /// </remarks>
    /// <typeparam name="T">Connection weight data type.</typeparam>
    public static class NeatPopulationSaver<T> where T : struct
    {
        #region Public Static Methods

        /// <summary>
        /// Save the given population of genomes to the specified path on the local filesystem.
        /// </summary>
        /// <param name="genomeList">The population of genomes to save.</param>
        /// <param name="parentPath">Path to an existing folder to create the new population folder.</param>
        /// <param name="name">The name to assign to the population folder.</param>
        /// <remarks>
        /// A population is a collection of genomes. The genomes each serialized individually as when saving a single genome.
        /// The genome files are then either created in a new folder that contains all genomes for the population, or in a single
        /// zip archive file.
        /// </remarks>
        public static void SaveToFolder(
            IList<NeatGenome<T>> genomeList,
            string parentPath,
            string name)
        {
            if(parentPath is null) throw new ArgumentNullException(nameof(parentPath));
            if(name is null) throw new ArgumentNullException(nameof(name));

            // Check if the specified parent folder exists.
            if(!Directory.Exists(parentPath)) {
                throw new IOException($"parentPath does not exist [{parentPath}]");
            }

            // Check if the specified population folder name exists.
            string popFolderPath = Path.Combine(parentPath, name);
            if(Directory.Exists(popFolderPath)) {
                throw new IOException($"The specified population folder already exists [{popFolderPath}]");
            }

            // Create the population folder.
            DirectoryInfo popDirInfo = Directory.CreateDirectory(popFolderPath);
            string popDirPath = popDirInfo.FullName;

            // Loop the genomes; save each one in turn.
            foreach(var genome in genomeList)
            {
                // Build the genome's filepath.
                string genomePath = Path.Combine(popDirPath, genome.Id.ToString("D6") + ".genome");

                // Save the genome.
                NeatGenomeSaver<T>.Save(genome, genomePath);
            }
        }

        /// <summary>
        /// Save the given population of genomes to a single zip archive file.
        /// </summary>
        /// <param name="genomeList">The population of genomes to save.</param>
        /// <param name="parentPath">Path to an existing folder to create the zip archive within.</param>
        /// <param name="name">The name of the zip archive (without the .zip extension, which will be appended by default).</param>
        /// <param name="compressionLevel">ZIP archive compression level.</param>
        public static void SaveToZipArchive(
            IList<NeatGenome<T>> genomeList,
            string parentPath,
            string name,
            CompressionLevel compressionLevel)
        {
            if(parentPath is null) throw new ArgumentNullException(nameof(parentPath));
            if(name is null) throw new ArgumentNullException(nameof(name));

            // Check if the specified parent folder exists.
            if(!Directory.Exists(parentPath)) {
                throw new IOException($"parentPath does not exist [{parentPath}]");
            }

            // Append zip extension for filenames that do not yet have it.
            // For all other extensions we leave them in place. If it isn't .zip then we assume the caller does this with good reason.
            string extension = Path.GetExtension(name);
            if(extension == string.Empty) {
                name += ".zip";
            }

            // Check if the specified population zip archive name exists.
            string popZipPath = Path.Combine(parentPath, name);
            if(File.Exists(popZipPath)) {
                throw new IOException($"The specified population zip archive already exists [{popZipPath}]");
            }

            string nameWithoutExt = Path.GetFileNameWithoutExtension(name);

            // Create a new zip archive.
            using(FileStream fs = new(popZipPath, FileMode.CreateNew))
            using(ZipArchive zipArchive = new(fs, ZipArchiveMode.Create))
            {
                // Loop the genomes; add each one in turn to the zip archive.
                foreach(var genome in genomeList)
                {
                    // Build the genome's entry name.
                    string entryName = Path.Combine(nameWithoutExt, genome.Id.ToString("D6") + ".genome");

                    // Create an new zip entry.
                    ZipArchiveEntry zipEntry = zipArchive.CreateEntry(entryName, compressionLevel);
                    using(Stream zipEntryStream = zipEntry.Open())
                    {
                        // Serialize the genome into the zip entry.
                        NeatGenomeSaver<T>.Save(genome, zipEntryStream);
                    }
                }
            }
        }

        #endregion
    }
}
