using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Genome.IO;
using SharpNeat.NeuralNet.Double.ActivationFunctions;

namespace SharpNeat.Tests.Neat.Genome.IO
{
    [TestClass]
    public class NeatGenomeLoaderTests
    {
        [TestMethod]
        [TestCategory("NeatGenomeLoader")]
        [DeploymentItem("TestData", "TestData")]  
        public void LoadGenome()
        {
            var metaNeatGenome = new MetaNeatGenome<double>(3, 2, true, new ReLU());

            // Load test genome.
            NeatGenomeLoader<double> loader = NeatGenomeLoaderFactory.CreateLoaderDouble(metaNeatGenome);
            NeatGenome<double> genomeLoaded = loader.Load("TestData/example1.genome");

            // Manually build an equivalent genome.
            NeatGenome<double> genomeBuilt = CreateGenome1(metaNeatGenome);

            // Compare the two genomes.
            IOTestUtils.CompareGenomes(genomeLoaded, genomeBuilt);
        }

        #region Private Methods

        private NeatGenome<double> CreateGenome1(MetaNeatGenome<double> metaNeatGenome)
        {
            var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(metaNeatGenome);

            // Define a genome that matches the one defined in example1.genome.
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
}
