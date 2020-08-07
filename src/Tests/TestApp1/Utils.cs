using System.IO;
using System.Text.Json;
using SharpNeat.Experiments;
using SharpNeat.IO;
using SharpNeat.Neat.EvolutionAlgorithm;

namespace TestApp1
{
    internal static class Utils
    {
        public static NeatEvolutionAlgorithm<double> CreateNeatEvolutionAlgorithm(
            INeatExperimentFactory<double> experimentFactory)
        {
            string jsonConfigFilename = $"config/{experimentFactory.Id}.config.json";

            // Load experiment json config from file.
            JsonDocument configDoc = JsonUtils.LoadUtf8(jsonConfigFilename);

            // Create an instance of INeatExperiment, configured using the supplied json config.
            INeatExperiment<double> neatExperiment = experimentFactory.CreateExperiment(configDoc.RootElement);

            // Create a NeatEvolutionAlgorithm instance ready to run the experiment.
            var ea = NeatExperimentUtils.CreateNeatEvolutionAlgorithm(neatExperiment);
            return ea;
        }        
    }
}
