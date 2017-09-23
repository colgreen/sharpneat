using System.Diagnostics;

namespace SharpNeat.Network
{
    public struct ConnectionIdArrays
    {
        public readonly int[] _sourceIdArr;
        public readonly int[] _targetIdArr;

        public ConnectionIdArrays(int[] srcIdArr, int[] tgtIdArr)
        {
            Debug.Assert(srcIdArr.Length == tgtIdArr.Length);
            _sourceIdArr = srcIdArr;
            _targetIdArr = tgtIdArr;
        }        
    }
}
