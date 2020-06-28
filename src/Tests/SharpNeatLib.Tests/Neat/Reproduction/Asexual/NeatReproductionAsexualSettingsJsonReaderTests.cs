using System.Text.Json;
using SharpNeat.Neat.Reproduction.Asexual;
using Xunit;

namespace SharpNeatLib.Tests.Neat.Reproduction.Asexual
{
    public class NeatReproductionAsexualSettingsJsonReaderTests
    {
        [Fact]
        public void Read()
        {
            JsonDocument jdoc = JsonDocument.Parse(
@"{
    ""connectionWeightMutationProbability"":0.11,
    ""addNodeMutationProbability"":0.22,
    ""addConnectionMutationProbability"":0.33,
    ""deleteConnectionMutationProbability"":0.44
}");
            // Init a default settings object.
            var settings = new NeatReproductionAsexualSettings();

            // Read json properties into the settings object.
            NeatReproductionAsexualSettingsJsonReader.Read(settings, jdoc.RootElement);

            // Assert the expected values.
            Assert.Equal(0.11, settings.ConnectionWeightMutationProbability);
            Assert.Equal(0.22, settings.AddNodeMutationProbability);
            Assert.Equal(0.33, settings.AddConnectionMutationProbability);
            Assert.Equal(0.44, settings.DeleteConnectionMutationProbability);
        }
    }
}
