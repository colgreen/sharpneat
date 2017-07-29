using System.Collections.Generic;

namespace SharpNeat.Network
{
    public static class DirectedConnectionUtils
    {
        public static bool IsSorted(IList<IDirectedConnection> connList)
        {
            if(connList.Count == 0) {
                return true;
            }

            IDirectedConnection prev = connList[0];
            for(int i=1; i < connList.Count; i++)
            {
                IDirectedConnection curr = connList[i];
                if(DirectedConnectionComparer.__Instance.Compare(prev, curr) > 0) {
                    return false;
                }
                prev = curr;
            }
            return true;
        }

        public static bool IsSorted<T>(IList<IWeightedDirectedConnection<T>> connList)
            where T : struct
        {
            if(connList.Count == 0) {
                return true;
            }

            IDirectedConnection prev = connList[0];
            for(int i=1; i < connList.Count; i++)
            {
                IDirectedConnection curr = connList[i];
                if(DirectedConnectionComparer.__Instance.Compare(prev, curr) > 0) {
                    return false;
                }
                prev = curr;
            }
            return true;
        }

    }
}
