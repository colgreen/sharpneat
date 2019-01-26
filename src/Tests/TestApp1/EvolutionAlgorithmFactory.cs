using Redzen.Random;
using SharpNeat.BlackBox;
using SharpNeat.Evaluation;
using SharpNeat.EvolutionAlgorithm;
using SharpNeat.Neat;
using SharpNeat.Neat.DistanceMetrics.Double;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Genome.Double;
using SharpNeat.Neat.Reproduction.Asexual;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation;
using SharpNeat.Neat.Reproduction.Sexual;
using SharpNeat.Neat.Speciation.GeneticKMeans;
using SharpNeat.NeuralNet;
using SharpNeat.NeuralNet.Double.ActivationFunctions;
using SharpNeat.Tasks.BinaryElevenMultiplexer;
using SharpNeat.Tasks.BinarySixMultiplexer;
using SharpNeat.Tasks.BinaryThreeMultiplexer;

namespace TestApp1
{
    public class EvolutionAlgorithmFactory
    {
        NeatEvolutionAlgorithmSettings _eaSettings;
        MetaNeatGenome<double> _metaNeatGenome;
        NeatPopulation<double> _neatPop;

        #region Public Methods

        public NeatEvolutionAlgorithm<double> CreateNeatEvolutionAlgorithm()
        {
            // Create an initial population.
            _metaNeatGenome = CreateMetaNeatGenome();
            _eaSettings = new NeatEvolutionAlgorithmSettings();
            _eaSettings.SpeciesCount = 40;
            _neatPop = CreatePopulation(_metaNeatGenome, 600);

            // Create a genome evaluator.
            IGenomeListEvaluator<NeatGenome<double>> genomeListEvaluator = CreateGenomeListEvaluator();

            // Create a speciation strategy instance.
            var distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
            var speciationStrategy = new SharpNeat.Neat.Speciation.GeneticKMeans.Parallelized.GeneticKMeansSpeciationStrategy<double>(distanceMetric, 5);

            // Create an asexual reproduction settings object (default settings).
            var reproductionAsexualSettings = new NeatReproductionAsexualSettings();

            // Create a sexual reproduction settings object (default settings).
            var reproductionSexualSettings = new NeatReproductionSexualSettings();

            // Create a connection weight mutation scheme.
            var weightMutationScheme = WeightMutationSchemeFactory.CreateDefaultScheme(_metaNeatGenome.ConnectionWeightScale);

            // Pull all of the parts together into an evolution algorithm instance.
            var ea = new NeatEvolutionAlgorithm<double>(
                _eaSettings,
                genomeListEvaluator,
                speciationStrategy,
                _neatPop,
                reproductionAsexualSettings,
                reproductionSexualSettings,
                weightMutationScheme);

            return ea;
        }

        #endregion

        #region Private Static Methods

        private static MetaNeatGenome<double> CreateMetaNeatGenome()
        {
            var activationFnFactory = new DefaultActivationFunctionFactory<double>();

            MetaNeatGenome<double> metaNeatGenome = new MetaNeatGenome<double>(
                inputNodeCount: 4, 
                outputNodeCount: 1,
                isAcyclic: true,
                activationFn: activationFnFactory.GetActivationFunction("LeakyReLU"));

            return metaNeatGenome;
        }

        private static NeatPopulation<double> CreatePopulation(
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

        private IGenomeListEvaluator<NeatGenome<double>> CreateGenomeListEvaluator()
        {
            var genomeDecoder = new NeatGenomeAcyclicDecoder(true);
            var phenomeEvaluationScheme = new BinaryThreeMultiplexerEvaluationScheme();

            var genomeListEvaluator = GenomeListEvaluatorFactory.CreateEvaluator(
                genomeDecoder,
                phenomeEvaluationScheme,
                createConcurrentEvaluator: false);

            return genomeListEvaluator;
        }

        #endregion
    }
}
