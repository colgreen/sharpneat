// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.Experiments;
using SharpNeat.Windows.App.Experiments;
using SharpNeat.Windows.Experiments;

namespace SharpNeat.Windows.App;

internal static class AppUtils
{
    public static INeatExperiment<TScalar> CreateAndConfigureExperiment<TScalar>(
        ExperimentInfo expInfo)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        // Create an experiment factory.
        INeatExperimentFactory factory = (INeatExperimentFactory)Activator.CreateInstance(
            expInfo.ExperimentFactory.AssemblyName,
            expInfo.ExperimentFactory.TypeName)
            .Unwrap();

        // Create an instance of INeatExperiment, configured using the supplied json config.
        return factory.CreateExperiment<TScalar>(expInfo.ConfigFile);
    }

    public static IExperimentUi CreateAndConfigureExperimentUi<TScalar>(
        INeatExperiment<TScalar> neatExperiment,
        ExperimentInfo expInfo)
        where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
    {
        if(expInfo.ExperimentUiFactory is null)
            return null;

        // Create an experiment UI factory.
        IExperimentUiFactory factory = (IExperimentUiFactory)Activator.CreateInstance(
            expInfo.ExperimentUiFactory.AssemblyName,
            expInfo.ExperimentUiFactory.TypeName)
            .Unwrap();

        // Create an instance of INeatExperiment, configured using the supplied json config.
        IExperimentUi experimentUi = factory.CreateExperimentUi(
            neatExperiment,
            expInfo.ConfigFile);

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
