using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.Phenomes;
using static SharpNeatLib.Tests.Phenomes.VectorUtils;

namespace SharpNeatLib.Tests.Phenomes
{
    [TestClass]
    public class MappingVectorTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("MappingVector")]
        public void SimpleMapping()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            int[] map = new int[] { 5, 3, 8, 0, 9 };
            var mappingVec = new MappingVector<int>(innerArr, map);

            int[] expectedArr = new int[] { 5, 3, 8, 0, 9 };
            Compare(expectedArr, mappingVec);
        }





        private static void Foo()
        {

        }



        #endregion


    }
}
