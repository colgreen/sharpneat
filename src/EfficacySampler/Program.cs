using System;
using System.IO;
using System.Xml;
using log4net.Config;
using SharpNeat.Domains;
using SharpNeat.Domains.BinaryElevenMultiplexer;
using SharpNeat.Domains.GenerativeFunctionRegression;

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
                    Sample s = eaHost.Sample();
                    __streamWriter.WriteLine($"{s.ElapsedTimeSecs},{s.GenerationCount},{s.BestFitness:0.#####},{s.MeanFitness:0.#####},{s.MaxComplexity:0.#####},{s.MeanComplexity:0.#####},{s.EvaluationCount}");
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
            sw.WriteLine("secs,gens,bestfitness,meanfitness,maxcomplexity,meancomplexity,evalcount");
            return sw;
        }

        private static IGuiNeatExperiment InitExperiment(string experimentId)
        {
            switch(experimentId)
            {
                case "binary11":
                    return InitExperiment_BinaryElevenMultiplexer();
                case "sinewave":
                    return InitExperiment_Sinwave();
            }

            Console.WriteLine($"Unrecognised experiment [{experimentId}]");
            return null;
        }

        private static IGuiNeatExperiment InitExperiment_BinaryElevenMultiplexer()
        {
            // Experiment classes encapsulate much of the nuts and bolts of setting up a NEAT search.
            var experiment = new BinaryElevenMultiplexerExperiment();

            // Load config XML.
            XmlDocument xmlConfig = new XmlDocument();
            xmlConfig.Load("config/binary-eleven-multiplexer.config.xml");
            experiment.Initialize(experiment.Name, xmlConfig.DocumentElement);
            return experiment;
        }

        private static IGuiNeatExperiment InitExperiment_Sinwave()
        {
            // Experiment classes encapsulate much of the nuts and bolts of setting up a NEAT search.
            var experiment = new GenerativeFnRegressionExperiment();

            // Load config XML.
            XmlDocument xmlConfig = new XmlDocument();
            xmlConfig.Load("config/generative-sinewave.config.xml");
            experiment.Initialize(experiment.Name, xmlConfig.DocumentElement);
            return experiment;
        }

        #endregion
    }
}
