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
using System.Globalization;
using System.Text.Json;
using System.Windows.Forms;
using log4net;
using SharpNeat.Experiments;
using SharpNeat.IO;
using SharpNeat.Windows.App.Experiments;

namespace SharpNeat.Windows.App
{
    internal static class AppUtils
    {
        private static readonly ILog __log = LogManager.GetLogger(typeof(AppUtils));

        public static INeatExperiment<double> CreateAndConfigureExperiment(ExperimentInfo expInfo)
        {
            // Create an experiment factory.
            INeatExperimentFactory factory = (INeatExperimentFactory)Activator.CreateInstance(expInfo.ExperimentFactory.AssemblyName,expInfo.ExperimentFactory.TypeName).Unwrap();

            // Load experiment json config from file.
            JsonDocument configDoc = JsonUtils.LoadUtf8(expInfo.ConfigFile);

            // Create an instance of INeatExperiment, configured using the supplied json config.
            INeatExperiment<double> experiment = factory.CreateExperiment(configDoc.RootElement);
            return experiment;
        }

        public static void SetValue(TextBox txtBox, int val)
        {
            txtBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        public static void SetValue(TextBox txtBox, double val)
        {
            txtBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }        

        public static int GetValue(TextBox txtBox, int defaultVal)
        {
            if(int.TryParse(txtBox.Text, out int tmp)) {
                return tmp;
            }
            __log.ErrorFormat("Error parsing value of text field [{0}]", txtBox.Name);
            return defaultVal;
        }

        public static double GetValue(TextBox txtBox, double defaultVal)
        {
            if(double.TryParse(txtBox.Text, out double tmp)) {
                return tmp;
            }
            __log.ErrorFormat("Error parsing value of text field [{0}]", txtBox.Name);
            return defaultVal;
        }
    }
}
