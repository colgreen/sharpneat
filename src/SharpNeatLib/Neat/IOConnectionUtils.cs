using SharpNeat.Network;

namespace SharpNeat.Neat
{
    internal static class IOConnectionUtils
    {
        public static bool IsInputOutputConnection(DirectedConnection key, int inputCount, int outputCount)
        {
            return (key.SourceId < inputCount) && (key.TargetId >= inputCount && key.TargetId < inputCount + outputCount);
        }
    }
}
