﻿using System.Diagnostics;
using System.Numerics;
using SharpNeat.Experiments;
using SharpNeat.Neat.EvolutionAlgorithm;

namespace EfficacySampler;

/// <summary>
/// An <see cref="IEvolutionAlgorithmHost"/> that is based on a generation count stop condition, e.g. run for 100 generations.
/// </summary>
public sealed class EvolutionAlgorithmHostGenerational<TScalar> : IEvolutionAlgorithmHost
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    readonly INeatExperiment<TScalar> _experiment;
    readonly int _stopGenerationCount;
    readonly Stopwatch _stopwatch;

    #region Constructor

    public EvolutionAlgorithmHostGenerational(
        INeatExperiment<TScalar> experiment,
        int stopGenerationCount)
    {
        _experiment = experiment;
        _stopGenerationCount = stopGenerationCount;
        _stopwatch = new Stopwatch();
    }

    #endregion

    #region Public Methods

    /// <inheritdoc/>
    public Sample Sample()
    {
        // Create a new instance of an evolution algorithm.
        NeatEvolutionAlgorithm<TScalar> ea = NeatEvolutionAlgorithmFactory.CreateEvolutionAlgorithm(_experiment);

        // Start the stopwatch.
        _stopwatch.Restart();

        // We include clock time spent doing initialisation in the recorded stats for each sample;
        // this is the scientifically robust approach as initialisation might perform a lot of work.
        ea.Initialise();

        // Run the main EA loop for the required number of generations.
        for(int i=0; i < _stopGenerationCount; i++)
        {
            ea.PerformOneGeneration();
        }

        // Stop the stopwatch.
        _stopwatch.Stop();

        // Copy the required stats into a new Sample instance.
        Sample sample = new()
        {
            ElapsedTimeSecs = _stopwatch.ElapsedMilliseconds * 0.001,
            GenerationCount = ea.Stats.Generation
        };

        var pop = ea.Population;
        sample.BestFitness = pop.Stats.BestFitness.PrimaryFitness;
        sample.MeanFitness = pop.Stats.MeanFitness;

        // TODO: Store max complexity? Or should we use BestComplexity? Or both?
        //sample.MaxComplexity = ;
        sample.MeanComplexity = pop.Stats.MeanComplexity;
        sample.EvaluationCount = ea.Stats.TotalEvaluationCount;

        // Make some attempts at forcing release of resources (especially RAM) before we hand control back.
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);

        // Return the sample.
        return sample;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    #endregion
}
