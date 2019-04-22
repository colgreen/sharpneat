using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using SharpNeat.Core;
using SharpNeat.Domains;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;

namespace EfficacySampler
{
    public class EvolutionAlgorithmHost
    {
        readonly INeatExperiment _experiment;
        readonly StopCondition _stopCond;
        readonly Stopwatch _stopwatch;
        NeatEvolutionAlgorithm<NeatGenome> _ea;

        #region Constructor

        public EvolutionAlgorithmHost(INeatExperiment experiment, StopCondition stopCond)
        {
            _experiment = experiment;
            _stopCond = stopCond;
            _stopwatch = new Stopwatch();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialise and run the evolutionary algorithm until the stop condition occurs (either elapsed clock time, or some number of generations).
        /// Once the stop condition is reached this method returns with the current best fitness in the population.
        /// </summary>
        /// <returns></returns>
        public Sample Sample()
        {
            _ea = CreateEvolutionAlgorithm();
            _stopwatch.Restart();
            _ea.StartContinue();

            // Block the current thread until the EA stop condition occurs.
            Block(_stopCond);

            // Record stats as soon as we can (the EA is still running).
            // Strictly speaking we should sync access to these states because the EA is still running; and we prefer 
            // not to stop it first because that can take a long time (because it waits for the current generation to complete).
            Sample sample = new Sample();

            lock(_ea.Statistics)
            {
                // Call Stop() here because we may have waited to obtain the lock, therefore this gives a more accurate
                // measure of how much time had actually elapsed when the EA statistics were read.
                _stopwatch.Stop();

                sample.ElapsedTimeSecs = _stopwatch.ElapsedMilliseconds * 0.001;
                sample.GenerationCount =  (int)_ea.CurrentGeneration;
                sample.BestFitness = _ea.Statistics._maxFitness;
                sample.MeanFitness = _ea.Statistics._meanFitness;
                sample.MaxComplexity = _ea.Statistics._maxComplexity;
                sample.MeanComplexity = _ea.Statistics._meanComplexity;
                sample.EvaluationCount = _ea.Statistics._totalEvaluationCount;
            }

            // Request the EA to stop, and wait for it. 
            // NB. This call could block for a long time, i.e. if the EA is within a long running generation then 
            // it will not return until the generation has completed. Hence we prefer to read statistics above, and then
            // call this method to actually stop the EA. 
            _ea.RequestTerminateAndWait();

            // Make some attempts at forcing release of resources (especially RAM) before we hand control back.
            _ea = null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);

            return sample;
        }

        #endregion

        #region Private Methods

        private NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm()
        {
            int popSize = _experiment.DefaultPopulationSize;
            IGenomeFactory<NeatGenome> genomeFactory = _experiment.CreateGenomeFactory();
            List<NeatGenome> genomeList = genomeFactory.CreateGenomeList(popSize, 0);
            NeatEvolutionAlgorithm<NeatGenome> ea = _experiment.CreateEvolutionAlgorithm(genomeFactory, genomeList);
            return ea;
        }

        private void Block(StopCondition stopCond)
        {
            // Enter monitor loop.
            switch(stopCond.StopConditionType)
            {
                case StopConditionType.ElapsedClockTime:
                    Block(TimeSpan.FromSeconds(stopCond.Value));
                    break;
                case StopConditionType.GenerationCount:
                    BlockUntilGeneration(stopCond.Value);
                    break;
                default:
                    throw new ArgumentException(nameof(stopCond));
            }
        }

        private void Block(TimeSpan timespan)
        {
            const double thresholdMs = 10.0;

            for(;;)
            {
                double remainingMs = (timespan - _stopwatch.Elapsed).TotalMilliseconds;
                // No point calling Thread.Sleep for very short durations.
                if(remainingMs <= thresholdMs || (RunState.Running != _ea.RunState)) {
                    return;
                }

                // Wait for slightly less time than is remaining, to increase the chances of waiting the correct amount of time 
                // (at time of writing Thread.Sleep does not appear to have especially accurate timing).
                remainingMs = Math.Max(0, remainingMs - thresholdMs);
                Thread.Sleep((int)remainingMs);
            }
        }

        private void BlockUntilGeneration(int generation)
        {
            for(;;)
            {
                if((_ea.CurrentGeneration >= generation) || (RunState.Running != _ea.RunState)) {
                    return;
                }
                Thread.Sleep(200);
            }
        }

        #endregion
    }
}
