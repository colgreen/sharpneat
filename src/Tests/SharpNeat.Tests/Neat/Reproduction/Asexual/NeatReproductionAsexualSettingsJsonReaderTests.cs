using System.Text.Json;
using Xunit;

namespace SharpNeat.Neat.Reproduction.Asexual.Tests;

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
    ""deleteConnectionMutationProbability"":0.34
}");
        // Init a default settings object.
        var settings = new NeatReproductionAsexualSettings();

        // Read json properties into the settings object.
        NeatReproductionAsexualSettingsJsonReader.Read(settings, jdoc.RootElement);

        // Assert the expected values.
        Assert.Equal(0.11, settings.ConnectionWeightMutationProbability);
        Assert.Equal(0.22, settings.AddNodeMutationProbability);
        Assert.Equal(0.33, settings.AddConnectionMutationProbability);
        Assert.Equal(0.34, settings.DeleteConnectionMutationProbability);
    }
}
