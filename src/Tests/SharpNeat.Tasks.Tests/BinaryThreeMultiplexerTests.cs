using SharpNeat.Evaluation;
using SharpNeat.IO;
using SharpNeat.IO.Models;
using SharpNeat.NeuralNets.IO;
using SharpNeat.Tasks.BinaryThreeMultiplexer;
using Xunit;

namespace SharpNeat.Tasks;

public class BinaryThreeMultiplexerTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void VerifyNeuralNetResponse(bool enableHardwareAcceleration)
    {
        VerifyNeuralNetResponseInner(enableHardwareAcceleration);
    }

    private static void VerifyNeuralNetResponseInner(bool enableHardwareAcceleration)
    {
        NetFileModel netFileModel = NetFile.Load("TestData/binary-three-multiplexer.net");
        IBlackBox<double> blackBox = NeuralNetConverter.ToNeuralNet(
            netFileModel, enableHardwareAcceleration, enableHardwareAcceleration);

        // Evaluate the neural net.
        var evaluator = new BinaryThreeMultiplexerEvaluator<double>();

        // Confirm the expected fitness (to a limited amount of precision to allow for small variations of floating point
        // results that can occur as a result of platform/environmental variations).
        FitnessInfo fitnessInfo = evaluator.Evaluate(blackBox);
        Assert.Equal(107.50554956432657, fitnessInfo.PrimaryFitness, 6);
    }
}
