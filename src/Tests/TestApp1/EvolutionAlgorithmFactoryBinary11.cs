using System.IO;
using SharpNeat.Experiments;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Tasks.BinaryElevenMultiplexer;

namespace TestApp1
{
    public class EvolutionAlgorithmFactoryBinary11
    {
        public NeatEvolutionAlgorithm<double> CreateNeatEvolutionAlgorithm()
        {
            // Read experiment json config from file.
            // Note. We read the entire contents into a string; we don't ever expect to see large json files here, so this fine.
            string jsonStr = File.ReadAllText("config/binary-eleven-multiplexer.config.json");

            // Create an instance of INeatExperiment for the binary 11-multiplexer task, configured using the supplied json config.
            var experimentFactory = new BinaryElevenMultiplexerExperimentFactory();
            INeatExperiment<double> neatExperiment = experimentFactory.CreateExperiment(jsonStr);

            // Create a NeatEvolutionAlgorithm instance ready to run the experiment.
            var ea = NeatExperimentUtils.CreateNeatEvolutionAlgorithm(neatExperiment);
            return ea;
        }
    }
}
