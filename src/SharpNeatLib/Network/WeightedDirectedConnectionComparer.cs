using System.Collections.Generic;

namespace SharpNeat.Network
{
    public class WeightedDirectedConnectionComparer<T> : IComparer<WeightedDirectedConnection<T>>
        where T : struct
    {
        public readonly static WeightedDirectedConnectionComparer<T> Default = new WeightedDirectedConnectionComparer<T>();

        public int Compare(
            WeightedDirectedConnection<T> x,
            WeightedDirectedConnection<T> y)
        {
            // Compare source IDs.
            if (x.SourceId < y.SourceId) {
                return -1;
            }
            if (x.SourceId > y.SourceId) {
                return 1;
            }

            // Source IDs are equal; compare target IDs.
            if (x.TargetId < y.TargetId) {
                return -1;
            }
            if (x.TargetId > y.TargetId) {
                return 1;
            }
            return 0;
        }
    }
}
