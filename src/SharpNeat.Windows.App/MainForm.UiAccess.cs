// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Globalization;
using SharpNeat.EvolutionAlgorithm.Runner;
using SharpNeat.Experiments;
using SharpNeat.Neat;
using SharpNeat.Neat.ComplexityRegulation;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.EvolutionAlgorithm.Settings;
using SharpNeat.Neat.Reproduction.Asexual;
using static SharpNeat.Windows.App.UiAccessUtils;

namespace SharpNeat.Windows.App;

partial class MainForm
{
    #region Private Methods

    private void UpdateUiState()
    {
        if(_eaRunner is null)
        {
            if(_neatPop is null)
                UpdateUiState_NoPopulation();
            else
                UpdateUiState_PopulationReady();
        }
        else
        {
            switch(_eaRunner.RunState)
            {
                case RunState.Ready:
                case RunState.Paused:
                    UpdateUiState_EaReadyPaused();
                    break;
                case RunState.Running:
                    UpdateUiState_EaRunning();
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected RunState [{_eaRunner.RunState}]");
            }
        }
    }

    #endregion

    #region Private Methods [UpdateUiState Subroutines]

    private void UpdateUiState_NoPopulation()
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
        SetParameterFieldsEnabledState(true);

        // Logging to file.
        gbxLogging.Enabled = true;

