using System.Reflection;
using FluentAssertions;
using SharpNeat.Graphs;
using SharpNeat.IO;
using SharpNeat.IO.Models;
using SharpNeat.NeuralNets.Double;
using Xunit;

namespace SharpNeat.NeuralNets.IO;

#pragma warning disable CA1861 // Avoid constant arrays as arguments

public class NeuralNetConverterTests
{
    [Fact]
    public void NetFileModelShouldConvertToNeuralNetAcyclic()
    {
        // Arrange.
        NetFileModel netFileModel = NetFile.Load("TestData/example2.net");

        // Act.
        IBlackBox<double> box = NeuralNetConverter.ToNeuralNet(netFileModel);
        var inputs = box.Inputs.Span;
        inputs[0] = 3.0;
        inputs[1] = 7.0;
        inputs[2] = 11.0;
        box.Activate();

        // Assert.
        box.Should().NotBeNull();
        box.GetType().Should().Be(typeof(NeuralNetAcyclic));
        box.Inputs.Length.Should().Be(3);
        box.Outputs.Length.Should().Be(2);

        // Inspect internal connections data.
        // Assert weights.
        Type netType = typeof(NeuralNetAcyclic);
        double[] weightArr = (double[])netType.GetField(
            "_weightArr", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(box);
        weightArr.Should().BeEquivalentTo(
            new double[] { 0.123, 1.234, -0.5835, 2.5, 5.123456789, 5.4 },
            o => o.WithStrictOrdering());

        // Assert source and target connection IDs.
        ConnectionIds connIds = (ConnectionIds)netType.GetField(
            "_connIds", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(box);

        Span<int> sourceIds = connIds.GetSourceIdSpan();
        Span<int> targetIds = connIds.GetTargetIdSpan();

        // Notes.
        // Nodes 4 and 5 swap IDs, because node 4 in NetFile definition has a higher depth (2) than node 5 (depth of 1).
        // Nodes 4 and 5 also swap places!
        sourceIds.ToArray().Should().BeEquivalentTo(new int[] { 0, 1, 2, 2, 2, 4 }, o => o.WithStrictOrdering());
        targetIds.ToArray().Should().BeEquivalentTo(new int[] { 3, 3, 3, 4, 5, 5 }, o => o.WithStrictOrdering());

        // Inspect the black box outputs.
        var outputs = box.Outputs.Span;
        outputs[0].Should().BeApproximately(2.5885, 6);
        outputs[1].Should().BeApproximately(204.8580247, 6);
    }

    [Fact]
    public void NetFileModelShouldConvertToNeuralNetCyclic()
    {
        // Arrange.
        NetFileModel netFileModel = NetFile.Load("TestData/example3.net");

        // Act.
        IBlackBox<double> box = NeuralNetConverter.ToNeuralNet(netFileModel);
        var inputs = box.Inputs.Span;
        inputs[0] = 3.0;
        inputs[1] = 5.0;
        inputs[2] = 7.0;
        box.Activate();

        // Assert.
        box.Should().NotBeNull();
        box.GetType().Should().Be(typeof(NeuralNetCyclic));
        box.Inputs.Length.Should().Be(3);
        box.Outputs.Length.Should().Be(2);

        // Inspect internal connections data.
        // Assert weights.
        Type netType = typeof(NeuralNetCyclic);
        double[] weightArr = (double[])netType.GetField(
            "_weightArr", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(box);
        weightArr.Should().BeEquivalentTo(
            new double[] { 3.0, 5.0, 7.0, 11.0, 13.0, 29.0, 17.0, 23.0, 19.0 },
            o => o.WithStrictOrdering());

        // Assert source and target connection IDs.
        ConnectionIds connIds = (ConnectionIds)netType.GetField(
            "_connIds", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(box);

        Span<int> sourceIds = connIds.GetSourceIdSpan();
        Span<int> targetIds = connIds.GetTargetIdSpan();

        sourceIds.ToArray().Should().BeEquivalentTo(
            new int[] { 0, 1, 1, 2, 5, 6, 6, 7, 7},
            o => o.WithStrictOrdering());

        targetIds.ToArray().Should().BeEquivalentTo(
            new int[] { 5, 5, 6, 6, 6, 4, 7, 3, 5},
            o => o.WithStrictOrdering());

        // Assert that cyclesPeractication.
        int cyclesPerActivation = (int)netType.GetField(
            "_cyclesPerActivation", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(box);
        cyclesPerActivation.Should().Be(3);
    }
}
