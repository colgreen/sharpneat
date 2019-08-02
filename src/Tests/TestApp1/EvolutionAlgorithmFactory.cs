using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Tasks.BinaryElevenMultiplexer;
using SharpNeat.Tasks.BinarySixMultiplexer;
using SharpNeat.Tasks.GenerativeFunctionRegression;
using SharpNeat.Tasks.SinglePoleBalancing;
using SharpNeat.Tasks.SinglePoleBalancingNv;
using SharpNeat.Tasks.Xor;

namespace TestApp1
{
    public static class EvolutionAlgorithmFactory
    {
        public static NeatEvolutionAlgorithm<double> CreateNeatEvolutionAlgorithm_Xor()
        {
            var experimentFactory = new XorExperimentFactory();
            return Utils.CreateNeatEvolutionAlgorithm(
                experimentFactory,
                "config/xor.config.json");
        }

        public static NeatEvolutionAlgorithm<double> CreateNeatEvolutionAlgorithm_Binary6()
        {
            var experimentFactory = new BinarySixMultiplexerExperimentFactory();
            return Utils.CreateNeatEvolutionAlgorithm(
                experimentFactory,
                "config/binary-six-multiplexer.config.json");
        }

        public static NeatEvolutionAlgorithm<double> CreateNeatEvolutionAlgorithm_Binary11()
        {
            var experimentFactory = new BinaryElevenMultiplexerExperimentFactory();
            return Utils.CreateNeatEvolutionAlgorithm(
                experimentFactory,
                "config/binary-eleven-multiplexer.config.json");
        }

        public static NeatEvolutionAlgorithm<double> CreateNeatEvolutionAlgorithm_GenerativeSinewave()
        {
            var experimentFactory = new GenerativeFnRegressionExperimentFactory();
            return Utils.CreateNeatEvolutionAlgorithm(
                experimentFactory,
                "config/generative-sinewave.config.json");
        }

        public static NeatEvolutionAlgorithm<double> CreateNeatEvolutionAlgorithm_SinglePoleBalancing()
        {
            var experimentFactory = new SinglePoleBalancingExperimentFactory();
            return Utils.CreateNeatEvolutionAlgorithm(
                experimentFactory,
                "config/single-pole-balancing.config.json");
        }

        public static NeatEvolutionAlgorithm<double> CreateNeatEvolutionAlgorithm_SinglePoleBalancingNv()
        {
            var experimentFactory = new SinglePoleBalancingNvExperimentFactory();
            return Utils.CreateNeatEvolutionAlgorithm(
                experimentFactory,
                "config/single-pole-balancing-nv.config.json");
        }
    }
}
