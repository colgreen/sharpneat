using SharpNeat.Experiments;
using SharpNeat.Neat;
using SharpNeat.Neat.EvolutionAlgorithm;

namespace TestApp1;

internal static class Utils
{
    public static NeatEvolutionAlgorithm<double> CreateNeatEvolutionAlgorithm(
        INeatExperimentFactory experimentFactory)
    {
        string jsonConfigFilename = $"experiments-config/{experimentFactory.Id}.config.json";

        // Create an instance of INeatExperiment, configured using the supplied json config.
        INeatExperiment<double> neatExperiment = experimentFactory.CreateExperiment(jsonConfigFilename);

        // Create a NeatEvolutionAlgorithm instance ready to run the experiment.
        var ea = NeatUtils.CreateNeatEvolutionAlgorithm(neatExperiment);
        return ea;
    }
}
