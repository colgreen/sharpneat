using System.Diagnostics;
using SharpNeat.Experiments;
using SharpNeat.Neat;
using SharpNeat.Neat.EvolutionAlgorithm;

namespace EfficacySampler;

/// <summary>
/// An <see cref="IEvolutionAlgorithmHost"/> that is based on a clock time stop condition, e.g. run for 60 seconds.
/// </summary>
public sealed class EvolutionAlgorithmHostClockTime : IEvolutionAlgorithmHost
{
    readonly INeatExperiment<double> _experiment;
    readonly TimeSpan _stopTimeSpan;
    readonly Stopwatch _stopwatch;

    readonly Thread _eaThread;
    readonly AutoResetEvent _awaitStartEvent = new(false);
    readonly AutoResetEvent _awaitStopEvent = new(false);
    volatile bool _stopFlag;

    NeatEvolutionAlgorithm<double>? _ea;

    #region Constructor

    public EvolutionAlgorithmHostClockTime(
        INeatExperiment<double> experiment,
        int stopSeconds)
    {
        _experiment = experiment;
        _stopTimeSpan = TimeSpan.FromSeconds(stopSeconds);
        _stopwatch = new Stopwatch();

        _eaThread = new Thread(EAThreadMethod)
        {
            // A background thread will be stopped when the process terminates; otherwise it will keep running and prevent
            // the process from terminating.
            IsBackground = true,

            // Set a priority lower than the current/main thread. The main thread will wake-up and signal the EA thread to stop,
            // and we don't want it to be delayed in doing that by the EA thread running at 100% utilisation on all CPU cores.
            Priority = ThreadPriority.BelowNormal
        };

        // Start the background EA thread; it will immediately block on _awaitStartEvent, and wait to be signalled to start proper.
        _eaThread.Start();
    }

    #endregion

    #region Public Methods

    /// <inheritdoc/>
    public Sample Sample()
    {
        // Create a new instance of an evolution algorithm.
        _ea = NeatUtils.CreateNeatEvolutionAlgorithm(_experiment);

        // Start the stopwatch.
        _stopwatch.Restart();

        // Signal the EA thread to start. Ensure the stop flag is reset before we do so.
        _stopFlag = false;
        Thread.MemoryBarrier();
        _awaitStartEvent.Set();

        // Block this thread (the main thread) for the required amount of clock time.
        Block(_stopTimeSpan);

        // Signal the EA thread to stop.
        _stopFlag = true;
        Thread.MemoryBarrier();

        // Record the sample without waiting for the EA to stop.
        // The EA may be within a PerformOneGeneration() call for some time, so we prefer to record the sample
        // at the exact required point in time (or very close to it) rather than some arbitrary point in time
        // after the last generation has completed.
        Sample sample = RecordSample();

        // Now wait for the EA to signal that it has stopped.
        _awaitStopEvent.WaitOne();

        // Make some attempts at forcing release of resources (especially RAM) before we hand control back.
        _ea = null;
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);

        // Return the sample.
        return sample;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _awaitStartEvent?.Dispose();
        _awaitStopEvent?.Dispose();
    }

    #endregion

    #region Private Methods [Background Evolution Algorithm Thread]

    private void EAThreadMethod()
    {
        // Keep entering the EA loop forever (until the process is terminated with ctrl-C!).
        for(;;)
        {
            // Wait until signalled to start the EA loop.
            _awaitStartEvent.WaitOne();

            // We include clock time spent doing initialisation in the recorded stats for each sample;
            // this is the scientifically robust approach as initialisation might perform a lot of work.
            _ea!.Initialise();

            // Run the main EA loop until we are signalled to stop.
            while(!_stopFlag)
            {
                _ea.PerformOneGeneration();
            }

            // Signal the waiting thread that we have stopped.
            _awaitStopEvent.Set();
        }
    }

    #endregion

    #region Private Methods

    private Sample RecordSample()
    {
        // Copy the required stats into a new Sample instance.
        Sample sample = new()
        {
            ElapsedTimeSecs = _stopwatch.ElapsedMilliseconds * 0.001,
            GenerationCount = _ea!.Stats.Generation
        };

        var pop = _ea.Population;
        sample.BestFitness = pop.Stats.BestFitness.PrimaryFitness;
        sample.MeanFitness = pop.Stats.MeanFitness;

        // TODO: Store max complexity? Or should we use BestComplexity? Or both?
        //sample.MaxComplexity = ;
        sample.MeanComplexity = pop.Stats.MeanComplexity;
        sample.EvaluationCount = _ea.Stats.TotalEvaluationCount;

        // Return the sample.
        return sample;
    }

    private void Block(TimeSpan timespan)
    {
        const double thresholdMs = 10.0;

        for(;;)
        {
            double remainingMs = (timespan - _stopwatch.Elapsed).TotalMilliseconds;

            // There's no point in calling Thread.Sleep() for very short durations, so just return
            // if the remaining time is almost zero.
            if(remainingMs <= thresholdMs)
                return;

            // Wait for slightly less time than is remaining, to increase the chances of waiting the correct amount of time
            // (at time of writing Thread.Sleep does not appear to have especially accurate timing, due to how kernel task
            // scheduling works).
            remainingMs = Math.Max(0, remainingMs - thresholdMs);
            Thread.Sleep((int)remainingMs);
        }
    }

    #endregion
}
