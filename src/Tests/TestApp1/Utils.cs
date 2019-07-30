using System.IO;
using SharpNeat.Experiments;
using SharpNeat.Neat.EvolutionAlgorithm;

namespace TestApp1
{
    internal static class Utils
    {
        public static NeatEvolutionAlgorithm<double> CreateNeatEvolutionAlgorithm(
            INeatExperimentFactory<double> experimentFactory,
            string jsonConfigFilename)
        {
            // Read experiment json config from file.
            // Note. We read the entire contents into a string; we don't ever expect to see large json files here, so this fine.
            string jsonStr = File.ReadAllText(jsonConfigFilename);

            // Create an instance of INeatExperiment, configured using the supplied json config.
            INeatExperiment<double> neatExperiment = experimentFactory.CreateExperiment(jsonStr);

            // Create a NeatEvolutionAlgorithm instance ready to run the experiment.
            var ea = NeatExperimentUtils.CreateNeatEvolutionAlgorithm(neatExperiment);
            return ea;
        }
    }
}
