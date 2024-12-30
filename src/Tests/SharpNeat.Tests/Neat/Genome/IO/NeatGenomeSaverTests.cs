using System.Globalization;
using SharpNeat.NeuralNets.ActivationFunctions;
using Xunit;

namespace SharpNeat.Neat.Genome.IO;

public class NeatGenomeSaverTests
{
    [Fact]
    public void SaveGenome()
    {
        // Manually build a genome.
        var metaNeatGenome = MetaNeatGenome<double>.CreateAcyclic(3, 2, new ReLU());
        using NeatGenome<double> genomeBuilt = CreateGenome1(metaNeatGenome);

        // Save the genome into a MemoryStream.
        using MemoryStream ms = new();
        NeatGenomeSaver.Save(genomeBuilt, ms);

        // Load the saved genome.
        ms.Position = 0;
        NeatGenome<double> genomeLoaded = NeatGenomeLoader.Load(ms, metaNeatGenome, 0);

        // Compare the two genomes.
        IOTestUtils.CompareGenomes(genomeLoaded, genomeBuilt);
    }

    [Fact]
    public void SaveGenome_FrenchLocale()
    {
        // Store the current/default culture info.
        Thread currentThread = Thread.CurrentThread;
        CultureInfo defaultCulture = currentThread.CurrentCulture;

        // Change the default culture to French (which uses e.g. a comma as a decimal separator).
        CultureInfo frenchCulture = new("fr-FR");
        Thread.CurrentThread.CurrentCulture = frenchCulture;

        try
        {
            // Manually build a genome.
            var metaNeatGenome = MetaNeatGenome<double>.CreateAcyclic(3, 2, new ReLU());
            using NeatGenome<double> genomeBuilt = CreateGenome1(metaNeatGenome);

            // Save the genome into a MemoryStream.
            using MemoryStream ms = new();
            NeatGenomeSaver.Save(genomeBuilt, ms);

            // Load the saved genome.
            ms.Position = 0;
            NeatGenome<double> genomeLoaded = NeatGenomeLoader.Load(ms, metaNeatGenome, 0);

            // Compare the original genome with the loaded one.
            IOTestUtils.CompareGenomes(genomeLoaded, genomeBuilt);
        }
        finally
        {
            // Restore the current thread's default culture; otherwise we may break other unit tests that use this thread.
            Thread.CurrentThread.CurrentCulture = defaultCulture;
        }
    }

    #region Private Static Methods

    private static NeatGenome<double> CreateGenome1(MetaNeatGenome<double> metaNeatGenome)
    {
        var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(metaNeatGenome);

        // Define a genome that matches the one defined in example1.net.
        var connGenes = new ConnectionGenes<double>(12);
        connGenes[0] = (0, 5, 0.5);
        connGenes[1] = (0, 7, 0.7);
        connGenes[2] = (0, 3, 0.3);
        connGenes[3] = (1, 5, 1.5);
        connGenes[4] = (1, 7, 1.7);
        connGenes[5] = (1, 3, 1.3);
        connGenes[6] = (1, 6, 1.6);
        connGenes[7] = (1, 8, 1.8);
        connGenes[8] = (1, 4, 1.4);
        connGenes[9] = (2, 6, 2.6);
        connGenes[10] = (2, 8, 2.8);
        connGenes[11] = (2, 4, 2.4);

        // Ensure the connections are sorted correctly.
        connGenes.Sort();

        // Wrap in a genome.
        NeatGenome<double> genome = genomeBuilder.Create(0, 0, connGenes);

        return genome;
    }

    #endregion
}
