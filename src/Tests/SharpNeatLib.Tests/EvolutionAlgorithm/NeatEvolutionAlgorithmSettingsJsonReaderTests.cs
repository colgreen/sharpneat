using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.EvolutionAlgorithm;

namespace SharpNeatLib.Tests.EvolutionAlgorithm
{
    [TestClass]
    public class NeatEvolutionAlgorithmSettingsJsonReaderTests
    {
        [TestMethod]
        public void Read()
        {
            JsonDocument jdoc = JsonDocument.Parse(
@"{
    ""speciesCount"":1111,
    ""elitismProportion"":0.11,
    ""selectionProportion"":0.22,
    ""offspringAsexualProportion"":0.33,
    ""offspringSexualProportion"":0.44,
    ""interspeciesMatingProportion"":0.55,
    ""statisticsMovingAverageHistoryLength"":2222
}");
            // Init a default settings object.
            var eaSettings = new NeatEvolutionAlgorithmSettings();

            // Read json properties into the settings object.
            NeatEvolutionAlgorithmSettingsJsonReader.Read(eaSettings, jdoc.RootElement);

            // Assert the expected values.
            Assert.AreEqual(1111, eaSettings.SpeciesCount);
            Assert.AreEqual(0.11, eaSettings.ElitismProportion);
            Assert.AreEqual(0.22, eaSettings.SelectionProportion);
            Assert.AreEqual(0.33, eaSettings.OffspringAsexualProportion);
            Assert.AreEqual(0.44, eaSettings.OffspringSexualProportion);
            Assert.AreEqual(0.55, eaSettings.InterspeciesMatingProportion);
            Assert.AreEqual(2222, eaSettings.StatisticsMovingAverageHistoryLength);
        }
    }
}
