using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Reproduction.Recombination.Strategies.UniformCrossover;

public class UniformCrossoverReproductionStrategyTestsUtils
{
    static readonly MetaNeatGenome<double> __metaNeatGenome;
    static readonly INeatGenomeBuilder<double> __genomeBuilder;

    static UniformCrossoverReproductionStrategyTestsUtils()
    {
        __metaNeatGenome = CreateMetaNeatGenome();
        __genomeBuilder = NeatGenomeBuilderFactory<double>.Create(__metaNeatGenome);
    }

    #region Public Static Methods

    public static NeatPopulation<double> CreateNeatPopulation(int size)
    {
        var genomeList = new List<NeatGenome<double>>();
        switch(size)
        {
            case 1:
                genomeList.Add(CreateNeatGenome1());
                break;
            case 2:
                genomeList.Add(CreateNeatGenome1());
                genomeList.Add(CreateNeatGenome2());
                break;
        }

        return new NeatPopulation<double>(
            __metaNeatGenome,
            __genomeBuilder,
            2,
            genomeList);
    }

    #endregion

    #region Private Static Methods

    private static MetaNeatGenome<double> CreateMetaNeatGenome()
    {
        return new MetaNeatGenome<double>(
            inputNodeCount: 1,
            outputNodeCount: 1,
            isAcyclic: false,
            cyclesPerActivation: 1,
            activationFn: new NeuralNets.ActivationFunctions.ReLU());
    }

    private static NeatGenome<double> CreateNeatGenome1()
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

        var genome = __genomeBuilder.Create(0, 0, connGenes);
        return genome;
    }

    private static NeatGenome<double> CreateNeatGenome2()
    {
        var connGenes = new ConnectionGenes<double>(11);
        connGenes[0] =   (0, 3, 0.1);
        connGenes[1] =   (0, 4, 0.2);
        connGenes[2] =   (0, 5, 0.3);

        connGenes[3] =   (3, 6, 0.4);
        connGenes[4] =   (4, 7, 0.5);

        connGenes[5] =   (6, 9, 0.7);
        connGenes[6] =   (7, 10, 0.8);
        connGenes[7] =   (8, 11, 0.9);

        connGenes[8] =   (9, 1, 1.0);
        connGenes[9] = (10, 1, 1.1);
        connGenes[10] = (11, 1, 1.2);

        var genome = __genomeBuilder.Create(0, 0, connGenes);
        return genome;
    }


    #endregion
}
