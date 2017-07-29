using System;
using System.Collections.Generic;

namespace SharpNeat.Network2
{
    public class DirectedConnectionComparer : IComparer<DirectedConnection>, IComparer<IDirectedConnection>
    {
        // Pre-built re-usable instance.
        public static readonly DirectedConnectionComparer __Instance = new DirectedConnectionComparer();

        public int Compare(DirectedConnection x, DirectedConnection y)
        {
            return Compare((IDirectedConnection)x, (IDirectedConnection)y);
        }

        public int Compare(IDirectedConnection x, IDirectedConnection y)
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
