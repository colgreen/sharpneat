using System;
using System.Linq;
using SharpNeat.BlackBox;
using Xunit;
using static SharpNeat.Tests.ArrayTestUtils;

namespace SharpNeat.Tests.BlackBox
{
    public class MappingVectorTests
    {
        #region Test Methods

        [Fact]
        public void SimpleMapping()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            int[] map = new int[] { 5, 3, 8, 0, 9 };
            var mappingVec = new MappingVector<int>(innerArr, map);

            int[] expectedArr = new int[] { 5, 3, 8, 0, 9 };
            ConponentwiseEqual(expectedArr, mappingVec);
        }

        #endregion

        #region Test Methods [CopyTo Tests]

        [Fact]
        public void CopyToTest1()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            int[] map = new int[] { 5, 3, 8, 0, 9 };
            var mappingVec = new MappingVector<int>(innerArr, map);

            int[] tgtArr = new int[12];
            mappingVec.CopyTo(tgtArr, 2);

            // Test the segment yields the expected values.
            int[] expectedArr = new int[] { 0, 0, 5, 3, 8, 0, 9, 0, 0, 0, 0, 0 };
            Assert.Equal(expectedArr, tgtArr);
        }

        [Fact]
        public void CopyToTest2()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            int[] map = new int[] { 5, 3, 8, 0, 9 };
            var mappingVec = new MappingVector<int>(innerArr, map);

            int[] tgtArr = new int[12];
            mappingVec.CopyTo(tgtArr, 2, 2);

            // Test the segment yields the expected values.
            int[] expectedArr = new int[] { 0, 0, 5, 3, 0, 0, 0, 0, 0, 0, 0, 0 };
            Assert.Equal(expectedArr, tgtArr);
        }

        [Fact]
        public void CopyToTest3()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            int[] map = new int[] { 5, 3, 8, 0, 9 };
            var mappingVec = new MappingVector<int>(innerArr, map);

            int[] tgtArr = new int[12];
            mappingVec.CopyTo(tgtArr, 2, 1, 2);

            // Test the segment yields the expected values.
            int[] expectedArr = new int[] { 0, 0, 3, 8, 0, 0, 0, 0, 0, 0, 0, 0 };
            Assert.Equal(expectedArr, tgtArr);
        }

        [Fact]
        public void CopyToTest_InvalidCopyOperations()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            int[] map = new int[] { 5, 3, 8 };
            var mappingVec = new MappingVector<int>(innerArr, map);

            int[] tgtArr = new int[12];

         //--- Two param tests.
            // Copy beyond end of tgtArr.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyTo(tgtArr, 10));

            // Invalid target index.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyTo(tgtArr, -1));

         //--- Three param tests.
            // Copy length longer then vecSeg length.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyTo(tgtArr, 0, 4));

            // Invalid target index.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyTo(tgtArr, -1, 1));

            // Invalid length.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyTo(tgtArr, 0, -1));

            // Copy beyond end of tgtArr.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyTo(tgtArr, 11, 2));
            Assert.Throws<ArgumentException>(() => mappingVec.CopyTo(tgtArr, 12, 1));

         //--- Four param tests.         
            // Copy beyond end of vecSeg.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyTo(tgtArr, 0, 1, 3));

            // Copy beyond end of tgtArr.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyTo(tgtArr, 11, 1, 2));
            Assert.Throws<ArgumentException>(() => mappingVec.CopyTo(tgtArr, 12, 1, 1));

            // Invalid source and target indexes.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyTo(tgtArr, -1, 0, 1));
            Assert.Throws<ArgumentException>(() => mappingVec.CopyTo(tgtArr, 0, -1, 1));

            // Invalid length.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyTo(tgtArr, 0, 1, -1));
        }

        #endregion

        #region Test Methods [CopyFrom Tests]

        [Fact]
        public void CopyFromTest1()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            int[] map = new int[] { 5, 3, 8, 0, 9 };
            var mappingVec = new MappingVector<int>(innerArr, map);

            int[] srcArr = Enumerable.Range(100, 3).ToArray();
            mappingVec.CopyFrom(srcArr, 0);

            // Test the segment yields the expected values.
            int[] expectedInnerArr = new int[] { 0, 1, 2, 101, 4, 100, 6, 7, 102, 9 };
            Assert.Equal(expectedInnerArr, innerArr);
        }

        [Fact]
        public void CopyFromTest2()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            int[] map = new int[] { 5, 3, 8, 0, 9 };
            var mappingVec = new MappingVector<int>(innerArr, map);

            int[] srcArr = Enumerable.Range(100, 3).ToArray();
            mappingVec.CopyFrom(srcArr, 1);

            // Test the segment yields the expected values.
            int[] expectedInnerArr = new int[] { 102, 1, 2, 100, 4, 5, 6, 7, 101, 9 };
            Assert.Equal(expectedInnerArr, innerArr);
        }

        [Fact]
        public void CopyFromTest3()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            int[] map = new int[] { 5, 3, 8, 0, 9 };
            var mappingVec = new MappingVector<int>(innerArr, map);

            int[] srcArr = Enumerable.Range(100, 3).ToArray();
            mappingVec.CopyFrom(srcArr, 1, 1);

            // Test the segment yields the expected values.
            int[] expectedInnerArr = new int[] { 0, 1, 2, 100, 4, 5, 6, 7, 8, 9 };
            Assert.Equal(expectedInnerArr, innerArr);
        }

        [Fact]
        public void CopyFromTest4()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            int[] map = new int[] { 5, 3, 8, 0, 9 };
            var mappingVec = new MappingVector<int>(innerArr, map);

            int[] srcArr = Enumerable.Range(100, 10).ToArray();
            mappingVec.CopyFrom(srcArr, 5, 1, 2);

            // Test the segment yields the expected values.
            int[] expectedInnerArr = new int[] { 0, 1, 2, 105, 4, 5, 6, 7, 106, 9 };
            Assert.Equal(expectedInnerArr, innerArr);
        }

        [Fact]
        public void CopyFromTest_InvalidCopyOperations()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            int[] map = new int[] { 5, 3, 8, 0 };
            var mappingVec = new MappingVector<int>(innerArr, map);

            int[] srcArr = Enumerable.Range(100, 3).ToArray();

         //--- Two param tests.
            // Copy beyond end of vecSeg.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyFrom(srcArr, 2));

            // Invalid target index.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyFrom(srcArr, -1));
            
         //--- Three param tests.

            // Copy length longer than srcArr.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyFrom(srcArr, 0, 4));

            // Invalid source index.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyFrom(srcArr, -1, 1));

            // Invalid length.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyFrom(srcArr, 0, -1));

            // Copy beyond the end of vecSeg.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyFrom(srcArr, 2, 3));
            Assert.Throws<ArgumentException>(() => mappingVec.CopyFrom(srcArr, 3, 2));
            Assert.Throws<ArgumentException>(() => mappingVec.CopyFrom(srcArr, 4, 1));

         // Four param tests.
            
            // Copy beyond end of srcArr.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyFrom(srcArr, 0, 0, 4));
            Assert.Throws<ArgumentException>(() => mappingVec.CopyFrom(srcArr, 1, 0, 3));
            Assert.Throws<ArgumentException>(() => mappingVec.CopyFrom(srcArr, 2, 0, 2));
            Assert.Throws<ArgumentException>(() => mappingVec.CopyFrom(srcArr, 3, 0, 1));

            // Copy beyond the end of vecSeg.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyFrom(srcArr, 0, 3, 2));
            Assert.Throws<ArgumentException>(() => mappingVec.CopyFrom(srcArr, 0, 2, 3));
            
            // Invalid source and target indexes.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyFrom(srcArr, -1, 0, 1));
            Assert.Throws<ArgumentException>(() => mappingVec.CopyFrom(srcArr, 0, -1, 1));

            // Invalid length.
            Assert.Throws<ArgumentException>(() => mappingVec.CopyFrom(srcArr, 0, 0, -1));
        }

        #endregion
    }
}
