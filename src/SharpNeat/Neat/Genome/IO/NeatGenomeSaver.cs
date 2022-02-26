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
using System.Globalization;
using System.IO;
using System.Text;
using SharpNeat.Graphs;

namespace SharpNeat.Neat.Genome.IO;

/// <summary>
/// For saving/serializing instances of <see cref="NeatGenome{T}"/> to file, stream, etc.
/// </summary>
/// <typeparam name="T">Connection weight data type.</typeparam>
public static class NeatGenomeSaver<T> where T : struct
{
    static readonly Encoding __utf8Encoding = new UTF8Encoding(false, true);

    #region Public Static Methods

    /// <summary>
    /// Save a genome to the specified file.
    /// </summary>
    /// <param name="genome">The genome to save.</param>
    /// <param name="path">The path of the file to save to.</param>
    public static void Save(NeatGenome<T> genome, string path)
    {
        using var sw = new StreamWriter(path, false, __utf8Encoding);
        Save(genome, sw);
    }

    /// <summary>
    /// Save a genome to the given stream.
    /// </summary>
    /// <param name="genome">The genome to save.</param>
    /// <param name="stream">The stream to save the genome to.</param>
    /// <remarks>This method does not close the Stream.</remarks>
    public static void Save(NeatGenome<T> genome, Stream stream)
    {
        using var sw = new StreamWriter(stream, __utf8Encoding, 1024, true);
        Save(genome, sw);
    }

    /// <summary>
    /// Save a genome to the given StreamWriter.
    /// </summary>
    /// <param name="genome">The genome to save.</param>
    /// <param name="sw">The StreamWriter to save the genome to.</param>
    public static void Save(NeatGenome<T> genome, StreamWriter sw)
    {
        // Write input and output node counts.
        sw.WriteLine("# Input and output node counts.");
        sw.WriteLine($"{genome.MetaNeatGenome.InputNodeCount}\t{genome.MetaNeatGenome.OutputNodeCount}");
        sw.WriteLine();

        // Write connections.
        WriteConnections(genome.ConnectionGenes, sw);

        // Write activation function.
        WriteActivationFunction(genome.MetaNeatGenome.ActivationFn.GetType().Name, sw);
    }

    #endregion

    #region Private Static Methods

    private static void WriteConnections(ConnectionGenes<T> connGenes, StreamWriter sw)
    {
        sw.WriteLine("# Connections (source target weight).");

        DirectedConnection[] connArr = connGenes._connArr;
        T[] weightArr = connGenes._weightArr;

        for(int i=0; i < connArr.Length; i++)
        {
            var conn = connArr[i];

            // Use runtime binding to access ToString(string) on the weight type,
            // which will be either double or float. This is slow but this is not performance
            // critical code.
            dynamic weight = weightArr[i];
            string weightStr = weight.ToString("R", CultureInfo.InvariantCulture);

            sw.WriteLine($"{conn.SourceId}\t{conn.TargetId}\t{weightStr}");
        }

        sw.WriteLine();
    }

    private static void WriteActivationFunction(string name, StreamWriter sw)
    {
        sw.WriteLine("# Activation functions (functionId functionCode).");
        sw.WriteLine($"0\t{name}");
        sw.WriteLine();
    }

    #endregion
}
