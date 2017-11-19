using System;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    internal static class AddConnectionUtils
    {
        public static int[] CreateConnectionIndexArray<T>(
            NeatGenome<T> parent, int insertIdx, 
            int connectionId, bool highInnovationId)
            where T : struct
        {
            // Alloc connection index array.
            int parentLen = parent.ConnectionGeneArray.Length;
            var connIdxArr = new int[parentLen+1];

            int insertIdxB;
            if(highInnovationId)
            {    
                // New innovation IDs will always be the higher than any existing ID, thus they would be sorted to the end of the array, 
                // i.e. we can avoid the cost of looking up the insertion point because we know it is at the end of the array.
                insertIdxB = parentLen;

                // Copy indexes from the parent to the child array.
                Array.Copy(parent.ConnectionIndexArray, connIdxArr, parentLen);
                connIdxArr[insertIdxB] = insertIdx;
            }
            else
            {
                // Lookup the insertion index in parent.ConnectionIndexArray.
                insertIdxB = ~ConnectionGeneUtils.BinarySearchId(parent.ConnectionIndexArray, parent.ConnectionGeneArray, connectionId);

                // Copy indexes from the parent to the child array; with the new index inserted in its sorted position.
                Array.Copy(parent.ConnectionIndexArray, connIdxArr, insertIdxB);
                connIdxArr[insertIdxB] = insertIdx;
                Array.Copy(parent.ConnectionIndexArray, insertIdxB, connIdxArr, insertIdxB + 1, parentLen - insertIdxB);
            }

            // All connections after the inserted connection will have been shifted by one element to make space, therefore
            // indexes to these genes will need incrementing by one.
            for(int i=0; i < connIdxArr.Length; i++) {
                if(connIdxArr[i] >= insertIdx) { connIdxArr[i]++; }
            }

            // Revert the increment on the inserted element.
            connIdxArr[insertIdxB]--;

            return connIdxArr;
        }

    }
}
