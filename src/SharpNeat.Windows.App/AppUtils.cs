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
using System;
using System.Text.Json;
using System.Windows.Forms;
using SharpNeat.Experiments;
using SharpNeat.Experiments.Windows;
using SharpNeat.IO;
using SharpNeat.Windows.App.Experiments;

namespace SharpNeat.Windows.App
{
    internal static class AppUtils
    {
        public static INeatExperiment<double> CreateAndConfigureExperiment(ExperimentInfo expInfo)
        {
            // Create an experiment factory.
            INeatExperimentFactory factory = (INeatExperimentFactory)Activator.CreateInstance(
                expInfo.ExperimentFactory.AssemblyName,
                expInfo.ExperimentFactory.TypeName)
                .Unwrap();

            // Load experiment json config from file.
            JsonDocument configDoc = JsonUtils.LoadUtf8(expInfo.ConfigFile);

            // Create an instance of INeatExperiment, configured using the supplied json config.
            INeatExperiment<double> experiment = factory.CreateExperiment(configDoc.RootElement);
            return experiment;
        }

        public static IExperimentUI CreateAndConfigureExperimentUI(ExperimentInfo expInfo)
        {
            if(expInfo.ExperimentUIFactory is null)
                return null;

            // Create an experimentUI factory.
            IExperimentUIFactory factory = (IExperimentUIFactory)Activator.CreateInstance(
                expInfo.ExperimentUIFactory.AssemblyName,
                expInfo.ExperimentUIFactory.TypeName)
                .Unwrap();

            // Load experiment json config from file.
            JsonDocument configDoc = JsonUtils.LoadUtf8(expInfo.ConfigFile);

            // Create an instance of INeatExperiment, configured using the supplied json config.
            IExperimentUI experimentUI = factory.CreateExperimentUI(configDoc.RootElement);
            return experimentUI;
        }

        /// <summary>
        /// Ask the user for a filename / path.
        /// </summary>
        public static string SelectFileToSave(string dialogTitle, string fileExtension, string filter)
        {
            SaveFileDialog oDialog = new()
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
}
