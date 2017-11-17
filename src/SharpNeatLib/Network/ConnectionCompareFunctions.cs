namespace SharpNeat.Network
{
    public static class ConnectionCompareFunctions
    {
        public static int Compare(DirectedConnection x, DirectedConnection y)
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

        public static int Compare<T>(WeightedDirectedConnection<T> x, WeightedDirectedConnection<T> y)
            where T : struct
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
