using System.Collections.Generic;

namespace SharpNeat.Network
{
    internal class ConnectionComparer : IComparer<int>
    {
        int[] _srcIdArr;
        int[] _tgtIdArr;

        public ConnectionComparer(ConnectionIdArrays connIdArrays)
        {
            _srcIdArr = connIdArrays._sourceIdArr;
            _tgtIdArr = connIdArrays._targetIdArr;
        }

        public int Compare(int x, int y)
        {
            // Compare source IDs.
            int xval = _srcIdArr[x];
            int yval = _srcIdArr[y];

            if(xval < yval) {
                return -1;
            }
            else if(xval > yval) {
                return 1;
            }

            // Source IDs are equal; compare target IDs.
            xval = _tgtIdArr[x];
            yval = _tgtIdArr[y];

            if(xval < yval) {
                return -1;
            }
            else if(xval > yval) {
                return 1;
            }          

            return 0;
        }
    }
}
