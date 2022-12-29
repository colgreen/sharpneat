// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Windows.Experiments;

/// <summary>
/// <see cref="IExperimentUiFactory"/> extension methods.
/// </summary>
public static class ExperimentUiFactoryExtensions
{
    /// <summary>
    /// Creates a new instance of <see cref="IExperimentUi"/> using the experiment
    /// configuration settings from the provided json object model.
    /// </summary>
    /// <param name="experimentUiFactory">The experiment UI factory instance.</param>
    /// <param name="jsonConfigFilename">The name of a file from which experiment JSON configuration can be read.</param>
    /// <returns>A new instance of <see cref="IExperimentUi"/>.</returns>
    public static IExperimentUi CreateExperimentUi(
        this IExperimentUiFactory experimentUiFactory,
        string jsonConfigFilename)
    {
        using FileStream fs = File.OpenRead(jsonConfigFilename);
        return experimentUiFactory.CreateExperimentUi(fs);
    }
}
