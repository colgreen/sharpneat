using SharpNeat.Evaluation;
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeatLib.Tests.EvolutionAlgorithm
{
    public class MockGenome : IGenome
    {
        public int Id { get; set; }

        public int BirthGeneration { get; set; }

        public FitnessInfo FitnessInfo { get; set; }

        public double Complexity { get; set; }
    }
}
