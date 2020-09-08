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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows.Forms;
using log4net;
using log4net.Config;
using log4net.Repository;
using SharpNeat.Windows.App.Experiments;

namespace SharpNeat.Windows.App
{
    /// <summary>
    /// SharpNEAT main GUI window.
    /// </summary>
    public partial class MainForm : Form
    {
        private static readonly ILog __log = LogManager.GetLogger(typeof(MainForm));

        #region Form Constructor / Initialisation

        /// <summary>
        /// Construct and initialize the form.
        /// </summary>
        public MainForm()
        {
            // Set the default culture for all threads in the application to the Invariant culture.
            // This is a cheap way of ensuring that all form fields and data IO routines
            // read and write textual data in the same format, in particular the use of a dot as the
            // decimal separator (some culture use a comma).
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            InitializeComponent();
            Logger.SetListBox(lbxLog);

            // Initialise logging.
            ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.properties"));

            // Populate the experiments combo-box (drop-down list) with experiment loaded from the experiments.json config file.
            InitExperimentList();
        }

        #endregion

        #region Private Methods

        private void InitExperimentList()
        {
            // Load experiments.json from file.
            // Note. Use of ReadAllText() isn't ideal, but for a small file it's fine, and this avoids the complexities of dealign 
            // with async code in a synchronous context.
            string experimentsJson = File.ReadAllText("config/experiments.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            ExperimentRegistry registry = JsonSerializer.Deserialize<ExperimentRegistry>(experimentsJson, options);

            // Populate the combo box.
            foreach(ExperimentInfo expInfo in registry.Experiments)
            {
                cmbExperiments.Items.Add(expInfo);
            }

            // Pre-select first item.
            cmbExperiments.SelectedIndex = 0;
        }

        #endregion
    }
}
