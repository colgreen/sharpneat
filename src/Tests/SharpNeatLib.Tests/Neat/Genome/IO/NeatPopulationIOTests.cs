using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Genome.IO;
namespace SharpNeat.Tests.Neat.Genome.IO
{
    [TestClass]
    public class NeatPopulationIOTests
    {
        // Note. The test framework sets this auto-property just prior to running the unit test methods.
        public TestContext TestContext { get; set; }

        #region Test Methods

        [TestMethod]
        [TestCategory("NeatPopulationIO")]
        public void SaveAndLoadPopulationToFolder()
        {
            // Create a test population.
            NeatPopulation<double> pop = NestGenomeTestUtils.CreateNeatPopulation();

            // Create a parent folder to save populations into.
            string parentPath = Path.Combine(TestContext.TestRunDirectory, "test-pops");
            Directory.CreateDirectory(parentPath);

            // Save the population to the unit test output folder.
            NeatPopulationSaver<double>.SaveToFolder(pop.GenomeList, parentPath, "pop1");

            // Load the population.
            NeatPopulationLoader<double> loader = NeatPopulationLoaderFactory.CreateLoaderDouble(pop.MetaNeatGenome);
            string populationFolderPath = Path.Combine(parentPath, "pop1");
            List<NeatGenome<double>> genomeListLoaded = loader.LoadFromFolder(populationFolderPath);

            // Compare the loaded genomes with the original genome list.
            IOTestUtils.CompareGenomeLists(pop.GenomeList, genomeListLoaded);
        }

        [TestMethod]
        [TestCategory("NeatPopulationIO")]
        public void SaveAndLoadPopulationToZipArchive()
        {
            // Create a test population.
            NeatPopulation<double> pop = NestGenomeTestUtils.CreateNeatPopulation();

            // Create a parent folder to save populations into.
            string parentPath = Path.Combine(TestContext.TestRunDirectory, "test-pops");
            Directory.CreateDirectory(parentPath);

            // Save the population to the unit test output folder.
            NeatPopulationSaver<double>.SaveToZipArchive(pop.GenomeList, parentPath, "pop2", System.IO.Compression.CompressionLevel.Optimal);

            // Load the population.
            NeatPopulationLoader<double> loader = NeatPopulationLoaderFactory.CreateLoaderDouble(pop.MetaNeatGenome);
            string populationZipPath = Path.Combine(parentPath, "pop2.zip");
            List<NeatGenome<double>> genomeListLoaded = loader.LoadFromZipArchive(populationZipPath);

            // Compare the loaded genomes with the original genome list.
            IOTestUtils.CompareGenomeLists(pop.GenomeList, genomeListLoaded);
        }

        #endregion
    }
}
