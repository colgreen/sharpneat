using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using SharpNeat.Core;
using SharpNeat.Domains;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;

namespace EfficacySampler
{
    public class EvolutionAlgorithmHost
    {
        IGuiNeatExperiment _experiment;
        StopCondition _stopCond;
        NeatEvolutionAlgorithm<NeatGenome> _ea;
        Stopwatch _stopwatch;

        #region Constructor

        public EvolutionAlgorithmHost(IGuiNeatExperiment experiment, StopCondition stopCond)
        {
            _experiment = experiment;
            _stopCond = stopCond;
            _stopwatch = new Stopwatch();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialise and run the evolutionary algorithm until the stop condition occurs (either elapsed clock time, or some number of generations).
        /// Once the stop conditon is reached this method returns with the current best fitness in the population.
        /// </summary>
        /// <returns></returns>
        public double Sample(out double secs, out int gens)
        {
            _ea = CreateEvolutionAlgorithm();
            _stopwatch.Restart();
            _ea.StartContinue();

            // Block the current thread until the EA stop condition occurs.
            Block(_stopCond);

            // Stop any threads and record best fitness.
            _ea.RequestTerminateAndWait();
            _stopwatch.Stop();

            double fitness = _ea.Statistics._maxFitness;
            secs = _stopwatch.ElapsedMilliseconds * 0.001;
            gens = (int)_ea.CurrentGeneration;

            // Make some attempts at forcing release of resources (especially RAM) before we hand control back.
            _ea = null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);

            return fitness;
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
            if(stopCond.StopConditionType == StopConditionType.ElapsedClockTime) {
                Block(TimeSpan.FromSeconds(stopCond.Value));
            }
            else {
                Block(stopCond.Value);
            }
        }

        private void Block(TimeSpan timespan)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for(;;)
            {
                if((sw.Elapsed >= timespan) || (RunState.Running != _ea.RunState)) {
                    sw.Stop();
                    return;
                }
                Thread.Sleep(1000);
            }
        }

        private void Block(int generation)
        {
            for(;;)
            {
                if((_ea.CurrentGeneration >= generation) || (RunState.Running != _ea.RunState)) {
                    return;
                }
                Thread.Sleep(1000);
            }
        }

        #endregion
    }
}
