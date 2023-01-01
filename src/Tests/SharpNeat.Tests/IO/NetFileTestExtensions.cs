using FluentAssertions;
using SharpNeat.IO.Models;

namespace SharpNeat.IO;

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
