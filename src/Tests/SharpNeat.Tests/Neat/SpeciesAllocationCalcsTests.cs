using Redzen.Random;
using SharpNeat.Evaluation;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Speciation;
using Xunit;

namespace SharpNeat.Neat.Tests;

public class SpeciesAllocationCalcsTests
{
    [Fact]
    public void UpdateSpeciesAllocationSizes()
    {
        IRandomSource rng = RandomDefaults.CreateRandomSource(0);

        NeatEvolutionAlgorithmSettings eaSettings = new()
        {
            SpeciesCount = 4
        };

        // Create population.
        NeatPopulation<double> neatPop = CreateNeatPopulation(100, eaSettings.SpeciesCount, 2, 2, 1.0);

        // Manually set-up some species.
        var speciesArr = neatPop.SpeciesArray;

        speciesArr[0].GenomeList.AddRange(neatPop.GenomeList.Take(25));
        speciesArr[1].GenomeList.AddRange(neatPop.GenomeList.Skip(25).Take(25));
        speciesArr[2].GenomeList.AddRange(neatPop.GenomeList.Skip(50).Take(25));
        speciesArr[3].GenomeList.AddRange(neatPop.GenomeList.Skip(75).Take(25));

        // Manually assign fitness scores to the genomes.
        speciesArr[0].GenomeList.ForEach(x => x.FitnessInfo = new FitnessInfo(100.0));
        speciesArr[1].GenomeList.ForEach(x => x.FitnessInfo = new FitnessInfo(200.0));
        speciesArr[2].GenomeList.ForEach(x => x.FitnessInfo = new FitnessInfo(400.0));
        speciesArr[3].GenomeList.ForEach(x => x.FitnessInfo = new FitnessInfo(800.0));

        // Invoke species target size calcs.
        neatPop.UpdateStats(PrimaryFitnessInfoComparer.Singleton, rng);
        SpeciesAllocationCalcs<double>.UpdateSpeciesAllocationSizes(
            neatPop, eaSettings, RandomDefaults.CreateRandomSource());

        // Species target sizes should be relative to the species mean fitness.
        double totalMeanFitness = 1500.0;
        double popSize = 100.0;

        Assert.Equal((100.0 / totalMeanFitness) * popSize, speciesArr[0].Stats.TargetSizeReal);
        Assert.Equal((200.0 / totalMeanFitness) * popSize, speciesArr[1].Stats.TargetSizeReal);
        Assert.Equal((400.0 / totalMeanFitness) * popSize, speciesArr[2].Stats.TargetSizeReal);
        Assert.Equal((800.0 / totalMeanFitness) * popSize, speciesArr[3].Stats.TargetSizeReal);

        // Note. Discretized target sizes will generally be equal to ceil(TargetSizeReal) or floor(TargetSizeReal),
        // but may not be due to the target size adjustment logic that is used to ensure that sum(TargetSizeInt) is equal
        // to the required population size.

        // Check that sum(TargetSizeInt) is equal to the required population size.
        Assert.Equal(speciesArr.Sum(x => x.Stats.TargetSizeInt), neatPop.GenomeList.Count);
    }

    #region Private Static Methods

    private static NeatPopulation<double> CreateNeatPopulation(
        int populationSize,
        int speciesCount,
        int inputNodeCount,
        int outputNodeCount,
        double connectionsProportion)
    {
        MetaNeatGenome<double> metaNeatGenome =
            MetaNeatGenome<double>.CreateAcyclic(
                inputNodeCount: inputNodeCount,
                outputNodeCount: outputNodeCount,
                activationFn: new NeuralNets.Double.ActivationFunctions.ReLU());

        NeatPopulation<double> neatPop = NeatPopulationFactory<double>.CreatePopulation(metaNeatGenome, connectionsProportion, populationSize, RandomDefaults.CreateRandomSource());
        neatPop.SpeciesArray = new Species<double>[speciesCount];

        for(int i=0; i < speciesCount; i++)
        {
            neatPop.SpeciesArray[i] = new Species<double>(i, null);
        }

        return neatPop;
    }

    #endregion
}
