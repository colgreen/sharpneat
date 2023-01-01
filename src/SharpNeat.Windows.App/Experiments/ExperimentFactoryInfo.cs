// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Windows.App.Experiments;

internal sealed record ExperimentFactoryInfo
{
    public required string AssemblyName { get; init; }
    public required string TypeName { get; init; }
}
