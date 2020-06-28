using System.Collections.Generic;
using SharpNeat.Neat;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;

namespace SharpNeat.Tests.Neat.Genome
{
    public static class NestGenomeTestUtils
    {
        public static NeatPopulation<double> CreateNeatPopulation()
        {
            var metaNeatGenome = new MetaNeatGenome<double>(
                inputNodeCount: 1,
                outputNodeCount: 1,
                isAcyclic: false,
                activationFn: new NeuralNet.Double.ActivationFunctions.ReLU());

            var genomeBuilder = NeatGenomeBuilderFactory<double>.Create(metaNeatGenome);

            var genome = CreateNeatGenome(genomeBuilder);
            var genome2 = CreateNeatGenome2(genomeBuilder);
            var genomeList = new List<NeatGenome<double>>() { genome, genome2 };
            return new NeatPopulation<double>(metaNeatGenome, genomeBuilder, genomeList);
        }

        public static NeatGenome<double> CreateNeatGenome(
            INeatGenomeBuilder<double> genomeBuilder)
        {
            var connGenes = new ConnectionGenes<double>(12);
            connGenes[0] =   (0, 3, 0.1);
            connGenes[1] =   (0, 4, 0.2);
            connGenes[2] =   (0, 5, 0.3);

            connGenes[3] =   (3, 6, 0.4);
            connGenes[4] =   (4, 7, 0.5);
            connGenes[5] =   (5, 8, 0.6);
 
            connGenes[6] =   (6, 9, 0.7);
            connGenes[7] =   (7, 10, 0.8);
            connGenes[8] =   (8, 11, 0.9);

            connGenes[9] =   (9, 1, 1.0);
            connGenes[10] = (10, 1, 1.1);
            connGenes[11] = (11, 1, 1.2);

            var genome = genomeBuilder.Create(0, 0, connGenes);
            return genome;
        }

        public static NeatGenome<double> CreateNeatGenome2(
            INeatGenomeBuilder<double> genomeBuilder)
        {
            var connGenes = new ConnectionGenes<double>(12);
            connGenes[0] =   (0, 3, 0.3);
            connGenes[1] =   (0, 4, 0.4);
            connGenes[2] =   (0, 5, 0.5);

            connGenes[3] =   (3, 6, 3.6);
            connGenes[4] =   (4, 7, 4.7);
            connGenes[5] =   (5, 8, 5.8);
 
            connGenes[6] =   (6, 9, 6.9);
            connGenes[7] =   (7, 10, 7.1);
            connGenes[8] =   (8, 11, 8.11);

            connGenes[9] =   (9, 1, 9.1);
            connGenes[10] = (10, 1, 10.1);
            connGenes[11] = (11, 1, 11.1);

            var genome = genomeBuilder.Create(1, 0, connGenes);
            return genome;
        }

        public static HashSet<int> GetNodeIdSet(NeatGenome<double> genome)
        {
            var idSet = new HashSet<int>();
            foreach(var conn in genome.ConnectionGenes._connArr)
            {
                idSet.Add(conn.SourceId);
                idSet.Add(conn.TargetId);
            }
            return idSet;
        }

        public static HashSet<DirectedConnection> GetDirectedConnectionSet(NeatGenome<double> genome)
        {
            var idSet = new HashSet<DirectedConnection>();
            foreach(var conn in genome.ConnectionGenes._connArr) {
                idSet.Add(conn);
            }
            return idSet;
        }

    }
}
