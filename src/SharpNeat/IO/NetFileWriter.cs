// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.IO.Models;
using static System.FormattableString;

namespace SharpNeat.IO;

/// <summary>
/// For writing of 'net' format files.
/// </summary>
internal class NetFileWriter
{
    /// <summary>
    /// Write a <see cref="NetFileModel"/> to a stream writer using the 'net' file format.
    /// </summary>
    /// <param name="model">The <see cref="NetFileModel"/> to write.</param>
    /// <param name="sw">The stream writer to write to.</param>
    public static void Write(NetFileModel model, StreamWriter sw)
    {
        // Write input and output node counts.
        sw.WriteLine("# Input and output node counts.");
        sw.WriteLine(Invariant($"{model.InputCount}\t{model.OutputCount}"));
        sw.WriteLine();

        // Write cyclic/acyclic indicator.
        sw.WriteLine("# Cyclic/acyclic indicator.");

        if (model.IsAcyclic)
            sw.WriteLine("acyclic");
        else
            sw.WriteLine(Invariant($"cyclic\t{model.CyclesPerActivation}"));

        sw.WriteLine();

        // Write connections.
        sw.WriteLine("# Connections (source target weight).");
        foreach(ConnectionLine conn in model.Connections)
        {
            sw.WriteLine(Invariant($"{conn.SourceId}\t{conn.TargetId}\t{conn.Weight:R}"));
        }

        sw.WriteLine();

        // Write activation functions.
        sw.WriteLine("# Activation functions (functionId functionCode).");
        foreach(ActivationFnLine actFn in model.ActivationFns)
        {
            sw.WriteLine(Invariant($"{actFn.Id}\t{actFn.Code}"));
        }

        sw.WriteLine();
    }
}
