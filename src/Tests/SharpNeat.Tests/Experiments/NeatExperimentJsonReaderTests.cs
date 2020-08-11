using System.Text.Json;
using Moq;
using SharpNeat.Evaluation;
using Xunit;

namespace SharpNeat.Experiments.Tests
{
    public class NeatExperimentJsonReaderTests
    {
        [Fact]
        public void Read()
        {
            JsonDocument jdoc = JsonDocument.Parse(
@"{
    ""description"":""bar description"",
    ""isAcyclic"":false,
    ""cyclesPerActivation"":111,
    ""activationFnName"":""bar-activation-fn"",

    ""evolutionAlgorithmSettings"":
    {
        ""speciesCount"":1111,
        ""elitismProportion"":0.11,
        ""selectionProportion"":0.22,
        ""offspringAsexualProportion"":0.33,
        ""offspringSexualProportion"":0.44,
        ""interspeciesMatingProportion"":0.55,
        ""statisticsMovingAverageHistoryLength"":2222
    },
    ""reproductionAsexualSettings"":
    {
        ""connectionWeightMutationProbability"":0.11,
        ""addNodeMutationProbability"":0.22,
        ""addConnectionMutationProbability"":0.33,
        ""deleteConnectionMutationProbability"":0.44
    },
    ""reproductionSexualSettings"":
    {
        ""secondaryParentGeneProbability"":0.11
    },

    ""populationSize"":222,
    ""initialInterconnectionsProportion"":0.33,
    ""connectionWeightScale"":4.44,

    ""complexityRegulationStrategy"":
    {
        ""strategyName"": ""absolute"",
        ""complexityCeiling"": 10,
        ""minSimplifcationGenerations"": 10
    },

    ""enableHardwareAcceleratedNeuralNets"":true,
    ""enableHardwareAcceleratedActivationFunctions"":true,
    ""degreeOfParallelism"":6
}");

            // Create a mock evaluation scheme.
            var evalScheme = new Mock<IBlackBoxEvaluationScheme<double>>();

            // Init a default settings object.
            var experiment = new NeatExperiment<double>(
                evalScheme.Object, "foo-experiment" );

            // Read json properties into the experiment object.
            NeatExperimentJsonReader<double>.Read(experiment, jdoc.RootElement);

            // Assert the expected values.
            Assert.Equal("bar description", experiment.Description);
            Assert.False(experiment.IsAcyclic);
            Assert.Equal(111, experiment.CyclesPerActivation);
            Assert.Equal("bar-activation-fn", experiment.ActivationFnName);

            var eaSettings = experiment.NeatEvolutionAlgorithmSettings;
            Assert.Equal(1111, eaSettings.SpeciesCount);
            Assert.Equal(0.11, eaSettings.ElitismProportion);
            Assert.Equal(0.22, eaSettings.SelectionProportion);
            Assert.Equal(0.33, eaSettings.OffspringAsexualProportion);
            Assert.Equal(0.44, eaSettings.OffspringSexualProportion);
            Assert.Equal(0.55, eaSettings.InterspeciesMatingProportion);
            Assert.Equal(2222, eaSettings.StatisticsMovingAverageHistoryLength);

            var asexualSettings = experiment.ReproductionAsexualSettings;
            Assert.Equal(0.11, asexualSettings.ConnectionWeightMutationProbability);
            Assert.Equal(0.22, asexualSettings.AddNodeMutationProbability);
            Assert.Equal(0.33, asexualSettings.AddConnectionMutationProbability);
            Assert.Equal(0.44, asexualSettings.DeleteConnectionMutationProbability);

            var sexualSettings = experiment.ReproductionSexualSettings;
            Assert.Equal(0.11, sexualSettings.SecondaryParentGeneProbability);

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
}
