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
using SharpNeat.EvolutionAlgorithm.Runner;
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

        private EvolutionAlgorithmRunner _eaRunner;


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

        private void btnLoadExperimentDefaultParameters_Click(object sender,EventArgs e)
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






        private void btnSearchStart_Click(object sender,EventArgs e)
        {
            if(_eaRunner is object)
            {   // Resume existing EA & update GUI state.
                _eaRunner.StartOrResume();
                UpdateUIState();
                return;
            }

            // Get the current neat experiment, with parameters set from the UI.
            INeatExperiment<double> neatExperiment = GetNeatExperiment();

            // Create evolution algorithm and runner.
            NeatEvolutionAlgorithm<double> ea = NeatUtils.CreateNeatEvolutionAlgorithm(neatExperiment, _neatPop);
            ea.Initialise();

            _eaRunner = new EvolutionAlgorithmRunner(ea, UpdateScheme.CreateTimeSpanUpdateScheme(TimeSpan.FromSeconds(1)));

            // Attach event listeners.
            _eaRunner.UpdateEvent += _eaRunner_UpdateEvent;
            



            // TODO: Implement.



            // Start the algorithm & update GUI state.
            _eaRunner.StartOrResume();
            UpdateUIState();
        }

        private void _eaRunner_UpdateEvent(object sender, EventArgs e)
        {
            // Switch to the UI thread, if not already on that thread.
            if(this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate()
                {
                    _eaRunner_UpdateEvent(sender, e);
                }));
                return;
            }

            // TODO: Implement.



            // Update stats fields.
            //UpdateUIState_EaStats();

            // Write entry to log.
            __log.Info(string.Format("gen={0:N0} bestFitness={1:N6}", _eaRunner.EA.Stats.Generation, _neatPop.Stats.BestFitness.PrimaryFitness));
        }




        private void btnSearchStop_Click(object sender,EventArgs e)
        {

        }






        private void btnSearchReset_Click(object sender,EventArgs e)
        {
            _neatPop = null;
            Logger.Clear();
            UpdateUIState();
            UpdateUIState_ResetStats();
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


        private void UpdateUIState()
        {
            if(_eaRunner is null)
            {
                if(_neatPop is null) {
                    UpdateUIState_NoPopulation();
                } else {
                    UpdateUIState_PopulationReady();
                }
            }
            else
            {
                switch(_eaRunner.RunState)
                {
                    case RunState.Ready:
                    case RunState.Paused:
                        UpdateUIState_EaReadyPaused();
                        break;
                    case RunState.Running:
                        UpdateUIState_EaRunning();
                        break;
                    default:
                        throw new ApplicationException($"Unexpected RunState [{_eaRunner.RunState}]");
                }
            }


        }
   
    }
}
