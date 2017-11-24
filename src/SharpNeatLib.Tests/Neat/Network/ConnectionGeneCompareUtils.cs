using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;

namespace SharpNeatLib.Tests.Neat.Network
{
    public static class ConnectionGeneCompareUtils
    {
        #region Public Static Methods

        public static void CompareConnectionLists<T>(ConnectionGenes<T> x,
                                                     ConnectionIdArrays yIdArrays,
                                                     T[] yWeightArr)
            where T : struct
        {
            Assert.AreEqual(x.Length, yIdArrays._sourceIdArr.Length);
            Assert.AreEqual(x.Length, yIdArrays._targetIdArr.Length);
            Assert.AreEqual(x.Length, yWeightArr.Length);

            for(int i=0; i<x.Length; i++)  {
                Assert.IsTrue(AreEqual(x, yIdArrays, yWeightArr, i));
            }
        }

        public static bool AreEqual<T>(ConnectionGenes<T> x,
                                       ConnectionIdArrays yIdArrays,
                                       T[] yWeightArr, int idx)
            where T : struct
        {
            return x._connArr[idx].SourceId == yIdArrays._sourceIdArr[idx] 
                &&  x._connArr[idx].TargetId == yIdArrays._targetIdArr[idx] 
                &&  x._weightArr[idx].Equals(yWeightArr[idx]);
        }

        #endregion
    }
}
