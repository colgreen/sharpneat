using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using SharpNeat.Core;
using SharpNeat.Domains.BinaryElevenMultiplexer;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;

namespace EfficacySampler
{
    public class EvolutionAlgorithmHost
    {
        StopCondition _stopCond;
        NeatEvolutionAlgorithm<NeatGenome> _ea;

        #region Constructor

        public EvolutionAlgorithmHost(StopCondition stopCond)
        {
            _stopCond = stopCond;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialise and run the evolutionary algorithm until the stop condition occurs (either elapsed clock time, or some number of generations).
        /// Once the stop conditon is reached this method returns with the current best fitness in the population.
        /// </summary>
        /// <returns></returns>
        public double Sample()
        {
            _ea = InitEA(1000);
            _ea.StartContinue();

            // Block the current thread until the EA stop condition occurs.
            Block(_stopCond);
            
            // Stop any threads and record best fitness.
            _ea.RequestTerminateAndWait();
            double fitness = _ea.Statistics._maxFitness;

            // Make some attempts at forcing release of resources (especially RAM) before we hand control back.
            _ea = null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);

            return fitness;
        }

        #endregion

        #region Private Methods

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

        #region Private Static Methods

        private static NeatEvolutionAlgorithm<NeatGenome> InitEA(int populationSize)
        {
            // Experiment classes encapsulate much of the nuts and bolts of setting up a NEAT search.
            BinaryElevenMultiplexerExperiment experiment = new BinaryElevenMultiplexerExperiment();

            // Load config XML.
            XmlDocument xmlConfig = new XmlDocument();
            xmlConfig.Load("binaryElevenMultiplexer.config.xml");
            experiment.Initialize(experiment.Name, xmlConfig.DocumentElement);

            IGenomeFactory<NeatGenome> genomeFactory = experiment.CreateGenomeFactory();
            List<NeatGenome> genomeList = genomeFactory.CreateGenomeList(populationSize, 0);
            NeatEvolutionAlgorithm<NeatGenome> ea = experiment.CreateEvolutionAlgorithm(genomeFactory, genomeList);
            return ea;
        }

        #endregion
    }
}
