using System;
using System.IO;
using System.Xml;
using log4net.Config;
using SharpNeat.Domains;
using SharpNeat.Domains.BinaryElevenMultiplexer;
using SharpNeat.DomainsExtra.InvertedDoublePendulum;

namespace EfficacySampler
{
    class Program
    {
        static void Main(string[] args)
        {
            string experimentId;
            StopCondition stopCond = ArgUtils.ReadArgs(args, out experimentId);
            if(null == stopCond) {
                return;
            }

            // Initialise NEAT experiment.
            IGuiNeatExperiment experiment = InitExperiment(experimentId);
            if(null == experiment) {
                return;
            }

            // Initialise log4net (log to console).
            XmlConfigurator.Configure(new FileInfo("log4net.properties"));

            // Initialise evolution algorithm host.
            EvolutionAlgorithmHost eaHost = new EvolutionAlgorithmHost(experiment, stopCond);

            for(;;)
            {
                double fitness = eaHost.Sample();
                Console.WriteLine(fitness);
            }  
        }

        #region Private Static Methods

        private static IGuiNeatExperiment InitExperiment(string experimentId)
        {
            switch(experimentId)
            {
                case "binaryeleven":
                    return InitExperiment_BinaryElevenMultiplexer();
                case "inverted":
                    return InitExperiment_InvertedDoublePendulum();
            }

            Console.WriteLine($"Unrecognised experiment [{experimentId}]");
            return null;
        }

        private static IGuiNeatExperiment InitExperiment_BinaryElevenMultiplexer()
        {
            // Experiment classes encapsulate much of the nuts and bolts of setting up a NEAT search.
            BinaryElevenMultiplexerExperiment experiment = new BinaryElevenMultiplexerExperiment();

            // Load config XML.
            XmlDocument xmlConfig = new XmlDocument();
            xmlConfig.Load("config/binaryElevenMultiplexer.config.xml");
            experiment.Initialize(experiment.Name, xmlConfig.DocumentElement);
            return experiment;
        }

        private static IGuiNeatExperiment InitExperiment_InvertedDoublePendulum()
        {
            // Experiment classes encapsulate much of the nuts and bolts of setting up a NEAT search.
            InvertedDoublePendulumExperiment experiment = new InvertedDoublePendulumExperiment();

            // Load config XML.
            XmlDocument xmlConfig = new XmlDocument();
            xmlConfig.Load("config/invertedDoublePendulum.config.xml");
            experiment.Initialize(experiment.Name, xmlConfig.DocumentElement);
            return experiment;
        }

        #endregion
    }
}
