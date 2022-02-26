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
using System.Buffers;
using System.Globalization;
using System.Text;
using System.Text.Json;
using log4net;
using SharpNeat.EvolutionAlgorithm.Runner;
using SharpNeat.Experiments;
using SharpNeat.Experiments.Windows;
using SharpNeat.Neat;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Genome;
using SharpNeat.Windows.App.Experiments;
using SharpNeat.Windows.App.Forms;
using SharpNeat.Windows.App.Forms.TimeSeries;
using static SharpNeat.Windows.App.AppUtils;

#pragma warning disable IDE1006 // Naming Styles. Allow naming convention for Windows.Forms event handlers.

namespace SharpNeat.Windows.App;

/// <summary>
/// SharpNEAT main GUI window.
/// </summary>
public partial class MainForm : Form
{
    private static readonly ILog __log = LogManager.GetLogger(typeof(MainForm));

    #region Instance Fields

    // The current NEAT experiment.
    private INeatExperiment<double> _neatExperiment;
    private NeatPopulation<double> _neatPop;
    private EvolutionAlgorithmRunner _eaRunner;
    private IExperimentUI _experimentUI;
    private GenomeForm _bestGenomeForm;

    // Time series forms.
    private FitnessTimeSeriesForm _fitnessTimeSeriesForm;
    private ComplexityTimeSeriesForm _complexityTimeSeriesForm;
    private EvalsPerSecTimeSeriesForm _evalsPerSecTimeSeriesForm;

    // Rankings forms.
    private RankGraphForm _speciesSizeRankForm;
    private RankPairGraphForm _speciesFitnessRankForm;
    private RankPairGraphForm _speciesComplexityRankForm;
    private RankGraphForm _genomeFitnessRankForm;
    private RankGraphForm _genomeComplexityRankForm;

