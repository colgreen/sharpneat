// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Buffers;
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
        _experimentUI = null;

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
        _fitnessTimeSeriesForm?.UpdateData(_eaRunner.EA.Stats, _neatPop.NeatPopulationStats);

        _complexityTimeSeriesForm?.UpdateData(_eaRunner.EA.Stats, _neatPop.NeatPopulationStats);

        _evalsPerSecTimeSeriesForm?.UpdateData(_eaRunner.EA.Stats, _neatPop.NeatPopulationStats);

        // Rankings forms.
        if(_speciesSizeRankForm is not null)
        {
            double[] speciesSizeByRank = GetSpeciesSizeByRank(out int speciesCount);
            try
            {
                _speciesSizeRankForm.UpdateData(
                    speciesSizeByRank.AsSpan(0, speciesCount));
            }
            finally
            {
                ArrayPool<double>.Shared.Return(speciesSizeByRank);
            }
        }

        if(_speciesFitnessRankForm is not null)
        {
            GetSpeciesFitnessByRank(
                out double[] bestFitnessByRank,
                out double[] meanFitnessSeries,
                out int speciesCount);

            try
            {
                _speciesFitnessRankForm.UpdateData(
                    bestFitnessByRank.AsSpan(0, speciesCount),
                    meanFitnessSeries.AsSpan(0, speciesCount));
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
                _speciesComplexityRankForm.UpdateData(
                    bestComplexityByRank.AsSpan(0, speciesCount),
                    meanComplexitySeries.AsSpan(0, speciesCount));
            }
            finally
            {
                ArrayPool<double>.Shared.Return(bestComplexityByRank);
                ArrayPool<double>.Shared.Return(meanComplexitySeries);
            }
        }

        if(_genomeFitnessRankForm is not null)
        {
            // Get a rented array of genome fitness values, and sort in descending order (highest fitness first).
            double[] genomeFitnessByRank = GetGenomeFitness(out int genomeCount);
            Array.Sort(genomeFitnessByRank, 0, genomeCount, Utils.ComparerDesc);

            try
            {
                _genomeFitnessRankForm.UpdateData(
                    genomeFitnessByRank.AsSpan(0, genomeCount));
            }
            finally
            {
                ArrayPool<double>.Shared.Return(genomeFitnessByRank);
            }
        }

        if(_genomeComplexityRankForm is not null)
        {
            // Get a rented array of genome complexities, and sort in descending order.
            double[] genomeComplexity = GetGenomeComplexity(out int genomeCount);
            Array.Sort(genomeComplexity, 0, genomeCount, Utils.ComparerDesc);

            try
            {
                _genomeComplexityRankForm.UpdateData(
                    genomeComplexity.AsSpan(0, genomeCount));
            }
            finally
            {
                ArrayPool<double>.Shared.Return(genomeComplexity);
            }
        }

        // Histogram forms.
        if(_genomeFitnessHistogramForm is not null)
        {
            // Get a rented array of genome fitness values.
            double[] genomeFitness = GetGenomeFitness(out int genomeCount);

            // Calculate histogram data (i.e., the number of histogram bins, and frequency of each bin).
            GetHistogramData(
                genomeFitness.AsSpan(0, genomeCount),
                out Span<double> binX,
                out Span<double> binFrequency,
                out double[] rentedArray);

            try
            {
                _genomeFitnessHistogramForm.UpdateData(
                    binX, binFrequency);
            }
            finally
            {
                ArrayPool<double>.Shared.Return(genomeFitness);
                ArrayPool<double>.Shared.Return(rentedArray);
            }
        }

        if(_genomeComplexityHistogramForm is not null)
        {
            // Get a rented array of genome complexities.
            double[] genomeComplexity = GetGenomeComplexity(out int genomeCount);

            // Calculate histogram data (i.e., the number of histogram bins, and frequency of each bin).
            GetHistogramData(
                genomeComplexity.AsSpan(0, genomeCount),
                out Span<double> binX,
                out Span<double> binFrequency,
                out double[] rentedArray);

            try
            {
                _genomeComplexityHistogramForm.UpdateData(
                    binX, binFrequency);
            }
            finally
            {
                ArrayPool<double>.Shared.Return(genomeComplexity);
                ArrayPool<double>.Shared.Return(rentedArray);
            }
        }

        // Write entry to log.
        __log.Info($"gen={_eaRunner.EA.Stats.Generation:N0} bestFitness={_neatPop.Stats.BestFitness.PrimaryFitness:N6}");

        if(_eaRunner.RunState == RunState.Paused)
            UpdateUIState_EaReadyPaused();
    }

    private double[] GetSpeciesSizeByRank(out int count)
    {
        var speciesArr = _neatPop.SpeciesArray;
        count = speciesArr.Length;

        double[] speciesSizeByRank = ArrayPool<double>.Shared.Rent(count);

        for(int i = 0; i < count; i++)
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

        for(int i = 0; i < count; i++)
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

        for(int i = 0; i < count; i++)
        {
            bestComplexityByRank[i] = speciesArr[i].GenomeList[0].Complexity;
            meanComplexitySeries[i] = speciesArr[i].CalcMeanComplexity();
        }

        // Sort best fitness values (highest values first).
        Array.Sort(bestComplexityByRank, meanComplexitySeries, 0, count, Utils.ComparerDesc);
    }

    private double[] GetGenomeFitness(out int count)
    {
        var genList = _neatPop.GenomeList;
        count = genList.Count;
        double[] genomeFitnessByRank = ArrayPool<double>.Shared.Rent(count);

        for(int i = 0; i < count; i++)
        {
            genomeFitnessByRank[i] = genList[i].FitnessInfo.PrimaryFitness;
        }

        return genomeFitnessByRank;
    }

    private double[] GetGenomeComplexity(out int count)
    {
        var genList = _neatPop.GenomeList;
        count = genList.Count;
        double[] genomeComplexity = ArrayPool<double>.Shared.Rent(count);

        for(int i = 0; i < count; i++)
        {
            genomeComplexity[i] = genList[i].Complexity;
        }

        return genomeComplexity;
    }

    private static void GetHistogramData(
        Span<double> data,
        out Span<double> binX,
        out Span<double> binFrequency,
        out double[] rentedArray)
    {
        // Calc histogram bin count using the Rice rule; see http://en.wikipedia.org/wiki/Histogram
        int binCount = (int)(2.0 * Math.Pow(data.Length, 1.0/3.0));

        HistogramData hd = NumericsUtils.BuildHistogramData(data, binCount);

        rentedArray = ArrayPool<double>.Shared.Rent(binCount * 2);
        binX = rentedArray.AsSpan().Slice(0, binCount);
        binFrequency = rentedArray.AsSpan().Slice(binCount, binCount);

        double incr = hd.Increment;
        double x = hd.Min + (incr / 2.0);

        for(int i = 0; i < hd.FrequencyArray.Length; i++, x += incr)
        {
            binX[i] = x;
            binFrequency[i] = hd.FrequencyArray[i];
        }
    }
}
