using SharpNeat.Evaluation;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Genome.Double;
using SharpNeat.Neat.Genome.IO;
using SharpNeat.NeuralNets;
using SharpNeat.Tasks.BinaryThreeMultiplexer;
using Xunit;

namespace SharpNeat.Tasks.Tests;

public class BinaryThreeMultiplexerTests
{
    [Fact]
    public void VerifyNeuralNetResponse()
    {
        VerifyNeuralNetResponseInner(false);
    }

    [Fact]
    public void VerifyNeuralNetResponse_EnableHardwareAcceleration()
    {
        VerifyNeuralNetResponseInner(true);
    }

    private static void VerifyNeuralNetResponseInner(bool enableHardwareAcceleration)
    {
        var activationFnFactory = new DefaultActivationFunctionFactory<double>(enableHardwareAcceleration);
        var metaNeatGenome = MetaNeatGenome<double>.CreateAcyclic(
            4, 1, activationFnFactory.GetActivationFunction("LeakyReLU"));

        // Load test genome.
        NeatGenome<double> genome = NeatGenomeLoader.Load("TestData/binary-three-multiplexer.net", metaNeatGenome, 0);

        // Decode genome to a neural net.
        var genomeDecoder = NeatGenomeDecoderFactory.CreateGenomeDecoder(true);
        IBlackBox<double> blackBox = genomeDecoder.Decode(genome);

        // Evaluate the neural net.
        var evaluator = new BinaryThreeMultiplexerEvaluator();

        // Confirm the expected fitness (to a limited amount of precision to allow for small variations of floating point
        // results that can occur as a result of platform/environmental variations).
        FitnessInfo fitnessInfo = evaluator.Evaluate(blackBox);
        Assert.Equal(107.50554956432657, fitnessInfo.PrimaryFitness, 6);
    }
}
