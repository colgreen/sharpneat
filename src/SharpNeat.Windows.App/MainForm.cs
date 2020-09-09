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
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using log4net;
using SharpNeat.Experiments;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Reproduction.Asexual;
using SharpNeat.Windows.App.Experiments;
using static SharpNeat.Windows.App.AppUtils;

namespace SharpNeat.Windows.App
{
    /// <summary>
    /// SharpNEAT main GUI window.
    /// </summary>
    public partial class MainForm : Form
    {
        private static readonly ILog __log = LogManager.GetLogger(typeof(MainForm));

        // The current NEAT experiment.
        private INeatExperiment<double> _neatExperiment;


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

        #region UI Event Handlers

        private void btnExperimentInfo_Click(object sender,System.EventArgs e)
        {
            if(cmbExperiments.SelectedItem is ExperimentInfo expInfo)
            {
                if(!string.IsNullOrEmpty(expInfo.DescriptionFile) && File.Exists(expInfo.DescriptionFile))
                {
                    string description = File.ReadAllText(expInfo.DescriptionFile);
                    MessageBox.Show(description, "Experiment Description");
                }
            }
        }

        private void btnResetExperiment_Click(object sender,EventArgs e)
        {
            _neatExperiment = CreateAndConfigureExperiment((ExperimentInfo)cmbExperiments.SelectedItem);
            LoadIntoUI(_neatExperiment);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frmAboutBox = new AboutForm();
            frmAboutBox.ShowDialog(this);
        }

        private void btnCopyLogToClipboard_Click(object sender,EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach(Logger.LogItem item in lbxLog.Items) {
                sb.AppendLine(item.Message);
            }

            if(sb.Length > 0) {
                Clipboard.SetText(sb.ToString());
            }
        }

        #endregion

        #region Private Methods [Load/Save UI Parameters]

        private void LoadIntoUI(INeatExperiment<double> experiment)
        {
            LoadIntoUI(experiment.NeatEvolutionAlgorithmSettings);
            LoadIntoUI(experiment.ReproductionAsexualSettings);

            SetValue(txtPopulationSize, experiment.PopulationSize);
            SetValue(txtInitialInterconnectionsProportion, experiment.InitialInterconnectionsProportion);
        }

        private void LoadIntoUI(NeatEvolutionAlgorithmSettings settings)
        {
            SetValue(txtSpeciesCount, settings.SpeciesCount);
            SetValue(txtElitismProportion, settings.ElitismProportion);
            SetValue(txtSelectionProportion, settings.SelectionProportion);
            SetValue(txtOffspringAsexualProportion, settings.OffspringAsexualProportion);
            SetValue(txtOffspringSexualProportion, settings.OffspringSexualProportion);
            SetValue(txtInterspeciesMatingProportion, settings.InterspeciesMatingProportion);
        }

        private void LoadIntoUI(NeatReproductionAsexualSettings settings)
        {
            SetValue(txtConnectionWeightMutationProbability, settings.ConnectionWeightMutationProbability);
            SetValue(txtAddNodeMutationProbability, settings.AddNodeMutationProbability);
            SetValue(txtAddConnectionMutationProbability, settings.AddConnectionMutationProbability);
            SetValue(txtDeleteConnectionMutationProbability, settings.DeleteConnectionMutationProbability);
        }

        private void SaveFromUI(NeatEvolutionAlgorithmSettings settings)
        {
            settings.SpeciesCount = GetValue(txtSpeciesCount, settings.SpeciesCount);
            settings.ElitismProportion = GetValue(txtSpeciesCount, settings.ElitismProportion);
            settings.SelectionProportion = GetValue(txtSpeciesCount, settings.SelectionProportion);
            settings.OffspringAsexualProportion = GetValue(txtSpeciesCount, settings.OffspringAsexualProportion);
            settings.OffspringSexualProportion = GetValue(txtSpeciesCount, settings.OffspringSexualProportion);
            settings.InterspeciesMatingProportion = GetValue(txtSpeciesCount, settings.InterspeciesMatingProportion);
        }

        #endregion
    }
}