    #endregion

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
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, ReadCommentHandling = JsonCommentHandling.Skip };
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

        _eaRunner = new EvolutionAlgorithmRunner(ea, UpdateScheme.CreateTimeSpanUpdateScheme(TimeSpan.FromSeconds(1)));

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
        if(_bestGenomeForm is not null) { _bestGenomeForm.Genome = null; }

        // Time series forms.
        if(_fitnessTimeSeriesForm is not null) { _fitnessTimeSeriesForm.Clear(); }
        if(_complexityTimeSeriesForm is not null) { _complexityTimeSeriesForm.Clear(); }
        if(_evalsPerSecTimeSeriesForm is not null) { _evalsPerSecTimeSeriesForm.Clear(); }

        // Rankings forms.
        if(_speciesSizeRankForm is not null) { _speciesSizeRankForm.Clear(); }
        if(_speciesFitnessRankForm is not null) { _speciesFitnessRankForm.Clear(); }
        if(_speciesComplexityRankForm is not null) { _speciesComplexityRankForm.Clear(); }
        if(_genomeFitnessRankForm is not null) { _genomeFitnessRankForm.Clear(); }
        if(_genomeComplexityRankForm is not null) { _genomeComplexityRankForm.Clear(); }

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

    #endregion

    #region Misc Event Handlers

    private void cmbExperiments_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Clear any existing references, as these are specific to each experiment.
        _neatExperiment = null;
        _experimentUI = null;

        // Close the genome form if it is open, as the content of this form is specific to each experiment.
        GenomeForm bestGenomeForm = _bestGenomeForm;
        if(bestGenomeForm is not null)
        {
            // Note. This will trigger the FormClosed event which will do further clean-up; Close() will also Dispose() the form.
            bestGenomeForm.Close();
        }
    }

    private void _eaRunner_UpdateEvent(object sender, EventArgs e)
    {
        if(_eaRunner == null || _eaRunner.RunState == RunState.Terminated)
            return;

        // Switch to the UI thread, if not already on that thread.
        if(this.InvokeRequired)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                _eaRunner_UpdateEvent(sender, e);
            }));
            return;
        }

        // TODO: Implement.

        // Update stats fields.
        UpdateUIState_EaStats();

        // Update child forms (those that are open).
        if(_bestGenomeForm is not null)
        {
            NeatEvolutionAlgorithm<double> neatEa = (NeatEvolutionAlgorithm<double>)(_eaRunner.EA);
            _bestGenomeForm.Genome = neatEa.Population.BestGenome;
        }

        // Time series forms.
        if(_fitnessTimeSeriesForm is not null)
            _fitnessTimeSeriesForm.UpdateData(_eaRunner.EA.Stats, _neatPop.NeatPopulationStats);

        if(_complexityTimeSeriesForm is not null)
            _complexityTimeSeriesForm.UpdateData(_eaRunner.EA.Stats, _neatPop.NeatPopulationStats);

        if(_evalsPerSecTimeSeriesForm is not null)
            _evalsPerSecTimeSeriesForm.UpdateData(_eaRunner.EA.Stats, _neatPop.NeatPopulationStats);

        // Rankings forms.
        if(_speciesSizeRankForm is not null)
        {
            double[] speciesSizeByRank = GetSpeciesSizeByRank(out int speciesCount);
            try
            {
                _speciesSizeRankForm.UpdateData(speciesSizeByRank.AsSpan(0, speciesCount));
            }
            finally
            {
                ArrayPool<double>.Shared.Return(speciesSizeByRank);
            }
        }

        if(_speciesFitnessRankForm is not null)
        {
            GetSpeciesFitnessByRank(out double[] bestFitnessByRank, out double[] meanFitnessSeries, out int speciesCount);
            try
            {
                _speciesFitnessRankForm.UpdateData(bestFitnessByRank.AsSpan(0, speciesCount), meanFitnessSeries.AsSpan(0, speciesCount));
            }
            finally
            {
                ArrayPool<double>.Shared.Return(bestFitnessByRank);
                ArrayPool<double>.Shared.Return(meanFitnessSeries);
            }
        }

        if(_speciesComplexityRankForm is not null)
        {
            GetSpeciesComplexityByRank(out double[] bestComplexityByRank, out double[] meanComplexitySeries, out int speciesCount);
            try
            {
                _speciesComplexityRankForm.UpdateData(bestComplexityByRank.AsSpan(0, speciesCount), meanComplexitySeries.AsSpan(0, speciesCount));
            }
            finally
            {
                ArrayPool<double>.Shared.Return(bestComplexityByRank);
                ArrayPool<double>.Shared.Return(meanComplexitySeries);
            }
        }

        if(_genomeFitnessRankForm is not null)
        {
            double[] genomeFitnessByRank = GetGenomeFitnessByRank(out int genomeCount);
            try
            {
                _genomeFitnessRankForm.UpdateData(genomeFitnessByRank.AsSpan(0, genomeCount));
            }
            finally
            {
                ArrayPool<double>.Shared.Return(genomeFitnessByRank);
            }
        }

        if(_genomeComplexityRankForm is not null)
        {
            double[] genomeComplexityByRank = GetGenomeComplexityByRank(out int genomeCount);
            try
            {
                _genomeComplexityRankForm.UpdateData(genomeComplexityByRank.AsSpan(0, genomeCount));
            }
            finally
            {
                ArrayPool<double>.Shared.Return(genomeComplexityByRank);
            }
        }

        // Write entry to log.
        __log.Info(string.Format("gen={0:N0} bestFitness={1:N6}", _eaRunner.EA.Stats.Generation, _neatPop.Stats.BestFitness.PrimaryFitness));

        if(_eaRunner.RunState == RunState.Paused)
            UpdateUIState_EaReadyPaused();
    }

    #endregion

    #region Private Methods

    private INeatExperiment<double> GetNeatExperiment()
    {
        // Create a new experiment instance if one has not already been created.
        if(_neatExperiment is null)
            _neatExperiment = CreateAndConfigureExperiment((ExperimentInfo)cmbExperiments.SelectedItem);

        // Read settings from the UI into the experiment instance, and return.
        GetSettingsFromUI(_neatExperiment);
        return _neatExperiment;
    }

    private IExperimentUI GetExperimentUI()
    {
        // Create a new experiment instance if one has not already been created.
        if(_experimentUI is null)
            _experimentUI = CreateAndConfigureExperimentUI((ExperimentInfo)cmbExperiments.SelectedItem);

        return _experimentUI;
    }

    #endregion

    #region Private Methods [Child Form Update Subroutines]

    private double[] GetSpeciesSizeByRank(out int count)
    {
        var speciesArr = _neatPop.SpeciesArray;
        count = speciesArr.Length;

        double[] speciesSizeByRank = ArrayPool<double>.Shared.Rent(count);

        for(int i=0; i < count; i++)
            speciesSizeByRank[i] = speciesArr[i].GenomeList.Count;

        // Sort size values (highest values first).
        Array.Sort(speciesSizeByRank, 0, count, Utils.ComparerDesc);
        return speciesSizeByRank;
    }

    private void GetSpeciesFitnessByRank(
        out double[] bestFitnessByRank,
        out double[] meanFitnessSeries,
        out int count)
    {
        var speciesArr = _neatPop.SpeciesArray;
        count = speciesArr.Length;

        bestFitnessByRank = ArrayPool<double>.Shared.Rent(count);
        meanFitnessSeries = ArrayPool<double>.Shared.Rent(count);

        for(int i=0; i < count; i++)
        {
            bestFitnessByRank[i] = speciesArr[i].GenomeList[0].FitnessInfo.PrimaryFitness;
            meanFitnessSeries[i] = speciesArr[i].Stats.MeanFitness;
        }

        // Sort best fitness values (highest values first).
        Array.Sort(bestFitnessByRank, meanFitnessSeries, 0, count, Utils.ComparerDesc);
    }

    private void GetSpeciesComplexityByRank(
        out double[] bestComplexityByRank,
        out double[] meanComplexitySeries,
        out int count)
    {
        var speciesArr = _neatPop.SpeciesArray;
        count = speciesArr.Length;

        bestComplexityByRank = ArrayPool<double>.Shared.Rent(count);
        meanComplexitySeries = ArrayPool<double>.Shared.Rent(count);

        for(int i=0; i < count; i++)
        {
            bestComplexityByRank[i] = speciesArr[i].GenomeList[0].Complexity;
            meanComplexitySeries[i] = speciesArr[i].CalcMeanComplexity();
        }

        // Sort best fitness values (highest values first).
        Array.Sort(bestComplexityByRank, meanComplexitySeries, 0, count, Utils.ComparerDesc);
    }

    private double[] GetGenomeFitnessByRank(out int count)
    {
        var genList = _neatPop.GenomeList;
        count = genList.Count;
        double[] genomeFitnessByRank = ArrayPool<double>.Shared.Rent(count);

        for(int i=0; i < count; i++)
        {
            genomeFitnessByRank[i] = genList[i].FitnessInfo.PrimaryFitness;
        }

        // Sort fitness values (highest values first).
        Array.Sort(genomeFitnessByRank, 0, count, Utils.ComparerDesc);
        return genomeFitnessByRank;
    }

    private double[] GetGenomeComplexityByRank(out int count)
    {
        var genList = _neatPop.GenomeList;
        count = genList.Count;
        double[] genomeComplexityByRank = ArrayPool<double>.Shared.Rent(count);

        for(int i=0; i < count; i++)
        {
            genomeComplexityByRank[i] = genList[i].Complexity;
        }

        // Sort fitness values (highest values first).
        Array.Sort(genomeComplexityByRank, 0, count, Utils.ComparerDesc);
        return genomeComplexityByRank;
    }

    #endregion
}
