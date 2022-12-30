// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Windows.App.Experiments;

internal sealed record ExperimentRegistry
{
    public required List<ExperimentInfo> Experiments { get; init; }
}
