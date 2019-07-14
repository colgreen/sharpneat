using System.IO;
using SharpNeat.Experiments;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Tasks.GenerativeFunctionRegression;

namespace TestApp1
{
    public class EvolutionAlgorithmFactorySinewave
    {
        public NeatEvolutionAlgorithm<double> CreateNeatEvolutionAlgorithm()
        {
            // Read experiment json config from file.
            // Note. We read the entire contents into a string; we don't ever expect to see large json files here, so this fine.
            string jsonStr = File.ReadAllText("config/generative-sinewave.config.json");

            // Create an instance of INeatExperiment for the generative sinewave task, configured using the supplied json config.
            var experimentFactory = new GenerativeFnRegressionExperimentFactory();
            INeatExperiment<double> neatExperiment = experimentFactory.CreateExperiment(jsonStr);

            // Create a NeatEvolutionAlgorithm instance ready to run the experiment.
            var ea = NeatExperimentUtils.CreateNeatEvolutionAlgorithm(neatExperiment);
            return ea;
        }
    }
}
