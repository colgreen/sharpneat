using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Reproduction.Sexual;

namespace SharpNeatLib.Tests.Neat.Reproduction.Sexual
{
    [TestClass]
    public class NeatReproductionSexualSettingsJsonReaderTests
    {
        [TestMethod]
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
            Assert.AreEqual(0.11, settings.SecondaryParentGeneProbability);
        }
    }
}
