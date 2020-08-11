using System.IO;
using SharpNeat.NeuralNets.Double.ActivationFunctions;
using Xunit;

namespace SharpNeat.Neat.Genome.IO.Tests
{
    public class NeatGenomeIOTests
    {
        #region Test Methods

        [Fact]
        public void SaveAndLoadGenome()
        {
            var metaNeatGenome = new MetaNeatGenome<double>(3, 2, true, new ReLU());
            var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(metaNeatGenome);

            // Simple acyclic graph.
            var connGenes = new ConnectionGenes<double>(6);
            connGenes[0] = (0, 3, 0.123);
            connGenes[1] = (1, 3, 1.234);
            connGenes[2] = (2, 3, -0.5835);
            connGenes[3] = (2, 4, 5.123456789);
            connGenes[4] = (2, 5, 2.5);
            connGenes[5] = (5, 4, 5.4);

            // Wrap in a genome.
            NeatGenome<double> genome = genomeBuilder.Create(0, 0, connGenes);

            // Create a memory stream to save the genome into.
            using(MemoryStream ms = new MemoryStream(1024)) 
            {    
                // Save the genome.
                NeatGenomeSaver<double>.Save(genome,  ms);

                // Load the genome.
                ms.Position = 0;
                NeatGenomeLoader<double> loader = NeatGenomeLoaderFactory.CreateLoaderDouble(metaNeatGenome);
                NeatGenome<double> genomeLoaded = loader.Load(ms);

                // Compare the original genome with the loaded genome.
                IOTestUtils.CompareGenomes(genome, genomeLoaded);
            }
        }

        #endregion
    }
}
