using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    internal static class AddConnectionUtils
    {
        public static int GetNodeIdFromIndex<T>(NeatGenome<T> parent, int idx)
            where T : struct
        {
            // For input/output nodes their index is their ID.
            if(idx < parent.MetaNeatGenome.InputOutputNodeCount) {
                return idx;
            }

            // All other nodes are hidden nodes; use a pre-built array of all hidden node IDs.
            return parent.HiddenNodeIdArray[idx - parent.MetaNeatGenome.InputOutputNodeCount];
        }
    }
}
