using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Phenomes;

namespace SharpNeatLib.Tests.Phenomes
{
    public static class VectorUtils
    {
        #region Public Static Methods

        public static void Compare(int[] expectedArr, IVector<int> vec)
        {
            Assert.AreEqual(expectedArr.Length, vec.Length);
            for(int i=0; i<expectedArr.Length; i++) {
                Assert.AreEqual(expectedArr[i], vec[i]);
            }
        }

        public static void Compare(int[] expectedArr, int[] actualArr)
        {
            Assert.AreEqual(expectedArr.Length, actualArr.Length);
            for(int i=0; i<expectedArr.Length; i++) {
                Assert.AreEqual(expectedArr[i], actualArr[i]);
            }
        }

        #endregion
    }
}
