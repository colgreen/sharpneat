using System.Collections.Generic;

namespace SharpNeat.Network
{
    public static class DirectedConnectionUtils
    {
        public static bool IsSorted(IList<DirectedConnection> connList)
        {
            if(connList.Count == 0) {
                return true;
            }

            DirectedConnection prev = connList[0];
            for(int i=1; i < connList.Count; i++)
            {
                DirectedConnection curr = connList[i];
                if(ConnectionCompareFunctions.Compare(prev, curr) > 0) {
                    return false;
                }
                prev = curr;
            }
            return true;
        }

        public static bool IsSorted<T>(IList<WeightedDirectedConnection<T>> connList)
            where T : struct
        {
            if(connList.Count == 0) {
                return true;
            }

            var prev = connList[0];
            for(int i=1; i < connList.Count; i++)
            {
                var curr = connList[i];
                if(ConnectionCompareFunctions.Compare(prev, curr) > 0) {
                    return false;
                }
                prev = curr;
            }
            return true;
        }

    }
}
