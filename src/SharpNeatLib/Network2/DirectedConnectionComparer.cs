using System.Collections.Generic;

namespace SharpNeat.Network2
{
    class DirectedConnectionComparer : IComparer<DirectedConnection>
    {
        // Pre-built re-usable instance.
        public static readonly DirectedConnectionComparer __Instance = new DirectedConnectionComparer();

        public int Compare(DirectedConnection x, DirectedConnection y)
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
