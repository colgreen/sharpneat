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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using log4net;
using SharpNeat.Experiments;
using SharpNeat.Neat;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Genome;
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
        private NeatPopulation<double> _neatPop;


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

        #region UI Event Handlers [Buttons]

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

        private void btnResetExperimentParameters_Click(object sender,EventArgs e)
        {
            _neatExperiment = CreateAndConfigureExperiment((ExperimentInfo)cmbExperiments.SelectedItem);
            SendSettingsToUI(_neatExperiment);
        }

        private void btnCreateRandomPop_Click(object sender,EventArgs e)
        {
            INeatExperiment<double> neatExperiment = GetNeatExperiment();
            MetaNeatGenome<double> metaNeatGenome = NeatUtils.CreateMetaNeatGenome(neatExperiment);

            // Create an initial population of genomes.
            _neatPop = NeatPopulationFactory<double>.CreatePopulation(
                metaNeatGenome,
                connectionsProportion: neatExperiment.InitialInterconnectionsProportion,
                popSize: neatExperiment.PopulationSize);

            // Update UI.
            UpdateUIState();
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

        #region UI Event Handlers [Menu Items]

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frmAboutBox = new AboutForm();
            frmAboutBox.ShowDialog(this);
        }

        #endregion

        #region Private Methods

        private INeatExperiment<double> GetNeatExperiment()
        {
            // Create a new experiment instance if one has not already been created.
            if(_neatExperiment is null) {
                _neatExperiment = CreateAndConfigureExperiment((ExperimentInfo)cmbExperiments.SelectedItem);
            }

            // Read settings from teh UI into the experiment instance, and return.
            GetSettingsFromUI(_neatExperiment);
            return _neatExperiment;
        }

        #endregion

        #region Private Methods [UpdateUIState]

        private void UpdateUIState()
        {
            if(_neatPop is null) {
                UpdateUIState_NoPopulation();
            } else {
                UpdateUIState_PopulationReady();
            }
        }

        private void UpdateUIState_NoPopulation()
        {
            // Enable experiment selection and initialization buttons.
            cmbExperiments.Enabled = true;
            btnResetExperimentParameters.Enabled = true;
            btnCreateRandomPop.Enabled = true;

            // Display population status (empty).
            txtPopulationStatus.Text = "Population not initialized";
            txtPopulationStatus.BackColor = Color.Red;

            // Disable search control buttons.
            btnSearchStart.Enabled = false;
            btnSearchStop.Enabled = false;
            btnSearchReset.Enabled = false;

            // Parameter fields enabled.
            txtPopulationSize.Enabled = true;
            txtInitialInterconnectionsProportion.Enabled = true;
            txtElitismProportion.Enabled = true;
            txtSelectionProportion.Enabled = true;
            txtOffspringAsexualProportion.Enabled = true;
            txtOffspringSexualProportion.Enabled = true;
            txtInterspeciesMatingProportion.Enabled = true;
            txtConnectionWeightMutationProbability.Enabled = true;
            txtAddNodeMutationProbability.Enabled = true;
            txtAddConnectionMutationProbability.Enabled = true;
            txtDeleteConnectionMutationProbability.Enabled = true;

            // Logging to file.
            gbxLogging.Enabled = true;

            // Menu bar (file).
            loadPopulationToolStripMenuItem.Enabled = true;
            loadSeedGenomesToolStripMenuItem.Enabled = true;
            loadSeedGenomeToolStripMenuItem.Enabled = true;
            savePopulationToolStripMenuItem.Enabled = false;
            saveBestGenomeToolStripMenuItem.Enabled = false;
        }

        private void UpdateUIState_PopulationReady()
        {
            // Disable anything to do with initialization now that we are initialized.
            cmbExperiments.Enabled = false;
            btnResetExperimentParameters.Enabled = false;
            btnCreateRandomPop.Enabled = false;

            // Display how many genomes & status.
            txtPopulationStatus.Text = $"{_neatPop.GenomeList.Count:D0} genomes ready";
            txtPopulationStatus.BackColor = Color.Orange;

            // Enable search control buttons.
            btnSearchStart.Enabled = true;
            btnSearchStop.Enabled = false;
            btnSearchReset.Enabled = true;

            // Parameter fields enabled (apart from population creation params)
            txtPopulationSize.Enabled = false;
            txtInitialInterconnectionsProportion.Enabled = false;
            txtElitismProportion.Enabled = true;
            txtSelectionProportion.Enabled = true;
            txtOffspringAsexualProportion.Enabled = true;
            txtOffspringSexualProportion.Enabled = true;
            txtInterspeciesMatingProportion.Enabled = true;
            txtConnectionWeightMutationProbability.Enabled = true;
            txtAddNodeMutationProbability.Enabled = true;
            txtAddConnectionMutationProbability.Enabled = true;
            txtDeleteConnectionMutationProbability.Enabled = true;

            // Logging to file.
            gbxLogging.Enabled = true;

            // Menu bar (file).
            loadPopulationToolStripMenuItem.Enabled = false;
            loadSeedGenomesToolStripMenuItem.Enabled = false;
            loadSeedGenomeToolStripMenuItem.Enabled = false;
            savePopulationToolStripMenuItem.Enabled = true;
            saveBestGenomeToolStripMenuItem.Enabled = false;
        }

        #endregion

        #region Private Methods [Send Settings to UI]

        private void SendSettingsToUI(INeatExperiment<double> experiment)
        {
            SendSettingsToUI(experiment.NeatEvolutionAlgorithmSettings);
            SendSettingsToUI(experiment.ReproductionAsexualSettings);

            SetValue(txtPopulationSize, experiment.PopulationSize);
            SetValue(txtInitialInterconnectionsProportion, experiment.InitialInterconnectionsProportion);
        }

        private void SendSettingsToUI(NeatEvolutionAlgorithmSettings settings)
        {
            SetValue(txtSpeciesCount, settings.SpeciesCount);
            SetValue(txtElitismProportion, settings.ElitismProportion);
            SetValue(txtSelectionProportion, settings.SelectionProportion);
            SetValue(txtOffspringAsexualProportion, settings.OffspringAsexualProportion);
            SetValue(txtOffspringSexualProportion, settings.OffspringSexualProportion);
            SetValue(txtInterspeciesMatingProportion, settings.InterspeciesMatingProportion);
        }

        private void SendSettingsToUI(NeatReproductionAsexualSettings settings)
        {
            SetValue(txtConnectionWeightMutationProbability, settings.ConnectionWeightMutationProbability);
            SetValue(txtAddNodeMutationProbability, settings.AddNodeMutationProbability);
            SetValue(txtAddConnectionMutationProbability, settings.AddConnectionMutationProbability);
            SetValue(txtDeleteConnectionMutationProbability, settings.DeleteConnectionMutationProbability);
        }

        #endregion

        #region Private Methods [Get Settings from UI]

        private void GetSettingsFromUI(INeatExperiment<double> experiment)
        {
            GetSettingsFromUI(experiment.NeatEvolutionAlgorithmSettings);
            GetSettingsFromUI(experiment.ReproductionAsexualSettings);

            experiment.PopulationSize = GetValue(txtPopulationSize, experiment.PopulationSize);
            experiment.InitialInterconnectionsProportion = GetValue(txtInitialInterconnectionsProportion, experiment.InitialInterconnectionsProportion);
        }


        private void GetSettingsFromUI(NeatEvolutionAlgorithmSettings settings)
        {
            settings.SpeciesCount = GetValue(txtSpeciesCount, settings.SpeciesCount);
            settings.ElitismProportion = GetValue(txtSpeciesCount, settings.ElitismProportion);
            settings.SelectionProportion = GetValue(txtSpeciesCount, settings.SelectionProportion);
            settings.OffspringAsexualProportion = GetValue(txtSpeciesCount, settings.OffspringAsexualProportion);
            settings.OffspringSexualProportion = GetValue(txtSpeciesCount, settings.OffspringSexualProportion);
            settings.InterspeciesMatingProportion = GetValue(txtSpeciesCount, settings.InterspeciesMatingProportion);
        }

        private void GetSettingsFromUI(NeatReproductionAsexualSettings settings)
        {
            settings.ConnectionWeightMutationProbability = GetValue(txtConnectionWeightMutationProbability, settings.ConnectionWeightMutationProbability);
            settings.AddNodeMutationProbability = GetValue(txtAddNodeMutationProbability, settings.AddNodeMutationProbability);
            settings.AddConnectionMutationProbability = GetValue(txtAddConnectionMutationProbability, settings.AddConnectionMutationProbability);
            settings.DeleteConnectionMutationProbability = GetValue(txtDeleteConnectionMutationProbability, settings.DeleteConnectionMutationProbability);
        }

        #endregion
    }
}
