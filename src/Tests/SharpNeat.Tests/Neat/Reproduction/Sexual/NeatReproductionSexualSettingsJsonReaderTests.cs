using System.Text.Json;
using Xunit;

namespace SharpNeat.Neat.Reproduction.Sexual.Tests
{
    public class NeatReproductionSexualSettingsJsonReaderTests
    {
        [Fact]
        public void Read()
        {
            JsonDocument jdoc = JsonDocument.Parse(
@"{
    ""secondaryParentGeneProbability"":0.11
}");
            // Init a default settings object.
            var settings = new NeatReproductionSexualSettings();

            // Read json properties into the settings object.
            NeatReproductionSexualSettingsJsonReader.Read(settings, jdoc.RootElement);

            // Assert the expected values.
            Assert.Equal(0.11, settings.SecondaryParentGeneProbability);
        }
    }
}
