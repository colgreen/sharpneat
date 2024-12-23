// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Text;
using SharpNeat.EvolutionAlgorithm.Runner;
using SharpNeat.Experiments;
using SharpNeat.Neat;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Genome;
using SharpNeat.Windows.App.Experiments;
using static SharpNeat.Windows.App.AppUtils;

#pragma warning disable IDE1006 // Naming Styles. Allow naming convention for Windows.Forms event handlers.

namespace SharpNeat.Windows.App;

partial class MainForm
{
    private void btnExperimentInfo_Click(object sender, EventArgs e)
    {
        if(cmbExperiments.SelectedItem is ExperimentInfo expInfo
            && !string.IsNullOrEmpty(expInfo.DescriptionFile) 
            && File.Exists(expInfo.DescriptionFile))
        {
            string description = File.ReadAllText(expInfo.DescriptionFile);
            MessageBox.Show(description, "Experiment Description");
        }
    }

    private void btnLoadExperimentDefaultParameters_Click(object sender, EventArgs e)
    {
        _neatExperiment = CreateAndConfigureExperiment(
            (ExperimentInfo)cmbExperiments.SelectedItem);

        SendSettingsToUi(_neatExperiment);
    }

    private void btnCreateRandomPop_Click(object sender, EventArgs e)
    {
        INeatExperiment<double> neatExperiment = GetNeatExperiment();
        MetaNeatGenome<double> metaNeatGenome = neatExperiment.CreateMetaNeatGenome();

        // Create an initial population of genomes.
        _neatPop = NeatPopulationFactory<double>.CreatePopulation(
            metaNeatGenome,
            connectionsProportion: neatExperiment.InitialInterconnectionsProportion,
            popSize: neatExperiment.PopulationSize);

        // Update UI.
        UpdateUiState();
    }

    private void btnSearchStart_Click(object sender, EventArgs e)
    {
        if(_eaRunner is not null)
        {   
            // Resume existing EA, and update GUI state.
            _eaRunner.StartOrResume();
            UpdateUiState();
            return;
        }

        // Get the current neat experiment, with parameters set from the UI.
        INeatExperiment<double> neatExperiment = GetNeatExperiment();

        // Create evolution algorithm and runner.
        NeatEvolutionAlgorithm<double> ea = NeatEvolutionAlgorithmFactory.CreateEvolutionAlgorithm(neatExperiment, _neatPop);
        ea.Initialise();

        _eaRunner = new EvolutionAlgorithmRunner(
            ea,
            UpdateScheme.CreateTimeSpanUpdateScheme(TimeSpan.FromSeconds(1)));

        // Attach event listeners.
        _eaRunner.UpdateEvent += _eaRunner_UpdateEvent;

        // Start the algorithm & update GUI state.
        _eaRunner.StartOrResume();
        UpdateUiState();
    }

    private void btnSearchStop_Click(object sender, EventArgs e)
    {
        _eaRunner.RequestPause();
    }

    private void btnSearchReset_Click(object sender, EventArgs e)
    {
        // Clear down any EA related state.
        if(_eaRunner is not null)
        {
            // Note. Dispose here will wait for the termination of the background thread use to run the EA.
            _eaRunner.Dispose();
            _eaRunner = null;
        }
        _neatPop = null;

        // Reset/update UI state.
        UiLogger.Clear();
        UpdateUiState();
        UpdateUiState_ResetStats();

        // Clear/reset child forms (those that are open).
        if(_bestGenomeForm is not null) 
            _bestGenomeForm.Genome = null;

        // Time series forms.
        _fitnessTimeSeriesForm?.Clear();
        _complexityTimeSeriesForm?.Clear();
        _evalsPerSecTimeSeriesForm?.Clear();

        // Rankings forms.
        _speciesSizeRankForm?.Clear();
        _speciesFitnessRankForm?.Clear();
        _speciesComplexityRankForm?.Clear();
        _genomeFitnessRankForm?.Clear();
        _genomeComplexityRankForm?.Clear();

        // Take the opportunity to clean-up the heap.
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
    }

    private void btnCopyLogToClipboard_Click(object sender, EventArgs e)
    {
        StringBuilder sb = new();
        foreach(UiLogger.LogItem item in lbxLog.Items)
        {
            sb.AppendLine(item.Message);
        }

        if(sb.Length > 0)
        {
            Clipboard.SetText(sb.ToString());
        }
    }
}
