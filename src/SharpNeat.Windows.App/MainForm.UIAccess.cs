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
using SharpNeat.EvolutionAlgorithm.Runner;
using SharpNeat.Experiments;
using SharpNeat.Neat;
using SharpNeat.Neat.ComplexityRegulation;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Reproduction.Asexual;
using static SharpNeat.Windows.App.UIAccessUtils;

namespace SharpNeat.Windows.App
{
    partial class MainForm
    {
        // Fields used to calculate evaluations per second, on each successive update.
        ulong _evalCountPrev;
        DateTime _evalCountPrevSampleTime = DateTime.MinValue;

        #region Private Methods

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

        #endregion

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

        private void UpdateUIState_EaReadyPaused()
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

        private void UpdateUIState_EaRunning()
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
            txtElitismProportion.Enabled = enabled;
            txtSelectionProportion.Enabled = enabled;
            txtOffspringAsexualProportion.Enabled = enabled;
            txtOffspringSexualProportion.Enabled = enabled;
            txtInterspeciesMatingProportion.Enabled = enabled;
            txtConnectionWeightMutationProbability.Enabled = enabled;
            txtAddNodeMutationProbability.Enabled = enabled;
            txtAddConnectionMutationProbability.Enabled = enabled;
            txtDeleteConnectionMutationProbability.Enabled = enabled;

        }

        private void UpdateUIState_EaStats()
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

            txtStatsGeneration.Text = eaStats.Generation.ToString("N0");
            txtStatsBest.Text = popStats.BestFitness.PrimaryFitness.ToString();

            // Auxiliary fitness info.
            double[] auxFitnessArr = popStats.BestFitness.AuxFitnessScores;
            if(auxFitnessArr != null && auxFitnessArr.Length > 0) {
                txtStatsAlternativeFitness.Text = auxFitnessArr[0].ToString("#.######");
            } else {
                txtStatsAlternativeFitness.Text = "";
            }

            txtStatsMean.Text = popStats.MeanFitness.ToString("#.######");
            txtSpeciesChampsMean.Text = popStats.AverageSpeciesBestFitness.ToString("#.######");
            txtStatsTotalEvals.Text = eaStats.TotalEvaluationCount.ToString("N0");

            // Calculate/update evaluations per second stat.
            // Skip this calc for the first call here, as we need to two successive calls to calc evaluation per sec.
            if(_evalCountPrevSampleTime == DateTime.MinValue)
            {
                // Record the count and sample time ready for the next call to this subroutine.
                _evalCountPrev = eaStats.TotalEvaluationCount;
                _evalCountPrevSampleTime = eaStats.SampleTime;
            }
            else
            {
                // Calc elapsed time since the previous update to this state. If it is less than one second ago then skip the update, 
                // as the timespan may be very short, thus giving an unrepresentative evals per second value.
                TimeSpan elapsed = eaStats.SampleTime - _evalCountPrevSampleTime;
                if(elapsed > TimeSpan.FromSeconds(1))
                {
                    double countDelta = eaStats.TotalEvaluationCount - _evalCountPrev;
                    double evalsPerSec = (countDelta * 1e7) / elapsed.Ticks;
                    txtStatsEvalsPerSec.Text = evalsPerSec.ToString("##,#.##");

                    // Record the count and sample time ready for the next call to this subroutine.
                    _evalCountPrev = eaStats.TotalEvaluationCount;
                    _evalCountPrevSampleTime = eaStats.SampleTime;
                }
            }

            txtStatsBestGenomeComplx.Text = popStats.BestComplexity.ToString("N0");
            txtStatsMeanGenomeComplx.Text = popStats.MeanComplexity.ToString("#.##");
            txtStatsMaxGenomeComplx.Text = popStats.MaxComplexity.ToString("N0");

            ulong totalOffspringCount = eaStats.TotalOffspringCount;
            if(totalOffspringCount > 0)
            { 
                txtStatsTotalOffspringCount.Text = totalOffspringCount.ToString("N0");
                txtStatsAsexualOffspringCount.Text = string.Format("{0:N0} ({1:P})", eaStats.TotalOffspringAsexualCount, (eaStats.TotalOffspringAsexualCount / (double)totalOffspringCount));
                txtStatsCrossoverOffspringCount.Text = string.Format("{0:N0} ({1:P})", eaStats.TotalOffspringSexualCount, (eaStats.TotalOffspringSexualCount / (double)totalOffspringCount));
                txtStatsInterspeciesOffspringCount.Text = string.Format("{0:N0} ({1:P})", eaStats.TotalOffspringInterspeciesCount, (eaStats.TotalOffspringInterspeciesCount/(double)totalOffspringCount));
            }
        }

        private void UpdateUIState_ResetStats()
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
