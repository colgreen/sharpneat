using FluentAssertions;
using SharpNeat.Graphs;

namespace SharpNeat.Neat.Genome;

internal static class NeatGenomeTestExtensions
{
    public static void Validate(
        this DirectedConnection conn,
        int srcId, int tgtId)
    {
        conn.SourceId.Should().Be(srcId);
        conn.TargetId.Should().Be(tgtId);
    }
}
