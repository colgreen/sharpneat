using FluentAssertions;
using SharpNeat.IO.Models;
using Xunit;

namespace SharpNeat.IO;

public class NetFileTests
{
    [Fact]
    public void Load()
    {
        NetFileModel netFileModel = NetFile.Load("TestData/example1.genome");

        // Test input/output counts.
        netFileModel.InputCount.Should().Be(3);
        netFileModel.OutputCount.Should().Be(2);

        // Test cyclic/acyclic indicator.
        netFileModel.IsAcyclic.Should().BeTrue();

        // Test connection lines.
        var conns = netFileModel.Connections;
        conns.Should().NotBeNull().And.HaveCount(12);
        conns[0].Validate(0, 5, 0.5);
        conns[1].Validate(0, 7, 0.7);
        conns[2].Validate(0, 3, 0.3);
        conns[3].Validate(1, 5, 1.5);
        conns[4].Validate(1, 7, 1.7);
        conns[5].Validate(1, 3, 1.3);
        conns[6].Validate(1, 6, 1.6);
        conns[7].Validate(1, 8, 1.8);
        conns[8].Validate(1, 4, 1.4);
        conns[9].Validate(2, 6, 2.6);
        conns[10].Validate(2, 8, 2.8);
        conns[11].Validate(2, 4, 2.4);

        // Test Activation functions.
        var actFns = netFileModel.ActivationFns;
        actFns.Should().NotBeNull().And.HaveCount(4);
        actFns[0].Validate(0, "ReLU");
        actFns[1].Validate(1, "Logistic");
        actFns[2].Validate(2, "Sine");
        actFns[3].Validate(3, "Gaussian");
    }
}

internal static class NetFileTestExtensions
{
    public static void Validate(
        this ConnectionLine connLine,
        int srcId, int tgtId, double weight)
    {
        connLine.SourceId.Should().Be(srcId);
        connLine.TargetId.Should().Be(tgtId);
        connLine.Weight.Should().Be(weight);
    }

    public static void Validate(
        this ActivationFnLine actFnLine,
        int id, string code)
    {
        actFnLine.Id.Should().Be(id);
        actFnLine.Code.Should().Be(code);
    }
}
