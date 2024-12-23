// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Buffers;
using Redzen.Numerics;
using SharpNeat.EvolutionAlgorithm.Runner;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Windows.App.Forms;

#pragma warning disable IDE1006 // Naming Styles. Allow naming convention for Windows.Forms event handlers.

namespace SharpNeat.Windows.App;

partial class MainForm
{
    private void cmbExperiments_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Clear any existing references, as these are specific to each experiment.
        _neatExperiment = null;
        _experimentUi = null;

        // Close the genome form if it is open, as the content of this form is specific to each experiment.
        GenomeForm bestGenomeForm = _bestGenomeForm;

        // Note. This will trigger the FormClosed event which will do further clean-up; Close() will also Dispose() the form.
        bestGenomeForm?.Close();
    }

    private void _eaRunner_UpdateEvent(object sender, EventArgs e)
    {
        if(_eaRunner == null || _eaRunner.RunState == RunState.Terminated)
            return;

        // Switch to the UI thread, if not already on that thread.
        if(InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate ()
            {
                _eaRunner_UpdateEvent(sender, e);
            }));
            return;
        }

        // Update stats fields.
        UpdateUiState_EaStats();

        // Update child forms (those that are open).
        var bestGenome = ((NeatEvolutionAlgorithm<double>)_eaRunner.EA).Population.BestGenome;

        if(_bestGenomeForm is not null)
            _bestGenomeForm.Genome = bestGenome;

        if(_taskForm is not null)
            _taskForm.Genome = bestGenome;

        // Time series forms.
        _fitnessTimeSeriesForm?.UpdateData(
            _eaRunner.EA.Stats,
            _neatPop.NeatPopulationStats);

        _complexityTimeSeriesForm?.UpdateData(
            _eaRunner.EA.Stats,
            _neatPop.NeatPopulationStats);

        _evalsPerSecTimeSeriesForm?.UpdateData(
            _eaRunner.EA.Stats,
            _neatPop.NeatPopulationStats);

        // Histogram forms.
        if(_speciesSizeHistogramForm is not null)
        {
            // Get rented array of species sizes.
            using RentedArray<double> speciesSizes = GetSpeciesSizes();

            // Calculate histogram data (i.e., the number of histogram bins, and frequency of each bin).
            HistogramData hd = HistogramData.BuildHistogramData(
                speciesSizes.AsSpan());

            _speciesSizeHistogramForm.UpdateData(hd);
        }

        if(_speciesMeanFitnessHistogramForm is not null)
        {
            // Get rented array of species mean fitnesses.
            using RentedArray<double> speciesMeanFitnesses = GetSpeciesMeanFitnesses();

            // Calculate histogram data (i.e., the number of histogram bins, and frequency of each bin).
            HistogramData hd = HistogramData.BuildHistogramData(
                speciesMeanFitnesses.AsSpan());

            _speciesMeanFitnessHistogramForm.UpdateData(hd);
        }

        if(_speciesMeanComplexityHistogramForm is not null)
        {
            // Get rented array of species mean complexities.
            using RentedArray<double> speciesMeanComplexities = GetSpeciesMeanComplexities();

            // Calculate histogram data (i.e., the number of histogram bins, and frequency of each bin).
            HistogramData hd = HistogramData.BuildHistogramData(
                speciesMeanComplexities.AsSpan());

            _speciesMeanComplexityHistogramForm.UpdateData(hd);
        }

        if(_genomeFitnessHistogramForm is not null)
        {
            // Get a rented array of genome fitness values.
            using RentedArray<double> genomeFitness = GetGenomeFitnesses();

            // Calculate histogram data (i.e., the number of histogram bins, and frequency of each bin).
            HistogramData hd = HistogramData.BuildHistogramData(
                genomeFitness.AsSpan());

            _genomeFitnessHistogramForm.UpdateData(hd);
        }

        if(_genomeComplexityHistogramForm is not null)
        {
            // Get a rented array of genome complexities.
            using RentedArray<double> genomeComplexity = GetGenomeComplexities();

            // Calculate histogram data (i.e., the number of histogram bins, and frequency of each bin).
            HistogramData hd = HistogramData.BuildHistogramData(
                genomeComplexity.AsSpan());

            _genomeComplexityHistogramForm.UpdateData(hd);
        }

        // Rankings forms.
        if(_speciesSizeRankForm is not null)
        {
            using RentedArray<double> speciesSizeByRank = GetSpeciesSizeByRank();
            _speciesSizeRankForm.UpdateData(
                speciesSizeByRank.AsSpan());
        }

        if(_speciesFitnessRankForm is not null)
        {
            GetSpeciesFitnessByRank(
                out RentedArray<double> bestFitnessByRank,
                out RentedArray<double> meanFitnessSeries);

            using (bestFitnessByRank)
            using (meanFitnessSeries)
            {
                _speciesFitnessRankForm.UpdateData(
                    bestFitnessByRank.AsSpan(),
                    meanFitnessSeries.AsSpan());
            }
        }

        if(_speciesComplexityRankForm is not null)
        {
            GetSpeciesComplexityByRank(
                out RentedArray<double> bestComplexityByRank,
                out RentedArray<double> meanComplexitySeries);

            using(bestComplexityByRank)
            using(meanComplexitySeries)
            {
                _speciesComplexityRankForm.UpdateData(
                    bestComplexityByRank.AsSpan(),
                    meanComplexitySeries.AsSpan());

            }
        }

        if(_genomeFitnessRankForm is not null)
        {            
            using RentedArray<double> genomeFitnessByRank = GetGenomeFitnesses();
            Span<double> span = genomeFitnessByRank.AsSpan();
            MemoryExtensions.Sort(span, Utils.ComparerDesc);

            _genomeFitnessRankForm.UpdateData(span);            
        }

        if(_genomeComplexityRankForm is not null)
        {
            using RentedArray<double> genomeComplexity = GetGenomeComplexities();
            Span<double> span = genomeComplexity.AsSpan();
            MemoryExtensions.Sort(span, Utils.ComparerDesc);

            _genomeComplexityRankForm.UpdateData(span);            
        }

        // Write entry to log.
        __log.Information($"gen={_eaRunner.EA.Stats.Generation:N0} bestFitness={_neatPop.Stats.BestFitness.PrimaryFitness:N6}");

        if(_eaRunner.RunState == RunState.Paused)
            UpdateUiState_EaReadyPaused();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if(_eaRunner?.RunState == RunState.Running)
        {
            MessageBox.Show("Evolution Algorithm is still running. Please click 'Stop' before closing the app.");
            e.Cancel = true;
        }
    }

    private RentedArray<double> GetGenomeFitnesses()
    {
        var genList = _neatPop.GenomeList;
        int count = genList.Count;

        RentedArray<double> genomeFitnessByRank = new(count);
        Span<double> span = genomeFitnessByRank.AsSpan();

        for(int i = 0; i < count; i++)
            span[i] = genList[i].FitnessInfo.PrimaryFitness;

        return genomeFitnessByRank;
    }

    private RentedArray<double> GetGenomeComplexities()
    {
        var genList = _neatPop.GenomeList;
        int count = genList.Count;
        RentedArray<double> genomeComplexity = new(count);
        Span<double> span = genomeComplexity.AsSpan();

        for(int i = 0; i < count; i++)
            span[i] = genList[i].Complexity;

        return genomeComplexity;
    }

    private RentedArray<double> GetSpeciesSizes()
    {
        var speciesArr = _neatPop.SpeciesArray;
        int count = speciesArr.Length;

        RentedArray<double> speciesSizes = new(count);
        Span<double> span = speciesSizes.AsSpan();

        for(int i = 0; i < count; i++)
            span[i] = speciesArr[i].GenomeList.Count;

        return speciesSizes;
    }

    private RentedArray<double> GetSpeciesMeanFitnesses()
    {
        var speciesArr = _neatPop.SpeciesArray;
        int count = speciesArr.Length;

        RentedArray<double> speciesMeanFitnesses = new(count);
        Span<double> span = speciesMeanFitnesses.AsSpan();

        for(int i = 0; i < count; i++)
            span[i] = speciesArr[i].Stats.MeanFitness;

        return speciesMeanFitnesses;
    }

    private RentedArray<double> GetSpeciesMeanComplexities()
    {
        var speciesArr = _neatPop.SpeciesArray;
        int count = speciesArr.Length;

        RentedArray<double> speciesMeanComplexities = new(count);
        Span<double> span = speciesMeanComplexities.AsSpan();

        for(int i = 0; i < count; i++)
            span[i] = speciesArr[i].CalcMeanComplexity();

        return speciesMeanComplexities;
    }

    private RentedArray<double> GetSpeciesSizeByRank()
    {
        var speciesArr = _neatPop.SpeciesArray;
        int count = speciesArr.Length;

        RentedArray<double> speciesSizeByRank = new(count);
        Span<double> span = speciesSizeByRank.AsSpan();

        for(int i = 0; i < count; i++)
            span[i] = speciesArr[i].GenomeList.Count;

        // Sort size values (highest values first).
        MemoryExtensions.Sort(span, Utils.ComparerDesc);

        return speciesSizeByRank;
    }

    private void GetSpeciesFitnessByRank(
        out RentedArray<double> bestFitnessByRank,
        out RentedArray<double> meanFitnessSeries)
    {
        var speciesArr = _neatPop.SpeciesArray;
        int count = speciesArr.Length;

        bestFitnessByRank = new(count);
        meanFitnessSeries = new(count);

        var bestFitnessByRankSpan = bestFitnessByRank.AsSpan();
        var meanFitnessSeriesSpan = meanFitnessSeries.AsSpan();

        for(int i = 0; i < count; i++)
        {
            bestFitnessByRankSpan[i] = speciesArr[i].GenomeList[0].FitnessInfo.PrimaryFitness;
            meanFitnessSeriesSpan[i] = speciesArr[i].Stats.MeanFitness;
        }

        // Sort best fitness values (highest values first).
        MemoryExtensions.Sort(
            bestFitnessByRankSpan,
            meanFitnessSeriesSpan,
            Utils.ComparerDesc);
    }

    private void GetSpeciesComplexityByRank(
        out RentedArray<double> bestComplexityByRank,
        out RentedArray<double> meanComplexitySeries)
    {
        var speciesArr = _neatPop.SpeciesArray;
        int count = speciesArr.Length;

        bestComplexityByRank = new(count);
        meanComplexitySeries = new(count);

        var bestComplexityByRankSpan = bestComplexityByRank.AsSpan();
        var meanComplexitySeriesSpan = meanComplexitySeries.AsSpan();

        for(int i = 0; i < count; i++)
        {
            bestComplexityByRankSpan[i] = speciesArr[i].GenomeList[0].Complexity;
            meanComplexitySeriesSpan[i] = speciesArr[i].CalcMeanComplexity();
        }

        // Sort best fitness values (highest values first).
        MemoryExtensions.Sort(
            bestComplexityByRankSpan, meanComplexitySeriesSpan, Utils.ComparerDesc);
    }
}
