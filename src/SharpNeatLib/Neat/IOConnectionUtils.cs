using SharpNeat.Network;

namespace SharpNeat.Neat
{
    internal static class IOConnectionUtils
    {
        public static bool TryGetInputOutputConnectionId(
            DirectedConnection key, 
            int inputCount, int outputCount,       
            out int connectionId)
        {
            // Test for a source node that is one of the input nodes, and a target node that is one of the output nodes.
            if(IsInputOutputConnection(key, inputCount, outputCount))
            {
                // Adjust for the fact that the output node IDs start where the input node IDs finish.
                int outputIdx = key.TargetId - inputCount;
                int ioCount = inputCount + outputCount;
                connectionId = (key.SourceId * outputCount) + outputIdx + ioCount;
                return true;
            }

            connectionId = default(int);
            return false;
        }

        public static bool IsInputOutputConnection(DirectedConnection key, int inputCount, int outputCount)
        {
            return (key.SourceId < inputCount) && (key.TargetId >= inputCount && key.TargetId < inputCount + outputCount);
        }

        public static bool IsInputOutputInnovationId(int id, int inputCount, int outputCount)
        {
            return (id >= inputCount + outputCount)
                && (id < (inputCount * outputCount) + inputCount + outputCount);
        }
    }
}
