using System.Collections.Generic;
using SharpNeat.Neat;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;

namespace SharpNeatLib.Tests.Neat.Genome
{
    public static class NestGenomeTestUtils
    {
        public static NeatPopulation<double> CreateNeatPopulation()
        {
            MetaNeatGenome<double> metaNeatGenome = new MetaNeatGenome<double>(
                inputNodeCount: 1,
                outputNodeCount: 1,
                isAcyclic: false,
                activationFn: new SharpNeat.NeuralNets.Double.ActivationFunctions.ReLU());

            var genome = CreateNeatGenome(metaNeatGenome);
            var genomeList = new List<NeatGenome<double>>() { genome };
            return new NeatPopulation<double>(metaNeatGenome, genomeList);
        }

        public static NeatGenome<double> CreateNeatGenome(MetaNeatGenome<double> metaNeatGenome)
        {
            var connGenes = new ConnectionGenes<double>(12);
            connGenes[0] =   (0, 3, 0.1, 12);
            connGenes[1] =   (0, 4, 0.2, 13);
            connGenes[2] =   (0, 5, 0.3, 14);

            connGenes[3] =   (3, 6, 0.4, 15);
            connGenes[4] =   (4, 7, 0.5, 16);
            connGenes[5] =   (5, 8, 0.6, 17);
 
            connGenes[6] =   (6, 9, 0.7, 18);
            connGenes[7] =   (7, 10, 0.8, 19);
            connGenes[8] =   (8, 11, 0.9, 20);

            connGenes[9] =   (9, 1, 1.0, 21);
            connGenes[10] = (10, 1, 1.1, 22);
            connGenes[11] = (11, 1, 1.2, 23);

            var genome = new NeatGenome<double>(metaNeatGenome, 0, 0, connGenes);
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
