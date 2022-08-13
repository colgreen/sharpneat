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
        if(cmbExperiments.SelectedItem is ExperimentInfo expInfo)
        {
            if(!string.IsNullOrEmpty(expInfo.DescriptionFile) && File.Exists(expInfo.DescriptionFile))
            {
                string description = File.ReadAllText(expInfo.DescriptionFile);
                MessageBox.Show(description, "Experiment Description");
            }
        }
    }

    private void btnLoadExperimentDefaultParameters_Click(object sender, EventArgs e)
    {
        _neatExperiment = CreateAndConfigureExperiment((ExperimentInfo)cmbExperiments.SelectedItem);
        SendSettingsToUI(_neatExperiment);
    }

    private void btnCreateRandomPop_Click(object sender, EventArgs e)
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

    private void btnSearchStart_Click(object sender, EventArgs e)
    {
        if(_eaRunner is not null)
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

        _eaRunner = new EvolutionAlgorithmRunner(
            ea,
            UpdateScheme.CreateTimeSpanUpdateScheme(TimeSpan.FromSeconds(1)));

        // Attach event listeners.
        _eaRunner.UpdateEvent += _eaRunner_UpdateEvent;

        // Start the algorithm & update GUI state.
        _eaRunner.StartOrResume();
        UpdateUIState();
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
        Logger.Clear();
        UpdateUIState();
        UpdateUIState_ResetStats();

        // Clear/reset child forms (those that are open).
        if(_bestGenomeForm is not null) _bestGenomeForm.Genome = null;

        // Time series forms.
        if(_fitnessTimeSeriesForm is not null) _fitnessTimeSeriesForm.Clear();
        if(_complexityTimeSeriesForm is not null) _complexityTimeSeriesForm.Clear();
        if(_evalsPerSecTimeSeriesForm is not null) _evalsPerSecTimeSeriesForm.Clear();

        // Rankings forms.
        if(_speciesSizeRankForm is not null) _speciesSizeRankForm.Clear();
        if(_speciesFitnessRankForm is not null) _speciesFitnessRankForm.Clear();
        if(_speciesComplexityRankForm is not null) _speciesComplexityRankForm.Clear();
        if(_genomeFitnessRankForm is not null) _genomeFitnessRankForm.Clear();
        if(_genomeComplexityRankForm is not null) _genomeComplexityRankForm.Clear();

        // Take the opportunity to clean-up the heap.
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
    }

    private void btnCopyLogToClipboard_Click(object sender, EventArgs e)
    {
        StringBuilder sb = new();
        foreach(Logger.LogItem item in lbxLog.Items)
        {
            sb.AppendLine(item.Message);
        }

        if(sb.Length > 0)
        {
            Clipboard.SetText(sb.ToString());
        }
    }

    private INeatExperiment<double> GetNeatExperiment()
    {
        // Create a new experiment instance if one has not already been created.
        _neatExperiment ??= CreateAndConfigureExperiment((ExperimentInfo)cmbExperiments.SelectedItem);

        // Read settings from the UI into the experiment instance, and return.
        GetSettingsFromUI(_neatExperiment);
        return _neatExperiment;
    }
}
