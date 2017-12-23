using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Neat;
using SharpNeat.Neat.Genome;

namespace SharpNeatLib.Tests.Neat.Reproduction.Sexual.Strategy
{
    public class UniformCrossoverReproductionStrategyTestsUtils
    {
        static MetaNeatGenome<double> __metaNeatGenome;

        #region Static Initializer

        static UniformCrossoverReproductionStrategyTestsUtils()
        {
            __metaNeatGenome = CreateMetaNeatGenome();
        }

        #endregion

        #region Public Static Methods

        public static NeatPopulation<double> CreateNeatPopulation(int size)
        {
            var genomeList = new List<NeatGenome<double>>();
            switch(size)
            {
                case 1: 
                    genomeList.Add(CreateNeatGenome1());
                    break;
            }

            return new NeatPopulation<double>(__metaNeatGenome, genomeList);
        }

        #endregion

        #region Private Static Methods

        private static MetaNeatGenome<double> CreateMetaNeatGenome()
        {
            return new MetaNeatGenome<double>(
                inputNodeCount: 1,
                outputNodeCount: 1,
                isAcyclic: false,
                activationFn: new SharpNeat.NeuralNets.Double.ActivationFunctions.ReLU());
        }

        private static NeatGenome<double> CreateNeatGenome1()
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

            var genome = new NeatGenome<double>(__metaNeatGenome, 0, 0, connGenes);
            return genome;
        }

        private static NeatGenome<double> CreateNeatGenome2()
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

            var genome = new NeatGenome<double>(__metaNeatGenome, 0, 0, connGenes);
            return genome;
        }


        #endregion
    }
}
