using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using SharpNeat.Experiments;
using SharpNeat.Tasks.BinaryElevenMultiplexer;
using SharpNeat.Tasks.GenerativeFunctionRegression;

namespace EfficacySampler
{
    class Program
    {
        static StreamWriter __streamWriter;

        #region Main Entry Point

        static void Main(string[] args)
        {
            // Intercept termination of the console app, to flush and close the output file stream 
            // (apparently the 'finally' block below is not executed if the app is terminated with Ctrl-C).
            Console.CancelKeyPress += delegate {
                if(null != __streamWriter) {
                    __streamWriter.Close();
                }
            };

            // Read command line arguments.
            StopCondition stopCond = ArgUtils.ReadArgs(args, out string experimentId, out string filename);
            if(null == stopCond) {
                return;
            }

            // Initialise log4net (log to console).
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.properties"));

            // Create and configure a NEAT experiment instance.
            INeatExperiment<double> experiment = InitExperiment(experimentId);
            if(null == experiment) {
                return;
            }

            // Create an evolution algorithm host.
            IEvolutionAlgorithmHost eaHost = CreateEvolutionAlgorithmHost(experiment, stopCond);

            // Open and initialise the output file.
            __streamWriter = InitOutputFile(filename);
            try
            {
                // Run the main eficay sampling loop until the process is terminated.
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

        #endregion

        #region Private Static Methods [Initialisation]

        private static IEvolutionAlgorithmHost CreateEvolutionAlgorithmHost(
            INeatExperiment<double> experiment,
            StopCondition stopCond)
        {
            switch(stopCond.StopConditionType)
            {
                case StopConditionType.ElapsedClockTime:
                    return new EvolutionAlgorithmHostClockTime(experiment, stopCond.Value);

                case StopConditionType.GenerationCount:
                    return new EvolutionAlgorithmHostGenerational(experiment, stopCond.Value);

                default:
                    throw new ArgumentException(nameof(stopCond));
            }
        }

        private static INeatExperiment<double> InitExperiment(string experimentId)
        {
            switch(experimentId)
            {
                case "binary11":
                    return InitExperiment_BinaryElevenMultiplexer();
                case "sinewave":
                    return InitExperiment_Sinewave();
            }

            Console.WriteLine($"Unrecognised experiment [{experimentId}]");
            return null;
        }

        private static INeatExperiment<double> InitExperiment_BinaryElevenMultiplexer()
        {
            // Read experiment json config from file.
            // Note. We read the entire contents into a string; we don't ever expect to see large json files here, so this fine.
            string jsonStr = File.ReadAllText("config/binary-eleven-multiplexer.config.json");

            // Create an instance of INeatExperiment for the binary 11-multiplexer task, configured using the supplied json config.
            var experimentFactory = new BinaryElevenMultiplexerExperimentFactory();
            INeatExperiment<double> neatExperiment = experimentFactory.CreateExperiment(jsonStr);
            return neatExperiment;
        }

        private static INeatExperiment<double> InitExperiment_Sinewave()
        {
            // Read experiment json config from file.
            // Note. We read the entire contents into a string; we don't ever expect to see large json files here, so this fine.
            string jsonStr = File.ReadAllText("config/generative-sinewave.config.json");

            // Create an instance of INeatExperiment for the generative sinewave task, configured using the supplied json config.
            var experimentFactory = new GenerativeFnRegressionExperimentFactory();
            INeatExperiment<double> neatExperiment = experimentFactory.CreateExperiment(jsonStr);
            return neatExperiment;
        }

        #endregion

        #region Private Static Methods

        private static StreamWriter InitOutputFile(string filename)
        {
            FileInfo fileInfo = new FileInfo(filename);
            if(fileInfo.Exists)
            {
                // Append to existing file.
                return new StreamWriter(filename, true);
            }
            
            // Create new file and write a CSV header row.
            StreamWriter sw = new StreamWriter(filename);
            sw.WriteLine("secs,gens,bestfitness,meanfitness,maxcomplexity,meancomplexity,evalcount");
            return sw;
        }

        #endregion
    }
}
