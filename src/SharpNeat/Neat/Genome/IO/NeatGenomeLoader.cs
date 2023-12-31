﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.IO;
using SharpNeat.IO.Models;

namespace SharpNeat.Neat.Genome.IO;

/// <summary>
/// For loading/deserializing instances of <see cref="NeatGenome{T}"/> from file, stream, etc.
/// </summary>
public static class NeatGenomeLoader
{
    /// <summary>
    /// Load a genome from the specified file.
    /// </summary>
    /// <typeparam name="TScalar">Connection weight data type.</typeparam>
    /// <param name="path">The name and path of the file to load.</param>
    /// <param name="metaNeatGenome">A <see cref="MetaNeatGenome{T}"/> instance; required to construct a new
    /// <see cref="MetaNeatGenome{T}"/>.</param>
    /// <param name="genomeId">The ID to assign to the new genome.</param>
    /// <returns>A new instance of <see cref="NeatGenome{T}"/>.</returns>
    public static NeatGenome<TScalar> Load<TScalar>(
        string path,
        MetaNeatGenome<TScalar> metaNeatGenome,
        int genomeId)
        where TScalar : struct, INumberBase<TScalar>
    {
        // Load the NetFileModel.
        NetFileModel netFileModel = NetFile.Load(path);

        // Convert the NetFileModel to a genome.
        NeatGenome<TScalar> genome = NeatGenomeConverter.ToNeatGenome(netFileModel, metaNeatGenome, genomeId);
        return genome;
    }

    /// <summary>
    /// Load a genome from the provided stream.
    /// </summary>
    /// <typeparam name="TScalar">Connection weight data type.</typeparam>
    /// <param name="stream">The stream to load from.</param>
    /// <param name="metaNeatGenome">A <see cref="MetaNeatGenome{T}"/> instance; required to construct a new
    /// <see cref="MetaNeatGenome{T}"/>.</param>
    /// <param name="genomeId">The ID to assign to the new genome.</param>
    /// <returns>A new instance of <see cref="NeatGenome{T}"/>.</returns>
    public static NeatGenome<TScalar> Load<TScalar>(
        Stream stream,
        MetaNeatGenome<TScalar> metaNeatGenome,
        int genomeId)
        where TScalar : struct, INumberBase<TScalar>
    {
        // Load the NetFileModel.
        NetFileModel netFileModel = NetFile.Load(stream);

        // Convert the NetFileModel to a genome.
        NeatGenome<TScalar> genome = NeatGenomeConverter.ToNeatGenome(netFileModel, metaNeatGenome, genomeId);
        return genome;
    }
}
