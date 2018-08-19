using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNeat.BlackBox;
using static SharpNeat.Tests.TestUtils;

namespace SharpNeat.Tests.BlackBox
{
    [TestClass]
    public class VectorSegmentTests
    {
        #region Test Methods [Segment Tests]

        [TestMethod]
        [TestCategory("VectorSegment")]
        public void SimpleSegment1()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 3);

            // Test the segment yields the expected values.
            int[] expectedArr = new int[] { 5, 6, 7 };
            Compare(expectedArr, vecSeg);
        }

        [TestMethod]
        [TestCategory("VectorSegment")]
        public void SimpleSegment2()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 0, 10);

            // Test the segment yields the expected values.
            int[] expectedArr = Enumerable.Range(0, 10).ToArray();
            Compare(expectedArr, vecSeg);
        }

        [TestMethod]
        [TestCategory("VectorSegment")]
        public void InvalidSegments()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();

            // Test constructor throws exceptions for invalid ranges over the inner array.
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new VectorSegment<int>(innerArr, 8, 3));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new VectorSegment<int>(innerArr, 9, 2));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new VectorSegment<int>(innerArr, 10, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new VectorSegment<int>(innerArr, 0, 11));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new VectorSegment<int>(innerArr, -1, 1));
        }

        #endregion

        #region Test Methods [CopyTo Tests]

        [TestMethod]
        [TestCategory("VectorSegment")]
        public void CopyToTest1()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 3);

            int[] tgtArr = new int[12];
            vecSeg.CopyTo(tgtArr, 2);

            // Test the segment yields the expected values.
            int[] expectedArr = new int[] { 0, 0, 5, 6, 7, 0, 0, 0, 0, 0, 0, 0 };
            Compare(expectedArr, tgtArr);
        }

        [TestMethod]
        [TestCategory("VectorSegment")]
        public void CopyToTest2()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 3);

            int[] tgtArr = new int[12];
            vecSeg.CopyTo(tgtArr, 2, 2);

            // Test the segment yields the expected values.
            int[] expectedArr = new int[] { 0, 0, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0 };
            Compare(expectedArr, tgtArr);
        }

        [TestMethod]
        [TestCategory("VectorSegment")]
        public void CopyToTest3()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 3);

            int[] tgtArr = new int[12];
            vecSeg.CopyTo(tgtArr, 2, 1, 2);

            // Test the segment yields the expected values.
            int[] expectedArr = new int[] { 0, 0, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0 };
            Compare(expectedArr, tgtArr);
        }

        [TestMethod]
        [TestCategory("VectorSegment")]
        public void CopyToTest_InvalidCopyOperations()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 3);

            int[] tgtArr = new int[12];

         //--- Two param tests.
            // Copy beyond end of tgtArr.
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyTo(tgtArr, 10));

            // Invalid target index.
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => vecSeg.CopyTo(tgtArr, -1));

         //--- Three param tests.
            // Copy length longer then vecSeg length.
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyTo(tgtArr, 0, 4));

            // Invalid target index.
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => vecSeg.CopyTo(tgtArr, -1, 1));

            // Invalid length.
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => vecSeg.CopyTo(tgtArr, 0, -1));

            // Copy beyond end of tgtArr.
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyTo(tgtArr, 11, 2));
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyTo(tgtArr, 12, 1));

         //--- Four param tests.         
            // Copy beyond end of vecSeg.
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyTo(tgtArr, 0, 1, 3));

            // Copy beyond end of tgtArr.
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyTo(tgtArr, 11, 1, 2));
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyTo(tgtArr, 12, 1, 1));

            // Invalid source and target indexes.
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => vecSeg.CopyTo(tgtArr, -1, 0, 1));
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyTo(tgtArr, 0, -1, 1));

            // Invalid length.
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => vecSeg.CopyTo(tgtArr, 0, 1, -1));
        }

        #endregion

        #region Test Methods [CopyFrom Tests]

        [TestMethod]
        [TestCategory("VectorSegment")]
        public void CopyFromTest1()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 3);

            int[] srcArr = Enumerable.Range(100, 3).ToArray();
            vecSeg.CopyFrom(srcArr, 0);

            // Test the segment yields the expected values.
            int[] expectedInnerArr = new int[] { 0, 1, 2, 3, 4, 100, 101, 102, 8, 9 };
            Compare(expectedInnerArr, innerArr);
        }

        [TestMethod]
        [TestCategory("VectorSegment")]
        public void CopyFromTest2()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 3);

            int[] srcArr = Enumerable.Range(100, 2).ToArray();
            vecSeg.CopyFrom(srcArr, 1);

            // Test the segment yields the expected values.
            int[] expectedInnerArr = new int[] { 0, 1, 2, 3, 4, 5, 100, 101, 8, 9 };
            Compare(expectedInnerArr, innerArr);
        }

        [TestMethod]
        [TestCategory("VectorSegment")]
        public void CopyFromTest3()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 3);

            int[] srcArr = Enumerable.Range(100, 2).ToArray();
            vecSeg.CopyFrom(srcArr, 1, 1);

            // Test the segment yields the expected values.
            int[] expectedInnerArr = new int[] { 0, 1, 2, 3, 4, 5, 100, 7, 8, 9 };
            Compare(expectedInnerArr, innerArr);
        }

        [TestMethod]
        [TestCategory("VectorSegment")]
        public void CopyFromTest4()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 3);

            int[] srcArr = Enumerable.Range(100, 10).ToArray();
            vecSeg.CopyFrom(srcArr, 5, 1, 2);

            // Test the segment yields the expected values.
            int[] expectedInnerArr = new int[] { 0, 1, 2, 3, 4, 5, 105, 106, 8, 9 };
            Compare(expectedInnerArr, innerArr);
        }

        [TestMethod]
        [TestCategory("VectorSegment")]
        public void CopyFromTest_InvalidCopyOperations()
        {
            int[] innerArr = Enumerable.Range(0, 10).ToArray();
            var vecSeg = new VectorSegment<int>(innerArr, 5, 4);

            int[] srcArr = Enumerable.Range(100, 3).ToArray();

         //--- Two param tests.
            // Copy beyond end of vecSeg.
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 2));

            // Invalid target index.
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyFrom(srcArr, -1));
            
         //--- Three param tests.

            // Copy length longer than srcArr.
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 0, 4));

            // Invalid source index.
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyFrom(srcArr, -1, 1));

            // Invalid length.
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => vecSeg.CopyFrom(srcArr, 0, -1));

            // Copy beyond the end of vecSeg.
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 2, 3));
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 3, 2));
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 4, 1));

         // Four param tests.
            
            // Copy beyond end of srcArr.
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 0, 0, 4));
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 1, 0, 3));
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 2, 0, 2));
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 3, 0, 1));

            // Copy beyond the end of vecSeg.
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 0, 3, 2));
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 0, 2, 3));
            
            // Invalid source and target indexes.
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => vecSeg.CopyFrom(srcArr, -1, 0, 1));
            Assert.ThrowsException<ArgumentException>(() => vecSeg.CopyFrom(srcArr, 0, -1, 1));

            // Invalid length.
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => vecSeg.CopyFrom(srcArr, 0, 0, -1));
        }

        #endregion
    }
}
