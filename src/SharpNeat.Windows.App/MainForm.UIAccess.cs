using System;
using System.Drawing;
using SharpNeat.EvolutionAlgorithm.Runner;
using SharpNeat.Experiments;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Reproduction.Asexual;
using static SharpNeat.Windows.App.UIAccessUtils;

namespace SharpNeat.Windows.App
{
    partial class MainForm
    {
        #region Private Methods [UpdateUIState Subroutines]

        private void UpdateUIState_NoPopulation()
        {
            // Enable experiment selection and initialization buttons.
            cmbExperiments.Enabled = true;
            btnLoadExperimentDefaultParameters.Enabled = true;
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
            btnLoadExperimentDefaultParameters.Enabled = false;
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

        private void UpdateUIState_ResetStats()
        {
            txtSearchStatsMode.Text = string.Empty;
            txtSearchStatsMode.BackColor = Color.LightSkyBlue;
            txtStatsGeneration.Text = string.Empty;
            txtStatsBest.Text = string.Empty;
            txtStatsAlternativeFitness.Text= string.Empty;
            txtStatsMean.Text = string.Empty;
            txtSpecieChampMean.Text = string.Empty;
            txtStatsTotalEvals.Text = string.Empty;
            txtStatsEvalsPerSec.Text = string.Empty;
            txtStatsBestGenomeComplx.Text =string.Empty;
            txtStatsMeanGenomeComplx.Text = string.Empty;
            txtStatsMaxGenomeComplx.Text = string.Empty;            
            txtStatsTotalOffspringCount.Text = string.Empty;
            txtStatsAsexualOffspringCount.Text = string.Empty;
            txtStatsCrossoverOffspringCount.Text = string.Empty;
            txtStatsInterspeciesOffspringCount.Text = string.Empty;
        }

        private void UpdateUIState_EaReadyPaused()
        {
            // Disable anything to do with initialization now that we are initialized.
            cmbExperiments.Enabled = false;
            btnLoadExperimentDefaultParameters.Enabled = false;
            btnCreateRandomPop.Enabled = false;

            // Display how many genomes & status.
            txtPopulationStatus.Text = $"{_neatPop.GenomeList.Count:D0} genomes paused.";
            txtPopulationStatus.BackColor = Color.Orange;

            // Search control buttons.
            btnSearchStart.Enabled = true;
            btnSearchStop.Enabled = false;
            btnSearchReset.Enabled = true;

            // Parameter fields (disable).
            txtPopulationSize.Enabled = false;
            txtInitialInterconnectionsProportion.Enabled = false;
            txtElitismProportion.Enabled = false;
            txtSelectionProportion.Enabled = false;
            txtOffspringAsexualProportion.Enabled = false;
            txtOffspringSexualProportion.Enabled = false;
            txtInterspeciesMatingProportion.Enabled = false;
            txtConnectionWeightMutationProbability.Enabled = false;
            txtAddNodeMutationProbability.Enabled = false;
            txtAddConnectionMutationProbability.Enabled = false;
            txtDeleteConnectionMutationProbability.Enabled = false;

            // Logging to file.
            gbxLogging.Enabled = true;

            // Menu bar.
            loadPopulationToolStripMenuItem.Enabled = false;
            loadSeedGenomesToolStripMenuItem.Enabled = false;
            loadSeedGenomeToolStripMenuItem.Enabled = false;
            savePopulationToolStripMenuItem.Enabled = true;
            // TODO: saveBestGenomeToolStripMenuItem.Enabled = (_eaRunner.CurrentChampGenome != null);
        }

        private void UpdateUIState_EaRunning()
        {
            // Disable anything to do with initialization now that we are initialized.
            cmbExperiments.Enabled = false;
            btnLoadExperimentDefaultParameters.Enabled = false;
            btnCreateRandomPop.Enabled = false;

            // Display how many genomes & status.
            txtPopulationStatus.Text = $"{_neatPop.GenomeList.Count:D0} genomes running";
            txtPopulationStatus.BackColor = Color.LightGreen;

            // Search control buttons.
            btnSearchStart.Enabled = false;
            btnSearchStop.Enabled = true;
            btnSearchReset.Enabled = false;

            // Parameter fields (disable).
            txtPopulationSize.Enabled = false;
            txtInitialInterconnectionsProportion.Enabled = false;
            txtElitismProportion.Enabled = false;
            txtSelectionProportion.Enabled = false;
            txtOffspringAsexualProportion.Enabled = false;
            txtOffspringSexualProportion.Enabled = false;
            txtInterspeciesMatingProportion.Enabled = false;
            txtConnectionWeightMutationProbability.Enabled = false;
            txtAddNodeMutationProbability.Enabled = false;
            txtAddConnectionMutationProbability.Enabled = false;
            txtDeleteConnectionMutationProbability.Enabled = false;

            // Logging to file.
            gbxLogging.Enabled = false;

            // Menu bar.
            loadPopulationToolStripMenuItem.Enabled = false;
            loadSeedGenomesToolStripMenuItem.Enabled = false;
            loadSeedGenomeToolStripMenuItem.Enabled = false;
            savePopulationToolStripMenuItem.Enabled = false;
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
            settings.ElitismProportion = GetValue(txtElitismProportion, settings.ElitismProportion);
            settings.SelectionProportion = GetValue(txtSelectionProportion, settings.SelectionProportion);
            settings.OffspringAsexualProportion = GetValue(txtOffspringAsexualProportion, settings.OffspringAsexualProportion);
            settings.OffspringSexualProportion = GetValue(txtOffspringSexualProportion, settings.OffspringSexualProportion);
            settings.InterspeciesMatingProportion = GetValue(txtInterspeciesMatingProportion, settings.InterspeciesMatingProportion);
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
