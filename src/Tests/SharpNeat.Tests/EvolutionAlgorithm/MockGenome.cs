using SharpNeat.Evaluation;

namespace SharpNeat.EvolutionAlgorithm.Tests;

public class MockGenome : IGenome
{
    public int Id { get; set; }

    public int BirthGeneration { get; set; }

    public FitnessInfo FitnessInfo { get; set; }

    public double Complexity { get; set; }
}
