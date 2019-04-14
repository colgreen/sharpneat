using SharpNeat.Evaluation;
using SharpNeat.Neat;
using SharpNeat.Neat.ComplexityRegulation;
using SharpNeat.Neat.DistanceMetrics.Double;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Genome.Double;
using SharpNeat.Neat.Reproduction.Asexual;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation;
using SharpNeat.Neat.Reproduction.Sexual;
using SharpNeat.Tasks.FunctionRegression;
using SharpNeat.Tasks.GenerativeFunctionRegression;
using static TestApp1.EvolutionAlgorithmFactoryUtils;

namespace TestApp1
{
    public class EvolutionAlgorithmFactorySinewave
    {
        NeatEvolutionAlgorithmSettings _eaSettings;
        MetaNeatGenome<double> _metaNeatGenome;
        NeatPopulation<double> _neatPop;

        #region Public Methods

        public NeatEvolutionAlgorithm<double> CreateNeatEvolutionAlgorithm()
        {
            // Create a genome evaluator.
            //var genomeListEvaluator = CreateGenomeListEvaluator_BinaryElevenMultiplexer(out int inputCount, out int outputCount);
            var genomeListEvaluator = CreateGenomeListEvaluator(out int inputCount, out int outputCount);

            // Create an initial population.
            _metaNeatGenome = CreateMetaNeatGenome(inputCount, outputCount, isAcyclic: false, activationFnName: "LeakyReLU");
            _eaSettings = new NeatEvolutionAlgorithmSettings();
            _eaSettings.SpeciesCount = 40;
            _neatPop = CreatePopulation(_metaNeatGenome, 600);

            // Create a complexity regulation strategy.
            var complexityRegulationStrategy = new RelativeComplexityRegulationStrategy(10, 10.0);

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
                complexityRegulationStrategy,
                reproductionAsexualSettings,
                reproductionSexualSettings,
                weightMutationScheme);

            return ea;
        }

        #endregion

        #region Private Static Methods

        private IGenomeListEvaluator<NeatGenome<double>> CreateGenomeListEvaluator(
            out int inputCount, out int outputCount)
        {
            var genomeDecoder = NeatGenomeDecoderFactory.CreateGenomeDecoder(1, true);

            // Create function regression evaluation scheme.
            int sampleResolution = 80;
            double sampleMin = 0;
            double sampleMax = 25.13274123;
            var paramSamplingInfo = new ParamSamplingInfo(sampleMin, sampleMax, sampleResolution);
            IBlackBoxEvaluationScheme<double> blackBoxEvaluationScheme = new GenerativeFuncRegressionEvaluationScheme(FunctionFactory.GetFunction(FunctionId.Sin), paramSamplingInfo, 0.3);

            var genomeListEvaluator = GenomeListEvaluatorFactory.CreateEvaluator(
                genomeDecoder,
                blackBoxEvaluationScheme,
                createConcurrentEvaluator: true);

            inputCount = blackBoxEvaluationScheme.InputCount;
            outputCount = blackBoxEvaluationScheme.OutputCount;
            return genomeListEvaluator;
        }

        #endregion
    }
}
