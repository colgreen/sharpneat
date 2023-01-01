// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Windows.App.Experiments;

internal sealed record ExperimentInfo
{
    public required string Name { get; init; }
    public required ExperimentFactoryInfo ExperimentFactory { get; init; }
    public required string ConfigFile { get; init; }
    public required string DescriptionFile { get; init; }
    public required ExperimentUiFactoryInfo ExperimentUiFactory { get; init; }
}
