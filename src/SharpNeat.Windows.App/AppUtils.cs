// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Experiments;
using SharpNeat.Windows.App.Experiments;
using SharpNeat.Windows.Experiments;

namespace SharpNeat.Windows.App;

internal static class AppUtils
{
    public static INeatExperiment<double> CreateAndConfigureExperiment(ExperimentInfo expInfo)
    {
        // Create an experiment factory.
        INeatExperimentFactory factory = (INeatExperimentFactory)Activator.CreateInstance(
            expInfo.ExperimentFactory.AssemblyName,
            expInfo.ExperimentFactory.TypeName)
            .Unwrap();

        // Create an instance of INeatExperiment, configured using the supplied json config.
        INeatExperiment<double> experiment = factory.CreateExperiment(expInfo.ConfigFile);
        return experiment;
    }

    public static IExperimentUi CreateAndConfigureExperimentUi(ExperimentInfo expInfo)
    {
        if(expInfo.ExperimentUiFactory is null)
            return null;

        // Create an experiment UI factory.
        IExperimentUiFactory factory = (IExperimentUiFactory)Activator.CreateInstance(
            expInfo.ExperimentUiFactory.AssemblyName,
            expInfo.ExperimentUiFactory.TypeName)
            .Unwrap();

        // Create an instance of INeatExperiment, configured using the supplied json config.
        IExperimentUi experimentUi = factory.CreateExperimentUi(expInfo.ConfigFile);
        return experimentUi;
    }

    /// <summary>
    /// Ask the user for a filename / path.
    /// </summary>
    public static string SelectFileToOpen(string dialogTitle, string fileExtension, string filter)
    {
        using OpenFileDialog oDialog = new()
        {
            AddExtension = true,
            DefaultExt = fileExtension,
            Filter = filter,
            Title = dialogTitle,
            RestoreDirectory = true
        };

        // Show dialog and block until user selects a file.
        if(oDialog.ShowDialog() == DialogResult.OK)
            return oDialog.FileName;

        // No selection.
        return null;
    }

    /// <summary>
    /// Ask the user for a filename / path.
    /// </summary>
    public static string SelectFileToSave(string dialogTitle, string fileExtension, string filter)
    {
        using SaveFileDialog oDialog = new()
        {
            AddExtension = true,
            DefaultExt = fileExtension,
            Filter = filter,
            Title = dialogTitle,
            RestoreDirectory = true
        };

        // Show dialog and block until user selects a file.
        if(oDialog.ShowDialog() == DialogResult.OK)
            return oDialog.FileName;

        // No selection.
        return null;
    }
}
