using System.IO.Compression;
using SharpNeat.Neat.Genome.Tests;
using Xunit;

namespace SharpNeat.Neat.Genome.IO.Tests;

public class NeatPopulationIOTests
{
    #region Test Methods

    [Fact]
    public void SaveAndLoadPopulationToFolder()
    {
        // Create a test population.
        NeatPopulation<double> pop = NestGenomeTestUtils.CreateNeatPopulation();

        // Build path to test population folder.
        string parentPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "test-pops");

        // Delete folder if it already exists.
        if(Directory.Exists(parentPath))
            Directory.Delete(parentPath, true);

        // Create an empty parent folder to save populations into.
        Directory.CreateDirectory(parentPath);

        // Save the population to the unit test output folder.
        NeatPopulationSaver.SaveToFolder(pop.GenomeList, parentPath, "pop1");

        // Load the population.
        NeatPopulationLoader<double> loader = NeatPopulationLoaderFactory.CreateLoaderDouble(pop.MetaNeatGenome);
        string populationFolderPath = Path.Combine(parentPath, "pop1");
        List<NeatGenome<double>> genomeListLoaded = loader.LoadFromFolder(populationFolderPath);

        // Compare the loaded genomes with the original genome list.
        IOTestUtils.CompareGenomeLists(pop.GenomeList, genomeListLoaded);
    }

    [Fact]
    public void SaveAndLoadPopulationToZipArchive()
    {
        // Create a test population.
        NeatPopulation<double> pop = NestGenomeTestUtils.CreateNeatPopulation();

        // Build path to test population folder.
        string parentPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "test-pops");

        string filepath = Path.Combine(parentPath, "pop2.zip");

        // Delete file if it already exists.
        if(File.Exists(filepath))
            File.Delete(filepath);

        // Save the population to the unit test output folder.
        NeatPopulationSaver.SaveToZipArchive(
            pop.GenomeList, filepath,
            CompressionLevel.Optimal);

        // Load the population.
        NeatPopulationLoader<double> loader = NeatPopulationLoaderFactory.CreateLoaderDouble(pop.MetaNeatGenome);
        List<NeatGenome<double>> genomeListLoaded = loader.LoadFromZipArchive(filepath);

        // Compare the loaded genomes with the original genome list.
        IOTestUtils.CompareGenomeLists(pop.GenomeList, genomeListLoaded);
    }

    #endregion
}
