// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Windows.App.Experiments;

internal sealed class ExperimentInfo
{
    public string Name { get; set; }
    public ExperimentFactoryInfo ExperimentFactory { get; set; }
    public string ConfigFile { get; set; }
    public string DescriptionFile { get; set; }
    public ExperimentUiFactoryInfo ExperimentUiFactory { get; set; }
}
