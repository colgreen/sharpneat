using Redzen.Random;
using SharpNeat.Neat;
using SharpNeat.Neat.Genome;
using SharpNeat.NeuralNet;

namespace TestApp1
{
    public static class EvolutionAlgorithmFactoryUtils
    {
        public static MetaNeatGenome<double> CreateMetaNeatGenome(
            int inputCount, int outputCount,
            bool isAcyclic,
            string activationFnName)
        {
            var activationFnFactory = new DefaultActivationFunctionFactory<double>();

            var metaNeatGenome = new MetaNeatGenome<double>(
                inputNodeCount: inputCount, 
                outputNodeCount: outputCount,
                isAcyclic: isAcyclic,
                activationFn: activationFnFactory.GetActivationFunction(activationFnName));

            return metaNeatGenome;
        }

        public static NeatPopulation<double> CreatePopulation(
            MetaNeatGenome<double> metaNeatGenome,
            int popSize)
        {
            NeatPopulation<double> pop = NeatPopulationFactory<double>.CreatePopulation(
                metaNeatGenome,
                connectionsProportion: 0.1,
                popSize: popSize,
                rng: RandomDefaults.CreateRandomSource());

            return pop;
        }
    }
}
