using System;
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
            var connArr = new ConnectionGene<double>[12];
            connArr[0] = new ConnectionGene<double>(12, 0, 3, 1.0);
            connArr[1] = new ConnectionGene<double>(13, 0, 4, 1.0);
            connArr[2] = new ConnectionGene<double>(14, 0, 5, 1.0);

            connArr[3] = new ConnectionGene<double>(15, 3, 6, 1.0);
            connArr[4] = new ConnectionGene<double>(16, 4, 7, 1.0);
            connArr[5] = new ConnectionGene<double>(17, 5, 8, 1.0);
            
            connArr[6] = new ConnectionGene<double>(18, 6, 9, 1.0);
            connArr[7] = new ConnectionGene<double>(19, 7, 10, 1.0);
            connArr[8] = new ConnectionGene<double>(20, 8, 11, 1.0);

            connArr[9] = new ConnectionGene<double>(21, 9, 1, 1.0);
            connArr[10] = new ConnectionGene<double>(22, 10, 1, 1.0);
            connArr[11] = new ConnectionGene<double>(23, 11, 1, 1.0);

            ConnectionGeneUtils.Sort(connArr);

            var genome = new NeatGenome<double>(metaNeatGenome, 0, 0, connArr);
            return genome;
        }

        public static HashSet<int> GetNodeIdSet(NeatGenome<double> genome)
        {
            var idSet = new HashSet<int>();
            foreach(var connGene in genome.ConnectionGeneArray)
            {
                idSet.Add(connGene.SourceId);
                idSet.Add(connGene.TargetId);
            }
            return idSet;
        }

        public static HashSet<DirectedConnection> GetDirectedConnectionSet(NeatGenome<double> genome)
        {
            var idSet = new HashSet<DirectedConnection>();
            foreach(var connGene in genome.ConnectionGeneArray) {
                idSet.Add(new DirectedConnection(connGene.SourceId, connGene.TargetId));
            }
            return idSet;
        }

    }
}
