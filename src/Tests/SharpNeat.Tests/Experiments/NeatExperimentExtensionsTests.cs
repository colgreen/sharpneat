using Moq;
using SharpNeat.Evaluation;
using SharpNeat.Experiments.ConfigModels;
using SharpNeat.IO;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Reproduction.Asexual;
using SharpNeat.Neat.Reproduction.Recombination;
using Xunit;

namespace SharpNeat.Experiments;

public class NeatExperimentExtensionsTests
{
    [Fact]
    public void Configure()
    {
        string json = """
        {
            "description":"bar description",
            "isAcyclic":false,
            "cyclesPerActivation":111,
            "activationFnName":"bar-activation-fn",

            "evolutionAlgorithm":
            {
                "speciesCount":1111,
                "elitismProportion":0.11,
                "selectionProportion":0.22,
                "offspringAsexualProportion":0.39,
                "offspringRecombinationProportion":0.61,
                "interspeciesMatingProportion":0.55,
                "statisticsMovingAverageHistoryLength":2222
            },
            "asexualReproduction":
            {
                "connectionWeightMutationProbability":0.11,
                "addNodeMutationProbability":0.22,
                "addConnectionMutationProbability":0.33,
                "deleteConnectionMutationProbability":0.34
            },
            "recombination":
            {
                "secondaryParentGeneProbability":0.11
            },

            "populationSize":222,
            "initialInterconnectionsProportion":0.33,
            "connectionWeightScale":4.44,

            "complexityRegulationStrategy":
            {
                "strategyName": "absolute",
                "complexityCeiling": 10,
                "minSimplifcationGenerations": 10
            },

            "enableHardwareAcceleratedNeuralNets":true,
            "enableHardwareAcceleratedActivationFunctions":true,
            "degreeOfParallelism":6
        }
        """;

        ExperimentConfig experimentConfig = JsonUtils.Deserialize<ExperimentConfig>(json);

        // Create a mock evaluation scheme.
        var evalScheme = new Mock<IBlackBoxEvaluationScheme<double>>();

        // Init a default settings object.
        var experiment = new NeatExperiment<double>(
            evalScheme.Object, "foo-experiment")
        {
            EvolutionAlgorithmSettings = experimentConfig.EvolutionAlgorithm ?? new NeatEvolutionAlgorithmSettings(),
            AsexualReproductionSettings = experimentConfig.AsexualReproduction ?? new NeatAsexualReproductionSettings(),
            RecombinationSettings = experimentConfig.Recombination ?? new NeatRecombinationSettings()
        };

        // Apply the experiment config.
        experiment.Configure(experimentConfig);

        // Assert the expected values.
        Assert.Equal("bar description", experiment.Description);
        Assert.False(experiment.IsAcyclic);
        Assert.Equal(111, experiment.CyclesPerActivation);
        Assert.Equal("bar-activation-fn", experiment.ActivationFnName);

        var eaSettings = experiment.EvolutionAlgorithmSettings;
        Assert.Equal(1111, eaSettings.SpeciesCount);
        Assert.Equal(0.11, eaSettings.ElitismProportion);
        Assert.Equal(0.22, eaSettings.SelectionProportion);
        Assert.Equal(0.39, eaSettings.OffspringAsexualProportion);
        Assert.Equal(0.61, eaSettings.OffspringRecombinationProportion);
        Assert.Equal(0.55, eaSettings.InterspeciesMatingProportion);
        Assert.Equal(2222, eaSettings.StatisticsMovingAverageHistoryLength);

        var asexualSettings = experiment.AsexualReproductionSettings;
        Assert.Equal(0.11, asexualSettings.ConnectionWeightMutationProbability);
        Assert.Equal(0.22, asexualSettings.AddNodeMutationProbability);
        Assert.Equal(0.33, asexualSettings.AddConnectionMutationProbability);
        Assert.Equal(0.34, asexualSettings.DeleteConnectionMutationProbability);

        var recombinationSettings = experiment.RecombinationSettings;
        Assert.Equal(0.11, recombinationSettings.SecondaryParentGeneProbability);

        Assert.Equal(222, experiment.PopulationSize);
        Assert.Equal(0.33, experiment.InitialInterconnectionsProportion);
        Assert.Equal(4.44, experiment.ConnectionWeightScale);

        var complexityRegulationStrategy = experiment.ComplexityRegulationStrategy;
        Assert.Equal("AbsoluteComplexityRegulationStrategy", complexityRegulationStrategy.GetType().Name);

        Assert.Equal(6, experiment.DegreeOfParallelism);
        Assert.True(experiment.EnableHardwareAcceleratedNeuralNets);
        Assert.True(experiment.EnableHardwareAcceleratedActivationFunctions);
    }
}
