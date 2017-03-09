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
        static StreamWriter __streamWriter;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += delegate {
                if(null != __streamWriter) {
                    __streamWriter.Close();
                }
            };

            string experimentId;
            string filename;
            StopCondition stopCond = ArgUtils.ReadArgs(args, out experimentId, out filename);
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

            __streamWriter = InitOutputFile(filename);
            try
            {
                for(;;)
                {
                    double secs;
                    int gens;
                    double fitness = eaHost.Sample(out secs, out gens);
                    __streamWriter.WriteLine($"{secs},{gens},{fitness}");
                    __streamWriter.Flush();
                }
            }
            finally
            {
                if(null != __streamWriter) {
                    __streamWriter.Close();
                }
            }
        }

        #region Private Static Methods

        private static StreamWriter InitOutputFile(string filename)
        {
            FileInfo fileInfo = new FileInfo(filename);
            if(fileInfo.Exists)
            {
                // Append to existing file.
                return new StreamWriter(filename, true);
            }
            
            // Create new file and write a header row.
            StreamWriter sw = new StreamWriter(filename);
            sw.WriteLine("secs,gens,bestfitness");
            return sw;
        }

        private static IGuiNeatExperiment InitExperiment(string experimentId)
        {
            switch(experimentId)
            {
                case "binary11":
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