        // Menu bar (file).
        loadPopulationToolStripMenuItem.Enabled = true;
        loadSeedGenomesToolStripMenuItem.Enabled = true;
        loadSeedGenomeToolStripMenuItem.Enabled = true;
        savePopulationToolStripMenuItem.Enabled = false;
        saveBestGenomeToolStripMenuItem.Enabled = false;
    }

    private void UpdateUiState_PopulationReady()
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
        SetParameterFieldsEnabledState(true);
        txtPopulationSize.Enabled = false;
        txtInitialInterconnectionsProportion.Enabled = false;

        // Logging to file.
        gbxLogging.Enabled = true;

        // Menu bar (file).
        loadPopulationToolStripMenuItem.Enabled = false;
        loadSeedGenomesToolStripMenuItem.Enabled = false;
        loadSeedGenomeToolStripMenuItem.Enabled = false;
        savePopulationToolStripMenuItem.Enabled = true;
        saveBestGenomeToolStripMenuItem.Enabled = true;
    }

    private void UpdateUiState_EaReadyPaused()
    {
        // Disable anything to do with initialization now that we are initialized.
        cmbExperiments.Enabled = false;
        btnLoadExperimentDefaultParameters.Enabled = false;
        btnCreateRandomPop.Enabled = false;

        // Display how many genomes & status.
        txtPopulationStatus.Text = $"{_neatPop.GenomeList.Count:D0} genomes - paused.";
        txtPopulationStatus.BackColor = Color.Orange;

        // Search control buttons.
        btnSearchStart.Enabled = true;
        btnSearchStop.Enabled = false;
        btnSearchReset.Enabled = true;

        // Parameter fields (disable).
        SetParameterFieldsEnabledState(false);

        // Logging to file.
        gbxLogging.Enabled = true;

        // Menu bar.
        loadPopulationToolStripMenuItem.Enabled = false;
        loadSeedGenomesToolStripMenuItem.Enabled = false;
        loadSeedGenomeToolStripMenuItem.Enabled = false;
        savePopulationToolStripMenuItem.Enabled = true;
        saveBestGenomeToolStripMenuItem.Enabled = true;
    }

    private void UpdateUiState_EaRunning()
    {
        // Disable anything to do with initialization now that we are initialized.
        cmbExperiments.Enabled = false;
        btnLoadExperimentDefaultParameters.Enabled = false;
        btnCreateRandomPop.Enabled = false;

        // Display how many genomes & status.
        txtPopulationStatus.Text = $"{_neatPop.GenomeList.Count:D0} genomes - running";
        txtPopulationStatus.BackColor = Color.LightGreen;

        // Search control buttons.
        btnSearchStart.Enabled = false;
        btnSearchStop.Enabled = true;
        btnSearchReset.Enabled = false;

        // Parameter fields (disable).
        SetParameterFieldsEnabledState(false);

        // Logging to file.
        gbxLogging.Enabled = false;

        // Menu bar.
        loadPopulationToolStripMenuItem.Enabled = false;
        loadSeedGenomesToolStripMenuItem.Enabled = false;
        loadSeedGenomeToolStripMenuItem.Enabled = false;
        savePopulationToolStripMenuItem.Enabled = false;
        saveBestGenomeToolStripMenuItem.Enabled = false;
    }

    private void SetParameterFieldsEnabledState(bool enabled)
    {
        txtPopulationSize.Enabled = enabled;
        txtInitialInterconnectionsProportion.Enabled = enabled;
        txtSpeciesCount.Enabled = enabled;
        txtElitismProportion.Enabled = enabled;
        txtSelectionProportion.Enabled = enabled;
        txtOffspringAsexualProportion.Enabled = enabled;
        txtOffspringRecombinationProportion.Enabled = enabled;
        txtInterspeciesMatingProportion.Enabled = enabled;
        txtConnectionWeightMutationProbability.Enabled = enabled;
        txtAddNodeMutationProbability.Enabled = enabled;
        txtAddConnectionMutationProbability.Enabled = enabled;
        txtDeleteConnectionMutationProbability.Enabled = enabled;
    }

    private void UpdateUiState_EaStats()
    {
        NeatEvolutionAlgorithmStatistics eaStats = (NeatEvolutionAlgorithmStatistics)_eaRunner.EA.Stats;
        NeatPopulationStatistics popStats = _neatPop.NeatPopulationStats;

        // Search mode.
        ComplexityRegulationMode mode = ((NeatEvolutionAlgorithm<double>)_eaRunner.EA).ComplexityRegulationMode;
        txtSearchStatsMode.Text = mode.ToString();
        txtSearchStatsMode.BackColor = mode switch
        {
            ComplexityRegulationMode.Complexifying => Color.LightSkyBlue,
            _ => Color.LightSkyBlue
        };

        txtStatsGeneration.Text = eaStats.Generation.ToString("N0", CultureInfo.CurrentCulture);
        txtStatsBest.Text = popStats.BestFitness.PrimaryFitness.ToString(CultureInfo.CurrentCulture);

        // Auxiliary fitness info.
        double[] auxFitnessArr = popStats.BestFitness.AuxFitnessScores;
        if(auxFitnessArr != null && auxFitnessArr.Length > 0)
        {
            txtStatsAlternativeFitness.Text = auxFitnessArr[0].ToString("#.######", CultureInfo.CurrentCulture);
        }
        else
        {
            txtStatsAlternativeFitness.Text = "";
        }

        txtStatsMean.Text = popStats.MeanFitness.ToString("#.######", CultureInfo.CurrentCulture);
        txtSpeciesChampsMean.Text = popStats.AverageSpeciesBestFitness.ToString("#.######", CultureInfo.CurrentCulture);
        txtStatsTotalEvals.Text = eaStats.TotalEvaluationCount.ToString("N0", CultureInfo.CurrentCulture);
        txtStatsEvalsPerSec.Text = eaStats.EvaluationsPerSec.ToString("##,#.##", CultureInfo.CurrentCulture);
        txtStatsBestGenomeComplx.Text = popStats.BestComplexity.ToString("N0", CultureInfo.CurrentCulture);
        txtStatsMeanGenomeComplx.Text = popStats.MeanComplexity.ToString("#.##", CultureInfo.CurrentCulture);
        txtStatsMaxGenomeComplx.Text = popStats.MaxComplexity.ToString("N0", CultureInfo.CurrentCulture);

        ulong totalOffspringCount = eaStats.TotalOffspringCount;
        if(totalOffspringCount > 0)
        {
            txtStatsTotalOffspringCount.Text = totalOffspringCount.ToString("N0", CultureInfo.CurrentCulture);
            txtStatsAsexualOffspringCount.Text = string.Format(CultureInfo.CurrentCulture,"{0:N0} ({1:P})", eaStats.TotalOffspringAsexualCount, (eaStats.TotalOffspringAsexualCount / (double)totalOffspringCount));
            txtStatsCrossoverOffspringCount.Text = string.Format(CultureInfo.CurrentCulture, "{0:N0} ({1:P})", eaStats.TotalOffspringRecombinationCount, (eaStats.TotalOffspringRecombinationCount / (double)totalOffspringCount));
            txtStatsInterspeciesOffspringCount.Text = string.Format(CultureInfo.CurrentCulture, "{0:N0} ({1:P})", eaStats.TotalOffspringInterspeciesCount, (eaStats.TotalOffspringInterspeciesCount/(double)totalOffspringCount));
        }
    }

    private void UpdateUiState_ResetStats()
    {
        txtSearchStatsMode.Text = string.Empty;
        txtSearchStatsMode.BackColor = Color.LightSkyBlue;
        txtStatsGeneration.Text = string.Empty;
        txtStatsBest.Text = string.Empty;
        txtStatsAlternativeFitness.Text= string.Empty;
        txtStatsMean.Text = string.Empty;
        txtSpeciesChampsMean.Text = string.Empty;
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

    #endregion

    #region Private Methods [Send Settings to UI]

    private void SendSettingsToUi(
        INeatExperiment<double> experiment)
    {
        SendSettingsToUi(experiment.EvolutionAlgorithmSettings);
        SendSettingsToUi(experiment.AsexualReproductionSettings);

        SetValue(txtPopulationSize, experiment.PopulationSize);
        SetValue(txtInitialInterconnectionsProportion, experiment.InitialInterconnectionsProportion);
    }

    private void SendSettingsToUi(
        NeatEvolutionAlgorithmSettings settings)
    {
        SetValue(txtSpeciesCount, settings.SpeciesCount);
        SetValue(txtElitismProportion, settings.ElitismProportion);
        SetValue(txtSelectionProportion, settings.SelectionProportion);
        SetValue(txtOffspringAsexualProportion, settings.OffspringAsexualProportion);
        SetValue(txtOffspringRecombinationProportion, settings.OffspringRecombinationProportion);
        SetValue(txtInterspeciesMatingProportion, settings.InterspeciesMatingProportion);
    }

    private void SendSettingsToUi(
        NeatAsexualReproductionSettings settings)
    {
        SetValue(txtConnectionWeightMutationProbability, settings.ConnectionWeightMutationProbability);
        SetValue(txtAddNodeMutationProbability, settings.AddNodeMutationProbability);
        SetValue(txtAddConnectionMutationProbability, settings.AddConnectionMutationProbability);
        SetValue(txtDeleteConnectionMutationProbability, settings.DeleteConnectionMutationProbability);
    }

    #endregion

    #region Private Methods [Get Settings from UI]

    private void GetSettingsFromUi(
        INeatExperiment<double> experiment)
    {
        GetSettingsFromUi(experiment.EvolutionAlgorithmSettings);
        GetSettingsFromUi(experiment.AsexualReproductionSettings);

        experiment.PopulationSize = GetValue(txtPopulationSize, experiment.PopulationSize);
        experiment.InitialInterconnectionsProportion = GetValue(txtInitialInterconnectionsProportion, experiment.InitialInterconnectionsProportion);
    }

    private void GetSettingsFromUi(
        NeatEvolutionAlgorithmSettings settings)
    {
        settings.SpeciesCount = GetValue(txtSpeciesCount, settings.SpeciesCount);
        settings.ElitismProportion = GetValue(txtElitismProportion, settings.ElitismProportion);
        settings.SelectionProportion = GetValue(txtSelectionProportion, settings.SelectionProportion);

        double offspringAsexualProportion = GetValue(txtOffspringAsexualProportion, settings.OffspringAsexualProportion);
        double offspringRecombinationProportion = GetValue(txtOffspringRecombinationProportion, settings.OffspringRecombinationProportion);

        Normalize(ref offspringAsexualProportion, ref offspringRecombinationProportion);

        settings.OffspringAsexualProportion = offspringAsexualProportion;
        settings.OffspringRecombinationProportion = offspringRecombinationProportion;

        settings.InterspeciesMatingProportion = GetValue(txtInterspeciesMatingProportion, settings.InterspeciesMatingProportion);
    }

    private void GetSettingsFromUi(
        NeatAsexualReproductionSettings settings)
    {
        double connectionWeightMutationProbability = GetValue(txtConnectionWeightMutationProbability, settings.ConnectionWeightMutationProbability);
        double addNodeMutationProbability = GetValue(txtAddNodeMutationProbability, settings.AddNodeMutationProbability);
        double addConnectionMutationProbability = GetValue(txtAddConnectionMutationProbability, settings.AddConnectionMutationProbability);
        double deleteConnectionMutationProbability = GetValue(txtDeleteConnectionMutationProbability, settings.DeleteConnectionMutationProbability);

        Normalize(
            ref connectionWeightMutationProbability, ref addNodeMutationProbability,
            ref addConnectionMutationProbability, ref deleteConnectionMutationProbability);

        settings.ConnectionWeightMutationProbability = connectionWeightMutationProbability;
        settings.AddNodeMutationProbability = addNodeMutationProbability;
        settings.AddConnectionMutationProbability = addConnectionMutationProbability;
        settings.DeleteConnectionMutationProbability = deleteConnectionMutationProbability;
    }

    #endregion

    #region Private Static Methods

    private static void Normalize(ref double x1, ref double x2)
    {
        double sum = x1 + x2;
        x1 /= sum;
        x2 /= sum;
    }

    private static void Normalize(ref double x1, ref double x2, ref double x3, ref double x4)
    {
        double sum = x1 + x2 + x3 + x4;
        x1 /= sum;
        x2 /= sum;
        x3 /= sum;
        x4 /= sum;
    }

    #endregion
}
